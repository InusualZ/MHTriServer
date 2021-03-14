using MHTriServer.Server;
using MHTriServer.Server.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace MHTriServer.Player
{
    public class Player
    {
        private byte[] readBuffer = new byte[0x4000];

        private int waitingPosition = 0;

        public const bool FilledSlot = false; // TODO: Remove

        private readonly Stopwatch m_LastSent;
        private Stream m_NetworkStream;
        private Socket m_Socket;
        private bool AfterFirstConnection = false;
        private ushort m_PacketCounter = 0;
        private bool m_ConnectionRequest = false;

        private UserSlot SentSlot = null; // TODO: Remove
        private bool SendLastPacket = false;

        public ushort NetworkState { get; private set; } = 0;

        public ConnectionType ConnectionType { get; private set; }

        public EndPoint RemoteEndPoint => m_Socket.RemoteEndPoint;

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
            m_ConnectionRequest = false;
            m_LastSent.Restart();
            m_Socket = socket;
            m_NetworkStream = networkStream;

            if (NetworkState > 0x00)
            {
                AfterFirstConnection = true;
            }

            NetworkState = 0x00;
        }

        private Stream ReadAvailable()
        {
            try
            {
                var readBufferPostion = m_NetworkStream.Read(readBuffer, waitingPosition, readBuffer.Length - waitingPosition);
                if (readBufferPostion == 0)
                {
                    return null;
                }

                waitingPosition += readBufferPostion;
                return new MemoryStream(readBuffer, 0, waitingPosition);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public void HandlePacket()
        {
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
                    var notRead = readBuffer[((int)packetStream.Position)..((int)packetStream.Length)];
                    Console.Error.WriteLine($"Not enough data for a packet {Packet.Hexstring(notRead, ' ')}");
                    break;
                }

                var packetSize = reader.ReadUInt16();
                if (packetSize + 8 > packetStream.Length)
                {
                    // Wait for more data
                    return;
                }
                else
                {
                    waitingPosition = 0;
                }

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

                switch (packet)
                {
                    case ServerTimeout _:
                        Console.WriteLine("Server Timeout");
                        m_NetworkStream.Dispose();
                        m_Socket.Dispose();
                        return;

                    // TODO: Probably should do something with the data
                    case AnsConnection ansConnection:
                        m_ConnectionRequest = true;
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserListHead _:
                        NetworkState += 5;
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserListData _:
                        NetworkState += 5;
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserObject ansUserSelectedSlot:
                        {
                            SentSlot = ansUserSelectedSlot.Slot;
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqFmpListHead _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqFmpListData _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqFmpInfo _:
                        {
                            if (ConnectionType == ConnectionType.LMP)
                            {
                                NetworkState = 35;
                            }
                            else
                            {
                                NetworkState += 5;
                            }
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqBinaryHead _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqBinaryData _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserSearchInfoMine _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerStart _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserBinarySet _:
                        {
                            SendPacket(new AnsUserBinarySet());
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserSearchSet _:
                        {
                            SendPacket(new AnsUserSearchSet());
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqBinaryVersion _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerEnd _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerChildInfo _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqFriendList _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqBlackList _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerChildListHead _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerChildListData _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerUserList _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqLayerDown _:
                        {
                            NetworkState += 5;
                        }
                        break;

                    // TODO: Probably should do something with the data
                    case ReqUserStatusSet _:
                        {
                            SendPacket(new AnsUserStatusSet());
                        }
                        break;


                    case ReqServerTime _:
                        {
                            if (SendLastPacket)
                            {
                                SendPacket(new AnsServerTime(745, 0));
                            }
                            else
                            {
                                NetworkState += 5;
                            }
                        }
                        break;

                    case ReqLayerChildListFoot _:
                    case ReqCircleInfoNoticeSet _:
                    case ReqBinaryFoot _:
                    case ReqFmpListFoot _:
                    case ReqFmpListVersion _:
                    case ReqUserListFoot _:
                    case ReqTicketClient _:
                    case ReqShut _:
                    case ReqMediaVersionInfo _:
                    case ReqLoginInfo _:
                        NetworkState += 5;
                        break;
                }
            }
        }

        private void HandleOPNState()
        {
            if (!m_ConnectionRequest)
            {
                // I think this should never happen
                Console.Error.WriteLine($"Player expected to handle state {ConnectionType}, without requesting connection");
                return;
            }

            switch (NetworkState)
            {
                case 0:
                    {
                        // TODO: Figure out what login type 3 means?
                        SendPacket(new NtcLogin(3));
                    }
                    break;

                case 5:
                    {
                        SendPacket(new LmpConnect("127.0.0.1", LmpServer.DefaultPort));
                    }
                    break;
                case 10:
                    {
                        // To close the connection, although you can send anything and the server would close the connection
                        SendPacket(new AnsShut(0)); 
                    }
                    break;
            }
        }

        internal void HandleState()
        {
            if (m_LastSent.ElapsedMilliseconds < 200)
            {
                return;
            }

            if (!m_ConnectionRequest)
            {
                SendPacket(new ReqConnection(1));
                return;
            }

            if (ConnectionType == ConnectionType.OPN)
            {
                HandleOPNState();
            }
            else if (ConnectionType == ConnectionType.LMP)
            {
                HandleLMPState();
            }
            else if (ConnectionType == ConnectionType.FMP)
            {
                HandleFMPState();
            }
        }

        private void HandleFMPState()
        {
            switch (NetworkState)
            {
                case 0:
                    {
                        // TODO: Figure out what login type 3/5 means.
                        // Value need to be 3 or 5 in order to proceed with the login process
                        SendPacket(new NtcLogin(3));
                        NetworkState += 5;
                    }
                    break;
                case 10:
                    {
                        SendPacket(new AnsServerTime(0, 0));
                        NetworkState += 5;
                    }
                    break;

                case 20:
                    {
                        SendPacket(new AnsBinaryHead(1, (uint)"Hello World7".Length));
                        NetworkState += 5;
                    }
                    break;
                case 30:
                    {
                        var strBytes = Encoding.ASCII.GetBytes("Hello World7");
                        SendPacket(new AnsBinaryData(1, 0, (uint)strBytes.Length, strBytes));
                        NetworkState += 5;
                    }
                    break;
                case 40:
                    {
                        SendPacket(new AnsBinaryFoot());
                        NetworkState += 5;
                    }
                    break;
                case 50:
                    {
                        // This packet is really weird, I don't understand the client request
                        SendPacket(new AnsUserSearchInfoMine(new CompoundList()));
                        NetworkState += 5;
                    }
                    break;
                case 60:
                    {
                        var data = new LayerData() {
                            UnknownField1 = 1,
                            // Field 2
                            Name = "Sepalani",
                            UnknownField5 = 1,
                            CurrentPopulation = 1,
                            UnknownField7 = 1,
                            UnknownField8 = 1,
                            MaxPopulation = 1,
                            UnknownField10 = 1,
                            UnknownField11 = 1,
                            UnknownField12 = 1,
                            UnknownField13 = 1,
                            UnknownField16 = 1,
                            UnknownField17 = 1,
                            UnknownField18 = 1
                        };
                        SendPacket(new AnsLayerStart(data));
                        NetworkState += 5;
                    }
                    break;
                case 70:
                    {
                        SendPacket(new AnsCircleInfoNoticeSet());
                        NetworkState += 5;
                    }
                    break;

                case 80:
                    {
                        SendPacket(new AnsBinaryVersion(0, 1));
                        NetworkState += 5;
                    }
                    break;

                case 90:
                    {
                        SendPacket(new AnsBinaryHead(1, (uint)"Hello World8".Length));
                        NetworkState += 5;
                    }
                    break;

                case 100:
                    {
                        var strBytes = Encoding.ASCII.GetBytes("Hello World8");
                        SendPacket(new AnsBinaryData(1, 0, (uint)strBytes.Length, strBytes));
                        NetworkState += 5;
                    }
                    break;
                case 110:
                    {
                        SendPacket(new AnsBinaryFoot());
                        NetworkState += 5;
                    }
                    break;
                case 120:
                    {
                        SendPacket(new AnsFmpListVersion(1, true));
                        NetworkState += 5;
                    }
                    break;
                case 130:
                    {
                        SendPacket(new AnsFmpListHead(0, 1, true));
                        NetworkState += 5;
                    }
                    break;
                case 140:
                    {
                        var servers = new List<FmpData>() {
                            FmpData.Simple(1, 0, 100)
                        };
                        SendPacket(new AnsFmpListData(servers, true));
                        NetworkState += 5;
                    }
                    break;
                case 150:
                    {
                        SendPacket(new AnsFmpListFoot(true));
                        NetworkState += 5;
                    }
                    break;
                case 160:
                    {
                        var data = new LayerData()
                        {
                            UnknownField1 = 1,
                            // Field 2
                            Name = "Hello World8",
                            UnknownField5 = 3,
                            CurrentPopulation = 4,
                            UnknownField7 = 5,
                            UnknownField8 = 6,
                            MaxPopulation = 7,
                            UnknownField10 = 8,
                            UnknownField11 = 9,
                            UnknownField12 = 10,
                            UnknownField13 = 11,
                            UnknownField16 = 12,
                            UnknownField17 = 13,
                            UnknownField18 = 14
                        };
                        SendPacket(new AnsLayerChildInfo(1, data));
                        NetworkState += 5;
                    }
                    break;
                case 170:
                    {
                        var slot = new LayerUserData() {
                            UnknownField = "AAAA",
                            UnknownField2 = "Sepalani",
                            UnknownField6 = 1,
                            UnknownField7 = Encoding.ASCII.GetBytes("Hello World9")
                        };
                        var slots = new List<LayerUserData>() { slot };
                        SendPacket(new AnsLayerUserList(slots));
                        NetworkState += 5;
                    }
                    break;
                case 180:
                    {
                        SendPacket(new AnsBinaryVersion(0, 1));
                        NetworkState += 5;
                    }
                    break;
                case 190:
                    {
                        SendPacket(new AnsBinaryHead(1, (uint)"Hello World9".Length));
                        NetworkState += 5;
                    }
                    break;
                case 200:
                    {
                        var strBytes = Encoding.ASCII.GetBytes("Hello World9");
                        SendPacket(new AnsBinaryData(1, 0, (uint)strBytes.Length, strBytes));
                        NetworkState += 5;
                    }
                    break;
                case 210:
                    {
                        SendPacket(new AnsBinaryFoot());
                        NetworkState += 5;
                    }
                    break;
                case 220:
                    {
                        SendPacket(new AnsBinaryVersion(1, 2));
                        NetworkState += 5;
                    }
                    break;
                case 230:
                    {
                        SendPacket(new AnsBinaryHead(2, (uint)"Hello World10".Length));
                        NetworkState += 5;
                    }
                    break;
                case 240:
                    {
                        var strBytes = Encoding.ASCII.GetBytes("Hello World10");
                        SendPacket(new AnsBinaryData(1, 0, (uint)strBytes.Length, strBytes));
                        NetworkState += 5;
                    }
                    break;
                case 250:
                    {
                        SendPacket(new AnsBinaryFoot());
                        NetworkState += 5;
                    }
                    break;
                case 260:
                    {
                        SendPacket(new AnsBinaryVersion(0, 1));
                        NetworkState += 5;
                    }
                    break;
                case 270:
                    {
                        SendPacket(new AnsBinaryHead(1, (uint)"Hello World10".Length));
                        NetworkState += 5;
                    }
                    break;
                case 280:
                    {
                        var strBytes = Encoding.ASCII.GetBytes("Hello World10");
                        SendPacket(new AnsBinaryData(1, 0, (uint)strBytes.Length, strBytes));
                        NetworkState += 5;
                    }
                    break;
                case 290:
                    {
                        SendPacket(new AnsBinaryFoot());
                        NetworkState += 5;
                    }
                    break;
                case 300:
                    {
                        var friends = new List<FriendData>() { new FriendData(1, "AAAA", "Sepalani") };
                        SendPacket(new AnsFriendList(friends));
                        NetworkState += 5;
                    }
                    break;
                case 310:
                    {
                        var friends = new List<FriendData>();
                        SendPacket(new AnsBlackList(friends));
                        NetworkState += 5;
                    }
                    break;
                case 320:
                    {
                        SendPacket(new AnsLayerChildListHead(4));
                        NetworkState += 5;
                    }
                    break;
                case 330:
                    {
                        var gate1 = new LayerData()
                        {
                            UnknownField1 = 1,
                            // Field 2
                            Name = "Sepalani 1",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            UnknownField7 = 3,
                            UnknownField8 = 4,
                            MaxPopulation = 4,
                            UnknownField10 = 6,
                            UnknownField11 = 7,
                            UnknownField12 = 8,
                            UnknownField13 = 9,
                            UnknownField16 = 10,
                            UnknownField17 = 11,
                            UnknownField18 = 12
                        };

                        var gate2 = new LayerData()
                        {
                            UnknownField1 = 2,
                            // Field 2
                            Name = "Sepalani 2",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            UnknownField7 = 3,
                            UnknownField8 = 4,
                            MaxPopulation = 4,
                            UnknownField10 = 6,
                            UnknownField11 = 7,
                            UnknownField12 = 8,
                            UnknownField13 = 9,
                            UnknownField16 = 10,
                            UnknownField17 = 11,
                            UnknownField18 = 12
                        };

                        var gate3 = new LayerData()
                        {
                            UnknownField1 = 3,
                            // Field 2
                            Name = "Sepalani 3",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            UnknownField7 = 3,
                            UnknownField8 = 4,
                            MaxPopulation = 4,
                            UnknownField10 = 6,
                            UnknownField11 = 7,
                            UnknownField12 = 8,
                            UnknownField13 = 9,
                            UnknownField16 = 10,
                            UnknownField17 = 11,
                            UnknownField18 = 12
                        };

                        var gate4 = new LayerData()
                        {
                            UnknownField1 = 4,
                            // Field 2
                            Name = "Sepalani 4",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            UnknownField7 = 3,
                            UnknownField8 = 4,
                            MaxPopulation = 4,
                            UnknownField10 = 6,
                            UnknownField11 = 7,
                            UnknownField12 = 8,
                            UnknownField13 = 9,
                            UnknownField16 = 10,
                            UnknownField17 = 11,
                            UnknownField18 = 12
                        };
                        var gates = new List<LayerData>() { gate1, gate2, gate3, gate4 };
                        SendPacket(new AnsLayerChildListData(gates));
                        NetworkState += 5;
                    }
                    break;
                case 340:
                    {
                        SendPacket(new AnsLayerChildListFoot());
                        NetworkState += 5;
                    }
                    break;
                case 350:
                    {
                        var unkData = new UserNumData() { 
                            // UnknownField - Don't know what to send
                            UnknownField2 = 2,
                            UnknownField3 = 3,
                            UnknownField4 = 4,
                            UnknownField5 = 5,
                            UnknownField6 = 6,
                            UnknownField7 = 7,
                        };

                        SendPacket(new NtcLayerUserNum(1, unkData));

                        var gate1 = new LayerData()
                        {
                            UnknownField1 = 1,
                            // Field 2
                            Name = "Sepalani",
                            UnknownField5 = 1,
                            CurrentPopulation = 1,
                            UnknownField7 = 1,
                            UnknownField8 = 1,
                            MaxPopulation = 1,
                            UnknownField10 = 1,
                            UnknownField11 = 1,
                            UnknownField12 = 1,
                            UnknownField13 = 1,
                            UnknownField16 = 1,
                            UnknownField17 = 1,
                            UnknownField18 = 1
                        };
                        SendPacket(new AnsLayerChildInfo(1, gate1));
                        NetworkState += 5;
                    }
                    break;
                case 360:
                    {
                        SendPacket(new AnsLayerDown(2));
                        NetworkState += 5;
                    }
                    break;
                case 370:
                    {
                        var gate1 = new LayerData()
                        {
                            UnknownField1 = 1,
                            // Field 2
                            Name = "Sepalani",
                            UnknownField5 = 100,
                            CurrentPopulation = 100,
                            UnknownField7 = 100,
                            UnknownField8 = 100,
                            MaxPopulation = 100,
                            UnknownField10 = 100,
                            UnknownField11 = 100,
                            UnknownField12 = 100,
                            UnknownField13 = 100,
                            UnknownField16 = 100,
                            UnknownField17 = 100,
                            UnknownField18 = 100
                        };
                        SendPacket(new AnsLayerChildInfo(1, gate1));
                        NetworkState += 5;
                    }
                    break;
                case 380:
                    {
                        var slot = new LayerUserData()
                        {
                            UnknownField = "AAAA",
                            UnknownField2 = "Sepalani",
                            UnknownField6 = 100,
                            UnknownField7 = Encoding.ASCII.GetBytes("Hello World9")
                        };
                        var slots = new List<LayerUserData>() { slot };
                        SendPacket(new AnsLayerUserList(slots));
                        NetworkState += 5;

                        SendLastPacket = true;
                    }
                    break;

                default:
                    {
                        if (!SendLastPacket)
                        {
                            break;
                        }

                        if (m_LastSent.ElapsedMilliseconds >= 15000)
                        {
                            SendPacket(new ReqLineCheck());
                        }
                    }
                    break;
            }
        }

        private void HandleLMPState()
        {
            if (NetworkState == 0)
            {
                // TODO: Figure out what login type 1 means?
                // In this case if we don't send 1, the client just kick us
                SendPacket(new NtcLogin(!AfterFirstConnection ? (byte)1 : (byte)2));
            }

            if (!FilledSlot)
            {
                if (!AfterFirstConnection)
                {
                    Handle1NoSlotFilled();
                }
                else
                {
                    Handle2NotSlotFilled();
                }
            }
            else
            {
                HandleSlotFilled();
            }
        }

        private void HandleSlotFilled()
        {
            throw new NotImplementedException();
        }

        private void Handle1NoSlotFilled()
        {
            switch (NetworkState)
            {
                case 5:
                    {
                        var chargeInfo = new ChargeInfo() {
                            UnknownField1 = 1,
                            UnknownField2 = 2,
                            UnknownField5 = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x31 }, // "Hello World1"
                            OnlineSupportCode = "Hello World2"
                        };

                        // TODO: Figure out what login info byte 1 means?
                        // Depending on the state of this argument. The client would go different paths
                        // during the login process
                        const byte loginInfoByte = 0x01; 

                        SendPacket(new AnsLoginInfo(loginInfoByte, "Hello World3", chargeInfo));
                    }
                    break;
                case 10:
                    {
                        // TODO: Replace packet with a meaningful one
                        // The only purpose of this packet is to advance the client's network state
                        SendPacket(new AnsUserListFoot());
                    }
                    break;
                case 15:
                    {
                        SendPacket(new AnsUserListHead(0, 6));
                    }
                    break;
                case 20:
                    {
                        var slot = new List<UserSlot>() {
                            UserSlot.NoData(1), UserSlot.NoData(2), UserSlot.NoData(3),
                            UserSlot.NoData(4), UserSlot.NoData(5), UserSlot.NoData(6)
                        };
                        SendPacket(new AnsUserListData(slot));
                    }
                    break;
                case 25:
                    {
                        SendPacket(new AnsUserListFoot());
                    }
                    break;
                case 30:
                    {
                        SendPacket(new AnsServerTime(0, 0));
                    }
                    break;
                case 35:
                    {
                        SendPacket(new AnsShut(0x00));
                    }
                    break;
            }
        }

        private void Handle2NotSlotFilled()
        {
            switch (NetworkState)
            {
                case 5:
                    {
                        Debug.Assert(SentSlot != null);

                        SentSlot.SaveID = Guid.NewGuid().ToString().Substring(0, 7); // TODO: Replace this token
                        SentSlot.SlotIndex = 1;
                        SentSlot.UnknownField8 = "Hello World5";
                        SendPacket(new AnsUserObject(1, "Hello World6", SentSlot));
                    }
                    break;
                case 10:
                    {
                        // This is a dummy packet, this should be an AnsTicket
                        SendPacket(new AnsUserListFoot());
                        //SendPacket(new AnsTicket("Hello World7"));
                    }
                    break;
                case 15:
                    {
                        SendPacket(new AnsFmpListVersion(1));
                    }
                    break;
                case 20:
                    {
                        SendPacket(new AnsFmpListHead(0, 1));
                    }
                    break;
                case 25:
                    {
                        var servers = new List<FmpData>() {
                            FmpData.Simple(1, 2, 3, 2, "Sepalani", 1)
                        };
                        SendPacket(new AnsFmpListData(servers));
                    }
                    break;
                case 30:
                    {
                        SendPacket(new AnsFmpListFoot());
                    }
                    break;
                case 35:
                    {
                        SendPacket(new AnsFmpInfo(FmpData.Address("127.0.0.1", FmpServer.DefaultPort)));
                    }
                    break;
                case 40:
                    {
                        SendPacket(new AnsShut(2));
                    }
                    break;
            }
        }

        private void SendPacket(Packet packet)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new ExtendedBinaryWriter(memoryStream, Endianness.Big);

            packet.Serialize(writer);

            var packetSize = memoryStream.Length - 8;

            // Rewind
            memoryStream.Position = 0;
            writer.Write((ushort)packetSize);
            writer.Write(m_PacketCounter++);

            // Send bytes
            m_NetworkStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);

            Console.WriteLine($"Sent {packet.GetType().Name}");

            m_LastSent.Restart();
        }
    }
}
