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

        // TODO: Replace with a proper db to store this token
        public static string PlayerToken;

        private Stream m_NetworkStream;
        private Socket m_Socket;

        private byte[] m_ReadBuffer = new byte[0x2000];
        private int m_ReadingPosition = 0;

        private readonly Stopwatch m_LastSent;
        private ExtendedBinaryWriter m_SendStream = new ExtendedBinaryWriter(new MemoryStream(0x4000), Endianness.Big);

        private bool AfterFirstConnection = false;
        private ushort m_PacketCounter = 0;
        private ushort m_LastCounter = 0;
        private bool m_ConnectionRequest = false;

        public EndPoint RemoteEndPoint => m_Socket.RemoteEndPoint;

        public ConnectionType ConnectionType { get; private set; }

        public bool ConnectionAccepted { get; private set; }

        /*
         * TEMP VARIABLES
         */

        private bool AfterLayerChildData = false;

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
                    break;
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

                var packetCounter = m_LastCounter = reader.ReadUInt16();
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

            // Make sure we are reading everything!
            Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
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
                            SendPacket(new NtcLogin(ServerLoginType.OPN_SERVER_ANOUNCE));
                        }
                        else if (ConnectionType == ConnectionType.LMP)
                        {
                            // TODO: Figure out what login type 1 means?
                            // In this case if we don't send 1, the client just kick us
                            SendPacket(new NtcLogin(!AfterFirstConnection ? ServerLoginType.LMP_NORMAL_FIRST : ServerLoginType.LMP_NORMAL_SECOND));
                        }
                        else if (ConnectionType == ConnectionType.FMP)
                        {
                            // TODO: Figure out what login type 3/5 means.
                            // Value need to be 3 or 5 in order to proceed with the login process
                            SendPacket(new NtcLogin(ServerLoginType.FMP_NORMAL));
                        }
                    }
                    break;

                case ReqAuthenticationToken reqAuthenticationToken:
                    {
                        // TODO: Should probably store the token
                        PlayerToken = reqAuthenticationToken.Token;
                        SendPacket(new AnsAuthenticationToken());
                    }
                    break;

                case ReqMaintenance _:
                    {
                        SendPacket(new AnsMaintenance(Constants.MAINTENANCE_MESSAGE));
                    }
                    break;

                case ReqTermsVersion _:
                    {
                        SendPacket(new AnsTermsVersion(Constants.TERMS_AND_CONDITIONS_VERSION, (uint)Constants.TERMS_AND_CONDITIONS.Length));
                    }
                    break;

                case ReqTerms reqTerms:
                    {
                        SendPacket(new AnsTerms(reqTerms.TermsCurrentLength, (uint)Constants.TERMS_AND_CONDITIONS.Length, Constants.TERMS_AND_CONDITIONS));
                    }
                    break;

                case ReqMediaVersionInfo _:
                    {
                        Debug.Assert(ConnectionType == ConnectionType.OPN);

                        SendPacket(new AnsMediaVersionInfo("V1.0.0", "Alpha", "Hello World1"));

                        // It seems that there is a bug or something in their network loop.
                        // I can bypass a lot of thing by sending LmpConnect next

                        // SendPacket(new LmpConnect("127.0.0.1", LmpServer.DefaultPort));
                    }
                    break;

                case ReqAnnounce _:
                    {
                        SendPacket(new AnsAnnounce(Constants.ANNOUNCEMENT_MESSAGE));
                    }
                    break;

                case ReqNoCharge _:
                    {
                        SendPacket(new AnsNoCharge("hello Wordl2"));
                    }
                    break;

                case ReqVulgarityInfoLow reqVulgarityInfoLow:
                    {
                        const string message = "HelloWorld3";
                        SendPacket(new AnsVulgarityInfoLow(1, reqVulgarityInfoLow.UnknownField, (uint)message.Length));
                    }
                    break;

                case ReqVulgarityLow reqVulgarityLow:
                    {
                        const string message = "HelloWorld3";
                        SendPacket(new AnsVulgarityLow(reqVulgarityLow.InfoType, reqVulgarityLow.CurrentLength, (uint)message.Length, message));
                    }
                    break;

                case ReqCommonKey reqCommonKey:
                    {
                        // This probably have to do with encryption
                        // TODO: Extract the encryption/decryption method that way, we can send the packet.
                        // SendPacket(new AnsCommonKey(""));
                        SendPacket(new AnsAuthenticationToken());
                    }
                    break;

                case ReqLmpConnect _:
                    {
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
                        SendPacket(new AnsTicketClient(PlayerToken));
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
                            SendPacket(new AnsFmpListVersion(2));
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
                            SendPacket(new AnsFmpListHead(0, 1));
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
                            var servers = new List<FmpData>() {
                                FmpData.Simple(1, 0, 4, 4, "Valor1", 0)
                            };

                            SendPacket(new AnsFmpListData(servers));
                        }
                    }
                    break;
                case ReqFmpListFoot _:
                    {
                        SendPacket(new AnsFmpListFoot());
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
                        else if (reqBinaryHead.BinaryType == 2)
                        {
                            binaryLength = 12;
                        }
                        else if (reqBinaryHead. BinaryType == 3)
                        {
                            binaryLength = 4;
                        }
                        else if (reqBinaryHead.BinaryType == 4)
                        {
                            binaryLength = 4;
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
                            // Unknown Data
                            binaryData = new byte[] { 0x09, 0x09, 0, 0, 0, 0, 0, 0, 0, 0 };

                        }
                        else if (reqBinaryData.BinaryType == 2)
                        {
                            // Lobby Weather Data *Sandstorm*
                            binaryData = new byte[] { 
                                0x00, 0x00, 0x01, 0xF4, 
                                0x00, 0x00, 0x00, 0x01, 
                                0x00, 0x00, 0x50, 0x00
                            };
                        }
                        else if (reqBinaryData.BinaryType == 3)
                        {
                            // ???
                            binaryData = new byte[] { 0x09, 0x09, 0, 0 };
                        }
                        else if (reqBinaryData.BinaryType == 4)
                        {
                            binaryData = new byte[] { 0x09, 0x09, 0, 0 };
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

                case ReqLayerStart reqLayerStart:
                    {
                        var data = new LayerData()
                        {
                            Name = "Joe",
                            UnknownField5 = (ushort)"Joe".Length,
                            CurrentPopulation = 0,
                            UnknownField7 = 100,
                            UnknownField10 = 0,
                            UnknownField11 = 0,
                            UnknownField17 = 0,
                            UnknownField18 = true
                        };
                        SendPacket(new AnsLayerStart(data));
                    }
                    break;

                case ReqCircleInfoNoticeSet _:
                    {
                        SendPacket(new AnsCircleInfoNoticeSet());
                    }
                    break;

                case ReqUserBinarySet reqUserBinarySet:
                    {
                        // TODO: What am I suppose to do with the binary data?
                        SendPacket(new AnsUserBinarySet());
                    }
                    break;

                case ReqUserSearchSet reqUserSearchSet:
                    {
                        // TODO: What am I suppose to do with the binary data?
                        SendPacket(new AnsUserSearchSet());
                    }
                    break;

                case ReqBinaryVersion reqBinaryVersion:
                    {
                        // The client doesn't seems to care about what we send in the parameter
                        if (reqBinaryVersion.UnknownField == 1)
                        {
                            SendPacket(new AnsBinaryVersion(0, 0));
                        }
                        else
                        {
                            SendPacket(new AnsBinaryVersion(1,1));
                        }
                    }
                    break;

                case ReqFriendList reqFriendList:
                    {
                        var friends = new List<FriendData>();
                        SendPacket(new AnsFriendList(friends));
                    }
                    break;

                case ReqBlackList reqBlackList:
                    {
                        var blackList = new List<FriendData>();
                        SendPacket(new AnsBlackList(blackList));
                    }
                    break;

                case ReqLayerChildInfo reqLayerChildInfo:
                    {
                        if (AfterLayerChildData)
                        {
                            var userNumData = new UserNumData()
                            {
                                // UnknownField - Don't know what to send
                                UnknownField2 = 1,
                                UnknownField3 = 2,
                                UnknownField4 = 3,
                                UnknownField5 = 4,
                                UnknownField6 = 5,
                                UnknownField7 = 6,
                            };

                            // TODO: Figure out what this packet doe

                            SendPacket(new NtcLayerUserNum(1, userNumData));

                            var data = new LayerData()
                            {
                                Name = "Joe",
                                UnknownField5 = 2,
                                CurrentPopulation = 1,
                                UnknownField7 = 100,
                                UnknownField10 = 3,
                                UnknownField11 = 2,
                                UnknownField12 = 1,
                                UnknownField17 = 4,
                                UnknownField18 = false
                            };

                            var unkData = new List<UnkByteIntStruct>() {
                                new UnkByteIntStruct() {
                                    UnknownField = 7,
                                    ContainUnknownField3 = true,
                                    UnknownField3 = 8
                                }
                            };

                            SendPacket(new AnsLayerChildInfo(1, data, unkData));
                        }
                        else
                        {

                            var data = new LayerData()
                            {
                                Name = "Joe",
                                UnknownField5 = 0,
                                CurrentPopulation = 1,
                                UnknownField7 = 100,
                                UnknownField10 = 3,
                                UnknownField11 = 2,
                                UnknownField12 = 1,
                                UnknownField17 = 4,
                                UnknownField18 = true
                            };

                            var unkData = new List<UnkByteIntStruct>() {
                                new UnkByteIntStruct() {
                                    UnknownField = 7,
                                    ContainUnknownField3 = true,
                                    UnknownField3 = 8
                                }
                            };

                            SendPacket(new AnsLayerChildInfo(1, data, unkData));
                        }

                    }
                    break;

                case ReqLayerChildListHead reqChildListHead:
                    {
                        SendPacket(new AnsLayerChildListHead(1));
                    }
                    break;

                case ReqLayerChildListData reqLayerChildListData:
                    {
                        var childsData = new List<LayerChildData>();
                        childsData.Add(new LayerChildData() { 
                            ChildData = new LayerData()
                            {
                                Name = "Valor2",
                                UnknownField5 = 1,
                                CurrentPopulation = 0,
                                MaxPopulation = 100,
                                UnknownField7 = 5,
                                UnknownField10 = 3,
                                UnknownField11 = 2,
                                UnknownField12 = 1,
                                UnknownField16 = 2, // Value must be 1 or 2. Seems to be a Enum of some kind
                                UnknownField17 = 4,
                                UnknownField18 = true
                            },
                            UnknownField2 = new List<UnkByteIntStruct>() { 
                                new UnkByteIntStruct() {
                                    UnknownField = 5,
                                    ContainUnknownField3 = true,
                                    UnknownField3 = 6
                                }
                            }
                        });

                        AfterLayerChildData = true;

                        SendPacket(new AnsLayerChildListData(childsData));
                    }
                    break;

                case ReqLayerChildListFoot _:
                    {
                        SendPacket(new AnsLayerChildListFoot());
                    }
                    break;

                case ReqLayerDown reqLayerDown:
                    {
                        SendPacket(new AnsLayerDown(2));
                    }
                    break;

                case ReqUserStatusSet reqUserStatusSet:
                    {
                        SendPacket(new AnsUserStatusSet());
                    }
                    break;

                case ReqShut reqShut:
                    {
                        SendPacket(new AnsShut(0));
                    }
                    break;

                case ServerTimeout _:
                    m_NetworkStream.Dispose();
                    m_Socket.Dispose();
                    break;
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
            else
            {
                // Ping System
                if (m_LastSent.ElapsedMilliseconds >= 30000) 
                {
                    SendPacket(new ReqLineCheck(), true);
                }
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

            if ((packet.ID & 0x0000ff00) == 2)
            {
                m_SendStream.Write(m_LastCounter);
            }
            else
            {
                m_SendStream.Write(m_PacketCounter++);
            }
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
