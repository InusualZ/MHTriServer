using MHTriServer.Server;
using MHTriServer.Server.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MHTriServer.Player
{
    public class Player
    {
        private const string DEFAULT_USER_ID = "AAAA";

        private Stream m_NetworkStream;
        private Socket m_Socket;

        private byte[] m_ReadBuffer = new byte[0x2000];
        private int m_ReadingPosition = 0;

        private readonly Stopwatch m_LastSent;
        private ExtendedBinaryWriter m_SendStream = new ExtendedBinaryWriter(new MemoryStream(0x4000), Endianness.Big);

        private bool AfterFirstConnection = false;
        private ushort m_PacketCounter = 0;
        private bool m_ConnectionRequest = false;

        public EndPoint RemoteEndPoint => m_Socket.RemoteEndPoint;

        public ConnectionType ConnectionType { get; private set; }

        public bool ConnectionAccepted { get; private set; }

        
        internal Player(ConnectionType connectionType, Socket socket, Stream networkStream)
        {
            m_LastSent = new Stopwatch();
            SetConnection(connectionType, socket, networkStream);
        }

        /*
         * Refactor this method. The idea was to share the player between server.
         * But in reality that's not how the server were suppose to work. Since there are multiple server
         * and they can exist at any end point. So the idea that they shared the player object doesn't make sense
         * One we have a better understanding of the network protocol. Please re-do this part
         */
        public void SetConnection(ConnectionType connectionType, Socket socket, Stream networkStream)
        {
            Debug.Assert(networkStream != null);
            Debug.Assert(socket != null && socket.Connected);

            ConnectionType = connectionType;

            m_LastSent.Restart();
            m_Socket = socket;
            m_NetworkStream = networkStream;

            if (ConnectionAccepted)
            {
                AfterFirstConnection = true;
            }

            m_ConnectionRequest = false;
            ConnectionAccepted = false;
        }

        private Stream ReadAvailable()
        {
            try
            {
                var readBufferPostion = m_NetworkStream.Read(m_ReadBuffer, m_ReadingPosition, m_ReadBuffer.Length - m_ReadingPosition);
                if (readBufferPostion == 0)
                {
                    return null;
                }

                m_ReadingPosition += readBufferPostion;
                return new MemoryStream(m_ReadBuffer, 0, m_ReadingPosition);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ReadPacketFromStream()
        {
            ReadMore:
            var packetStream = ReadAvailable();
            if (packetStream == null)
            {
                // This should only happens when the client closed the connection, while we were reading data
                return;
            }

            var reader = new ExtendedBinaryReader(packetStream, Endianness.Big);
            var packetStartPosition = packetStream.Position;
            while (packetStream.Position < packetStream.Length)
            {
                if (packetStream.Length - packetStream.Position < 8)
                {
                    var notRead = m_ReadBuffer[((int)packetStream.Position)..((int)packetStream.Length)];
                    m_ReadingPosition = (int)packetStream.Position;
                    Console.Error.WriteLine($"Not enough data for a packet {Packet.Hexstring(notRead, ' ')}");
                    return;
                }

                var packetSize = reader.ReadUInt16();
                if (packetStartPosition + packetSize + 8 > packetStream.Length)
                {
                    // Wait for more data
                    if (packetStartPosition > 0)
                    {

                        return;
                    }
                    else
                    {
                        // I don't fucking understand why is this needed. Please fix!
                        goto ReadMore;
                    }
                }

                // Reset pointer, since we are going to handle the entire stream here
                m_ReadingPosition = 0;

                var packetCounter = reader.ReadUInt16();
                var packetId = reader.ReadUInt32();
                var packet = Packet.CreateFrom(packetId, packetSize, packetCounter);
                if (packet == null)
                {
                    Console.WriteLine($"Received Unknown Packet ({packetSize}): {packetId:X8}");
                    byte[] packetData = reader.ReadBytes(packetSize);
                    Packet.Hexdump(packetData);
                    continue;
                }

                try
                {
                    packet.Deserialize(reader);
                    Console.WriteLine($"Received {packet}");

                    var advancementCount = ((packetStream.Position) - packetStartPosition);
                    if (packetSize + 8 != advancementCount)
                    {
                        if ((packetSize + 8) > advancementCount)
                        {
                            var underread = (packetSize + 8) - advancementCount;
                            packetStream.Position += underread;
                            Console.Error.WriteLine($"Packet {packet.GetType().Name} left {underread} bytes without reading. Expected size {packetSize + 8}; Actually Read {advancementCount}");
                        }
                        else
                        {
                            var overread = advancementCount - (packetSize + 8);
                            packetStream.Position -= overread;
                            Console.Error.WriteLine($"Packet {packet.GetType().Name} readed {overread} bytes from another packet. Expected size {packetSize + 8}; Actually Read {advancementCount}");
                        }
                    }

                    packetStartPosition = packetStream.Position;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to deserialize packet {packet.GetType().Name}");
                    Console.WriteLine(e.ToString());
                    break;
                }

                Handle(packet);
            }
        }

        private void Handle(Packet packet)
        {
            switch (packet)
            {
                // TODO: Do something with the data
                case AnsConnection _:
                    {
                        ConnectionAccepted = true;
                        if (ConnectionType == ConnectionType.OPN)
                        {
                            // TODO: Figure out what login type 3 means?
                            SendPacket(new NtcLogin(3));
                        }
                        else if (ConnectionType == ConnectionType.LMP)
                        {
                            // TODO: Figure out what login type 1 means?
                            // In this case if we don't send 1, the client just kick us
                            SendPacket(new NtcLogin(!AfterFirstConnection ? (byte)1 : (byte)2));
                        }
                        else if (ConnectionType == ConnectionType.FMP)
                        {
                            // TODO: Figure out what login type 3/5 means.
                            // Value need to be 3 or 5 in order to proceed with the login process
                            SendPacket(new NtcLogin(3));
                        }
                    }
                    break;

                case ReqMediaVersionInfo _:
                    {
                        Debug.Assert(ConnectionType == ConnectionType.OPN);

                        // It seems that there is a bug or something, in their network loop.
                        // I can bypass a lot of thing by sending LmpConnect next

                        SendPacket(new LmpConnect("127.0.0.1", LmpServer.DefaultPort));
                    }
                    break;

                // TODO: Do something with the data
                case ReqLoginInfo _:
                    {
                        var chargeInfo = new ChargeInfo()
                        {
                            UnknownField1 = 1,
                            UnknownField2 = 2,
                            UnknownField5 = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x31 }, // "Hello World1"
                            OnlineSupportCode = "NoSupport"
                        };

                        // TODO: Figure out what login info byte 1 means?
                        // Depending on the state of this argument. The client would go different paths
                        // during the login process
                        const byte loginInfoByte = 0x01;

                        SendPacket(new AnsLoginInfo(loginInfoByte, "Hello World3", chargeInfo));
                    }
                    break;

                case ReqTicketClient _:
                    {
                        // TODO: Replace sent packet
                        // The only purpose of sending this packet is to advance the client's network state
                        SendPacket(new AnsUserListFoot());
                    }
                    break;

                // TODO: Do something with the data
                case ReqUserListHead _:
                    {
                        SendPacket(new AnsUserListHead(0, 6));
                    }
                    break;

                case ReqUserListData reqUserListData:
                    {
                        // TODO: Load from database,
                        var slots = new List<UserSlot>();
                        for (var i = 0; i < reqUserListData.SlotCount; ++i)
                        {
                            slots.Add(UserSlot.NoData((uint)i));
                        }
                        SendPacket(new AnsUserListData(slots));
                    }
                    break;

                case ReqServerTime _:
                    {
                        SendPacket(new AnsServerTime(1500, 0));
                    }
                    break;

                case ReqUserListFoot _:
                    {
                        SendPacket(new AnsUserListFoot());
                    }
                    break;

                case ReqUserObject reqUserObject:
                    {
                        reqUserObject.Slot.SlotIndex = 1;
                        reqUserObject.Slot.SaveID = DEFAULT_USER_ID; // Guid.NewGuid().ToString().Substring(0, 7); // TODO: Replace this token
                        SendPacket(new AnsUserObject(1, string.Empty, reqUserObject.Slot));
                    }
                    break;

                case ReqFmpListVersion _:
                    {
                        if (ConnectionType == ConnectionType.LMP)
                        {
                            SendPacket(new AnsFmpListVersion(1));
                        }
                        else if (ConnectionType == ConnectionType.FMP) 
                        {
                            // TODO: Implement me
                        }
                    }
                    break;

                case ReqFmpListHead _:
                    {
                        if (ConnectionType == ConnectionType.LMP)
                        {
                            SendPacket(new AnsFmpListHead(0, 1));
                        }
                        else
                        {
                            // TODO: Implement me
                        }
                    }
                    break;

                case ReqFmpListData _:
                    {
                        if (ConnectionType == ConnectionType.LMP)
                        {
                            var servers = new List<FmpData>() {
                                FmpData.Simple(1, 2, 3, 2, "Valor1", 1)
                            };
                            SendPacket(new AnsFmpListData(servers));
                        }
                        else
                        {
                            // TODO: Implement me
                        }
                    }
                    break;
                case ReqFmpListFoot _:
                    {
                        if (ConnectionType == ConnectionType.LMP)
                        {
                            SendPacket(new AnsFmpListFoot());
                        }
                        else
                        {
                            // TODO: Implement me
                        }
                    }
                    break;

                case ReqFmpInfo reqFmpInfo:
                    {
                        if (ConnectionType == ConnectionType.LMP)
                        {
                            SendPacket(new AnsFmpInfo(FmpData.Address("127.0.0.1", FmpServer.DefaultPort)));
                        }
                        else
                        {

                        }
                    }
                    break;

                case ReqBinaryHead reqBinaryHead:
                    {
                        uint binaryLength = 0;
                        if (reqBinaryHead.BinaryType == 5)
                        {
                            binaryLength = 10;
                        }
                        else
                        {

                        }

                        SendPacket(new AnsBinaryHead(reqBinaryHead.BinaryType, binaryLength));
                    }
                    break;

                case ReqBinaryData reqBinaryData:
                    {
                        uint unkField2 = 0;
                        byte[] binaryData = null;
                        if (reqBinaryData.BinaryType == 5)
                        {
                            // The server doesn't seems to
                            binaryData = new byte[] { 0x09, 0x09, 0, 0, 0, 0, 0, 0, 0, 0 };

                        }
                        else
                        {

                        }

                        SendPacket(new AnsBinaryData(reqBinaryData.BinaryType, unkField2, (uint)binaryData.Length, binaryData));
                    }
                    break;

                case ReqUserSearchInfoMine _:
                    {
                        // The client won't even read the that we would send, so why bother.
                        SendPacket(new AnsUserSearchInfoMine(new CompoundList()));
                    }
                    break;

                case ReqBinaryFoot _:
                    {
                        SendPacket(new AnsBinaryFoot());
                    }
                    break;

                case ReqShut _:
                    {
                        SendPacket(new AnsShut(0));
                    }
                    break;

                case ServerTimeout _:
                    m_NetworkStream.Dispose();
                    m_Socket.Dispose();
                    return;
            }
        }

        internal void HandleWrite()
        {
            if (m_LastSent.ElapsedMilliseconds < 150)
            {
                return;
            }

            if (!m_ConnectionRequest)
            {
                SendPacket(new ReqConnection(0), true);
                m_ConnectionRequest = true;
                return;
            }

            // Flush packets
            if (m_SendStream.Position > 0)
            {
                FlushPackets();
            }
        }

        private void SendPacket(Packet packet, bool flushImmediatly = false)
        {
            var initialPosition = m_SendStream.Position;
            packet.Serialize(m_SendStream);

            var packetSize = m_SendStream.Position - initialPosition - 8;

            // Rewind
            m_SendStream.Position = initialPosition;
            m_SendStream.Write((ushort)packetSize);
            m_SendStream.Write(m_PacketCounter++);
            m_SendStream.Position += packetSize + 4;

            if (flushImmediatly)
            {
                FlushPackets();
            }

            Console.WriteLine($"Sent {packet.GetType().Name}");
        }

        private void FlushPackets()
        {
            // TODO: Do this in an elegant way
            var memStream = (MemoryStream)m_SendStream.BaseStream;

            m_NetworkStream.Write(memStream.GetBuffer(), 0, (int)m_SendStream.Position);
            m_SendStream.Position = 0;
            m_LastSent.Restart();
        }
    }
}
