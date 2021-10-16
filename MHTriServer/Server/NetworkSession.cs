using log4net;
using MHTriServer.Server.Packets;
using MHTriServer.Utils;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MHTriServer.Server
{
    public class NetworkSession : IDisposable
    {
        private const int PACKET_HEADER_SIZE = 8;
        private const int SPECIAL_MARK_POSITION = -1;
        private const int DEFAULT_BUFFER_SIZE = 0x2000;

        private static readonly ILog Log = LogManager.GetLogger(nameof(NetworkSession));

        private MemoryStream m_ReadStream;
        private Stopwatch m_LastReceived;

        private BEBinaryWriter m_WriteStreamWriter;
        private Stopwatch m_LastWrite;

        private ushort m_PacketCounter;

        private bool m_SentPingPacket;

        private object m_Tag;

        public Socket Socket { get; private set; }

        public EndPoint RemoteEndPoint { get; }

        public Stream NetworkStream { get; private set; }

        public ushort NextResponseCounter { get; set; }

        public bool Connected => Socket?.Connected ?? false;

        public NetworkSession(Socket socket, Stream newtworkStream)
        {
            Debug.Assert(socket != null);
            Debug.Assert(newtworkStream != null);

            m_ReadStream = new MemoryStream(new byte[DEFAULT_BUFFER_SIZE], 0, DEFAULT_BUFFER_SIZE, true, true);
            m_LastReceived = Stopwatch.StartNew();

            m_WriteStreamWriter = new BEBinaryWriter(new MemoryStream(DEFAULT_BUFFER_SIZE));
            m_LastWrite = Stopwatch.StartNew();

            m_PacketCounter = 0;

            m_SentPingPacket = false;

            m_Tag = null;

            Socket = socket;
            RemoteEndPoint = socket.RemoteEndPoint;
            NetworkStream = newtworkStream;

            NextResponseCounter = 0;
        }

        public void SetTag(object tag) => m_Tag = tag;

        /// <summary>
        /// The caller must ensure that the returned object is indeed of type T
        /// </summary>
        /// <typeparam name="T">Type to cast the tag</typeparam>
        /// <returns>Tag casted as T</returns>
        public T GetTag<T>() => (T)m_Tag;

        private (long, long) ReadNetworkStream(int? leftToRead = null)
        {
            var initialReadStreamPosition = m_ReadStream.Position;

            var totalLeftToRead = leftToRead ?? PACKET_HEADER_SIZE;
            var readPacketSize = leftToRead.HasValue;

            while (totalLeftToRead > 0)
            {
                var bytesRead = NetworkStream.Read(m_ReadStream.GetBuffer(), (int)m_ReadStream.Position, (int)(m_ReadStream.Length - m_ReadStream.Position));
                Debug.Assert(bytesRead > 0);

                totalLeftToRead -= bytesRead;

                if (!readPacketSize && totalLeftToRead < 6)
                {
                    // Read the packet size and append it
                    var span = new Span<byte>(m_ReadStream.GetBuffer(), (int)initialReadStreamPosition, sizeof(ushort));
                    totalLeftToRead += BinaryPrimitives.ReadUInt16BigEndian(span);
                    readPacketSize = true;
                }

                // Manually advance the buffer pointer since we are using the stream underlying buffer directly
                m_ReadStream.Position += bytesRead;
            }

            var bytesReadFromSocket = m_ReadStream.Position - initialReadStreamPosition;

            // Rewind so we can read the readed bytes from the socket
            m_ReadStream.Position = initialReadStreamPosition;

            return (initialReadStreamPosition, bytesReadFromSocket);
        }

        public IReadOnlyList<Packet> ReadPackets()
        {
            Debug.Assert(Socket.Available > 0);

            m_LastReceived.Restart();
            m_SentPingPacket = false;

            var packets = new List<Packet>();

            var (initialReadStreamPosition, bytesReadFromSocket) = ReadNetworkStream();
            Debug.Assert(bytesReadFromSocket > 0);

            var reader = new BEBinaryReader(m_ReadStream);

            while ((m_ReadStream.Position - initialReadStreamPosition) < bytesReadFromSocket)
            {
                var markStartPosition = (int)m_ReadStream.Position;
                var packetRelativeStart = (m_ReadStream.Position - initialReadStreamPosition);

                // Make sure that we have enough to read the packet header
                if (bytesReadFromSocket - packetRelativeStart < PACKET_HEADER_SIZE)
                {
                    var (_, readFromSocket) = ReadNetworkStream();
                    Debug.Assert(readFromSocket > 0);

                    bytesReadFromSocket += readFromSocket;
                }

                var packetSize = reader.ReadUInt16();

                // Make sure that we have enough data of the newly arrived packet
                if (packetSize + PACKET_HEADER_SIZE > (bytesReadFromSocket - packetRelativeStart))
                {
                    var bytesNeeded = (packetSize + PACKET_HEADER_SIZE - sizeof(ushort)) - (bytesReadFromSocket - (m_ReadStream.Position - markStartPosition));
                    var (_, readFromSocket) = ReadNetworkStream((int)bytesNeeded);
                    Debug.Assert(readFromSocket >= bytesNeeded);

                    bytesReadFromSocket += readFromSocket;

                    Debug.Assert(packetSize + PACKET_HEADER_SIZE <= (bytesReadFromSocket - packetRelativeStart));
                }

                var packetCounter = reader.ReadUInt16();
                var packetId = reader.ReadUInt32();
                var packet = Packet.CreateFrom(packetId, packetSize, packetCounter);
                if (packet == null)
                {
                    Log.WarnFormat("Received Unknown Packet ({0}): {1:X8}", packetSize, packetId);
                    var packetData = reader.ReadBytes(packetSize);
                    Packet.Hexdump(packetData);
                    continue;
                }

                Log.DebugFormat("Received {0}", packet.GetType().Name);

                try
                {
                    packet.Deserialize(reader);

                    var actualBytesReaded = ((m_ReadStream.Position - initialReadStreamPosition) - packetRelativeStart);
                    if (packetSize + PACKET_HEADER_SIZE != actualBytesReaded)
                    {
                        if ((packetSize + PACKET_HEADER_SIZE) > actualBytesReaded)
                        {
                            var underread = (packetSize + PACKET_HEADER_SIZE) - actualBytesReaded;
                            m_ReadStream.Position += underread;
                            Log.ErrorFormat($"Packet {packet.GetType().Name} left {underread} bytes without reading. Expected size {packetSize + 8}; Actually Read {actualBytesReaded}");
                        }
                        else
                        {
                            var overread = actualBytesReaded - (packetSize + PACKET_HEADER_SIZE);
                            m_ReadStream.Position -= overread;
                            Log.ErrorFormat($"Packet {packet.GetType().Name} readed {overread} bytes from another packet. Expected size {packetSize + 8}; Actually Read {actualBytesReaded}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Fatal($"Unable to deserialize packet {packet.GetType().Name}", e);

                    // Skip the entire packet
                    m_ReadStream.Position = markStartPosition + packetSize + PACKET_HEADER_SIZE;

                    continue;
                }

                packets.Add(packet);
            }

            // Make sure we have read everything!
            Debug.Assert(m_ReadStream.Position - initialReadStreamPosition == bytesReadFromSocket);

            // Since we are not waiting for more data, reset the position
            m_ReadStream.Position = 0;
            return packets;
        }

        public void CheckPingSystem()
        {
            const long PING_TIMEOUT_MS = 30 /* seconds */ * 1000;
            if (m_LastReceived.ElapsedMilliseconds < PING_TIMEOUT_MS)
            {
                return;
            }

            const long PING_FATAL_TIMEOUT_MS = 30 * /* minutes */ 60 /* seconds */ * 1000;
            if (m_LastReceived.ElapsedMilliseconds >= PING_FATAL_TIMEOUT_MS)
            {
                Close(Constants.INACTIVITY_MESSAGE);
                Log.InfoFormat("Player {0} was kicked due to inactivity", RemoteEndPoint);
            }
            else if (!m_SentPingPacket)
            {
                SendPacket(new ReqLineCheck(), true);
                m_SentPingPacket = true;
            }
        }

        public bool CouldWrite()
        {
            const long MINIMUM_TIME_BETWEEN_SOCKET_WRITE_MS = 100L;

            if (m_LastWrite.ElapsedMilliseconds < MINIMUM_TIME_BETWEEN_SOCKET_WRITE_MS)
            {
                return false;
            }

            if (m_WriteStreamWriter.Position > 0)
            {
                return true;
            }

            return false;
        }

        public void SendHandshake()
        {
            Debug.Assert(m_PacketCounter == 0);
            SendPacket(new ReqConnection(0));
        }

        public void SendPacket(Packet packet, bool flushImmediately = false)
        {
            var initialPosition = m_WriteStreamWriter.Position;

            packet.Serialize(m_WriteStreamWriter);

            var packetSize = m_WriteStreamWriter.Position - initialPosition - PACKET_HEADER_SIZE;

            // Rewind
            m_WriteStreamWriter.Position = initialPosition;
            m_WriteStreamWriter.Write((ushort)packetSize);

            if ((packet.ID & 0x0000ff00) == 0x200)
            {
                Debug.Assert(NextResponseCounter != 0);
                m_WriteStreamWriter.Write(NextResponseCounter);
            }
            else
            {
                m_WriteStreamWriter.Write(m_PacketCounter++);
            }

            m_WriteStreamWriter.Position += packetSize + sizeof(uint); // sizeof(PacketId)

            Log.Debug($"Queued {packet.GetType().Name}");

            if (flushImmediately)
            {
                FlushWriteBuffer();
            }
        }

        public void FlushWriteBuffer()
        {
            Debug.Assert(m_WriteStreamWriter.Position > 0);

            Log.Debug("Flush Packets Stream");

            if (!Socket.Connected)
            {
                Log.ErrorFormat("Socket is close, failed to flush packets");
                return;
            }

            var memStream = (MemoryStream)m_WriteStreamWriter.BaseStream;

            NetworkStream.Write(memStream.GetBuffer(), 0, (int)m_WriteStreamWriter.Position);
            m_WriteStreamWriter.Position = 0;
            m_LastWrite.Restart();
        }

        public void Close(string reason)
        {
            try
            {
                SendPacket(new NtcShut(1, reason), true);
            }
            catch (Exception) { /* Ignored, since we want to close the session anyway */ }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (Socket == null)
            {
                return;
            }

            Socket.Dispose();
            Socket = null;
            NetworkStream.Dispose();
            NetworkStream = null;

            m_ReadStream.Dispose();
            m_ReadStream = null;
            m_WriteStreamWriter.Dispose();
            m_WriteStreamWriter = null;

            m_LastReceived.Stop();
            m_LastReceived = null;

            m_LastWrite.Stop();
            m_LastWrite = null;

            m_Tag = null;
        }
    }
}
