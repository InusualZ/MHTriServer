using log4net;
using MHTriServer.Server.Game;
using MHTriServer.Server.Packets;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MHTriServer.Server
{
    public class FmpServer : BaseServer
    {
        private const int MAX_SERVER_TYPE = 4;

        private static readonly ILog Log = LogManager.GetLogger(nameof(FmpServer));

        private readonly PlayerManager m_PlayerManager = null;

        private ServerType[] m_ServerTypes;
        private byte[] m_ServerTypesPropertiesPayload;

        public FmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));

            m_PlayerManager = playerManager;
        }

        public override void OnStart()
        {
            Log.InfoFormat("Running on {0}:{1}", Address, Port);

            InitServerTypes();
        }

        public override void HandleNtcCollectionLog(NetworkSession session, NtcCollectionLog collectionLog)
        {
            var data = collectionLog.Data;
            Log.WarnFormat("Session {0} Error {1:X8} {2} {3}", session.RemoteEndPoint, data.ErrorCode, data.UnknownField2, data.Timeout);
        }

        public override void HandleAnsConnection(NetworkSession session, AnsConnection ansConnection)
        {
            var onlineSupportCode = PlayerManager.GetOnlineSupportCodeFrom(ansConnection.Data);
            if (!m_PlayerManager.TryGetPlayer(onlineSupportCode, out var player))
            {
                Log.FatalFormat("Kicked {0}, because it tried to connect to the server directly", session.RemoteEndPoint);
                session.Close(Constants.PLAYER_NOT_LOADED_ERROR_MESSAGE);
                return;
            }
            else
            {
                Debug.Assert(player.RequestedFmpServerAddress == true);
                Debug.Assert(player.RequestedUserList == false);

                // Make sure to update the remote endpoint so future player lookup work
                player.RequestedFmpServerAddress = false;
                player.RemoteEndPoint = session.RemoteEndPoint;
            }

            session.SetTag(player);

            if (player.SelectedServer == null)
            {
                player.SelectedServer = m_ServerTypes.SelectMany(st => st.Servers).FirstOrDefault(s => s.ServerIndex == 1);
                if (player.SelectedServer == default)
                {
                    session.Close(Constants.SERVER_NOT_FOUND_ERROR_MESSAGE);
                    return;
                }
            }

            // TODO: Figure out what login type 3/5 means.
            // Value need to be 3 or 5 in order to proceed with the login process
            session.SendPacket(new NtcLogin(ServerLoginType.FMP_NORMAL));
        }

        public override void HandleReqServerTime(NetworkSession session, ReqServerTime reqServerTime)
        {
            // TODO: Do we need the player for this?

            session.SendPacket(new AnsServerTime(1500, 0));
        }

        public override void HandleReqBinaryHead(NetworkSession session, ReqBinaryHead reqBinaryHead)
        {
            uint binaryLength = 0;
            if (reqBinaryHead.BinaryType == 5)
            {
                // Unknown request, max size is 0x1ff
                binaryLength = (uint)Player.BINARY_DATA_5_TEST.Length;
            }
            else if (reqBinaryHead.BinaryType == 2)
            {
                // Confirmed Weather Data Length
                binaryLength = 12;
            }
            else if (reqBinaryHead.BinaryType == 3)
            {
                // Confirmed Unknown Data Length
                binaryLength = 0x904;
            }
            else if (reqBinaryHead.BinaryType == 4)
            {
                // Confirmed Unknown Data Length
                binaryLength = 0xf0;
            }
            else if (reqBinaryHead.BinaryType == 1)
            {
                // Unconfirmed expected length of 0x140c
                binaryLength = (uint)m_ServerTypesPropertiesPayload.Length;
            }
            else if (reqBinaryHead.BinaryType == 6)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 7)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 8)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 9)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 10)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 11)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 12)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 13)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 14)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else if (reqBinaryHead.BinaryType == 15)
            {
                // Received when the player enter a city
                // *Don't know the real max length*
                binaryLength = 1;
            }
            else
            {
                Debug.Assert(false);
                // Unknown
            }

            session.SendPacket(new AnsBinaryHead(reqBinaryHead.BinaryType, binaryLength));
        }

        public override void HandleReqBinaryData(NetworkSession session, ReqBinaryData reqBinaryData)
        {
            byte[] binaryData = null;
            if (reqBinaryData.Type == 5)
            {
                // Unknown Data
                binaryData = Encoding.ASCII.GetBytes(Player.BINARY_DATA_5_TEST);

            }
            else if (reqBinaryData.Type == 2)
            {
                // Lobby Weather Data 
                binaryData = new byte[] {
                    0x00, 0x00, 0x01, 0xF4,
                    0x00, 0x00, 0x00, 0x01, // Fog
                    0x00, 0x00, 0x50, 0x00 // *Sandstorm*
                };
            }
            else if (reqBinaryData.Type == 3)
            {
                // ???
                binaryData = new byte[reqBinaryData.DataExpectedSize];
                var flag = 0 | 1 | 2 | 4 | 8;
                BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(binaryData, binaryData.Length - 4, 4), (uint)flag);
            }
            else if (reqBinaryData.Type == 4)
            {
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 1)
            {
                binaryData = m_ServerTypesPropertiesPayload;
            }
            else if (reqBinaryData.Type == 6)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 7)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 8)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 9)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 10)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 11)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 12)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 13)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 14)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else if (reqBinaryData.Type == 15)
            {
                // Received when the player enter a city
                binaryData = new byte[reqBinaryData.DataExpectedSize];
            }
            else
            {
                Debug.Assert(false);
            }

            session.SendPacket(new AnsBinaryData(reqBinaryData.Version, reqBinaryData.Offset, binaryData));
        }

        public override void HandleReqBinaryFoot(NetworkSession session, ReqBinaryFoot reqBinaryFoot)
        {
            session.SendPacket(new AnsBinaryFoot());
        }

        public override void HandleReqUserSearchInfoMine(NetworkSession session, ReqUserSearchInfoMine reqUserSearchInfoMine)
        {
            // The client won't even read the data that we would send, so why bother.
            session.SendPacket(new AnsUserSearchInfoMine(new CompoundList()));
        }

        public override void HandleReqLayerStart(NetworkSession session, ReqLayerStart reqLayerStart)
        {
            /*
            * Why does this packet send two formats, when the answer to this packet is only
            * one compound data struct
            * 
            *  reqLayerStart.Format
            *  reqLayerStart.Format2
            *  
            *  Also, it seems that some parts of this response are copied into the NetworkLayerPat
            *  struct, but nothing else is done with the data. No read, write or anything.
            *  Atleast up until the server selection screen.
            */

            var player = session.GetPlayer();
            var selectedServer = player.SelectedServer;

            var data = new LayerData()
            {
                UnknownField1 = 1,
                UnknownField2 = new UnkShortArrayStruct()
                {
                    UnknownField = 1,
                    UnknownField2 = 3,
                    UnknownField3 = new List<ushort>()
                    {
                        4, 5, 6
                    }
                },
                Name = selectedServer.Name,
                Index = (short)selectedServer.ServerIndex,
                CurrentPopulation = (uint)selectedServer.CurrentPopulation,
                UnknownField7 = 7,
                UnknownField8 = 8,
                MaxPopulation = (uint)selectedServer.MaxPopulation,
                UnknownField10 = 10,
                InCityPopulation = (uint)selectedServer.InCityPopulation,
                UnknownField12 = 12,
                UnknownField13 = 13,
                State = LayerData.StateEnum.Enable,
                UnknownField17 = 14,
                UnknownField18 = 1
            };

            session.SendPacket(new AnsLayerStart(data));
        }

        public override void HandleReqCircleInfoNoticeSet(NetworkSession session, ReqCircleInfoNoticeSet reqCircleInfoNoticeSet)
        {
            session.SendPacket(new AnsCircleInfoNoticeSet());
        }

        public override void HandleReqUserBinarySet(NetworkSession session, ReqUserBinarySet reqUserBinarySet)
        {
            // TODO: What am I suppose to do with the binary data?
            session.SendPacket(new AnsUserBinarySet());
        }

        public override void HandleReqUserSearchSet(NetworkSession session, ReqUserSearchSet reqUserSearchSet)
        {
            // TODO: What am I suppose to do with the binary data?
            session.SendPacket(new AnsUserSearchSet());
        }

        public override void HandleReqBinaryVersion(NetworkSession session, ReqBinaryVersion reqBinaryVersion)
        {
            // This packet means that the client is asking the server 
            // If the binary data of type {reqBinaryVersion.BinaryType} has change.

            // For now, we are going to always send a new version, because we want to know
            // every single binary requets there is...

            session.SendPacket(new AnsBinaryVersion(reqBinaryVersion.BinaryType, ++Player.BINARY_VERSION_COUNT));
        }

        public override void HandleReqFmpListVersion(NetworkSession session, ReqFmpListVersion reqFmpListVersion)
        {
            // If the Fmp List Verion sent in the LMP server, mistmatch this version
            // the client would send a request that allow us to rewrite the previous sent list
            // but, if the versions matches the previously sended version, the client would send a request to update
            // the population and max population of the server.
            session.SendPacket(new AnsFmpListVersion(2));
        }

        public override void HandleReqFmpListHead(NetworkSession session, ReqFmpListHead reqFmpListHead)
        {
            var serverCount = m_ServerTypes.Sum(st => st.Servers.Length);
            session.SendPacket(new AnsFmpListHead(0, (uint)serverCount));
        }

        public override void HandleReqFmpListData(NetworkSession session, ReqFmpListData reqFmpListData)
        {
            const uint UNKNOWN_SERVER_VALUE = 1;

            var servers = new List<FmpData>();

            for (var serverTypeIndex = 0U; serverTypeIndex < MAX_SERVER_TYPE; ++serverTypeIndex)
            {
                if (serverTypeIndex >= m_ServerTypes.Length)
                {
                    break;
                }

                var serverType = m_ServerTypes[serverTypeIndex];
                foreach (var server in serverType.Servers) 
                {
                    servers.Add(FmpData.Server(server.ServerIndex, (uint)server.CurrentPopulation, (uint)server.MaxPopulation, 
                        serverTypeIndex + 1, server.Name, UNKNOWN_SERVER_VALUE));
                }
            }

            session.SendPacket(new AnsFmpListData(servers));
        }

        public override void HandleReqFmpListFoot(NetworkSession session, ReqFmpListFoot reqFmpListFoot)
        {
            session.SendPacket(new AnsFmpListFoot());
        }

        public override void HandleReqFmpInfo(NetworkSession session, ReqFmpInfo reqFmpInfo)
        {
            // This means that the player have selected a server from the list other than the current one

            var player = session.GetPlayer();

            var serverIndex = reqFmpInfo.SelectedServerIndex;
            var selectedServer = m_ServerTypes.SelectMany(st => st.Servers).FirstOrDefault(s => s.ServerIndex == serverIndex);
            if (selectedServer == default)
            {
                session.Close(Constants.SERVER_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            player.SelectedServer = selectedServer;

            session.SendPacket(new AnsFmpInfo(FmpData.Address(MHTriServer.Config.FmpServer.Address, MHTriServer.Config.FmpServer.Port)), true);
            player.RequestedFmpServerAddress = true;
        }

        public override void HandleReqLayerChildInfo(NetworkSession session, ReqLayerChildInfo reqLayerChildInfo)
        {
            var player = session.GetPlayer();

            // This data would replace some field sent with ReqLayerStart/ReqLayerChildListData
            var layerData = new LayerData()
            {
                // if Index == 0, all the other field except UnknownField12, would be ignored by the client
                Index = reqLayerChildInfo.Index,

                // This field would be sent when the game send the packet `ReqLayerDown`
                UnknownField12 = 1,
            };

            var extraProperties = reqLayerChildInfo.UnknownField2.Select(f => {
                return new UnkByteIntStruct()
                {
                    UnknownField = f.UnknownField1,
                    ContainUnknownField3 = true,
                    UnknownField3 = f.UnknownField2
                };
            }).ToList(); ;

            if (reqLayerChildInfo.Index < 1)
            {
                // Index should not be < 1, it (probably) means that the game want to know if anything have changed
                // in the layer that we are in

                // TODO: What should we update here?
            }
            else if (player.SelectedGate == null)
            {
                // It means that we have selected a gate
                var gate = player.SelectedServer.Gates.FirstOrDefault(g => g.Id == reqLayerChildInfo.Index);
                if (gate == default)
                {
                    session.Close(Constants.GATE_NOT_FOUND_ERROR_MESSAGE);
                    return;
                }

                if (gate.CurrentPopulation + 1 > gate.MaxPopulation)
                {

                    session.Close(Constants.GATE_FULL_ERROR_MESSAGE);
                    return;
                }

                player.SelectedGate = gate;
                gate.PlayerInGate.Add(player);

                layerData.Name = gate.Name;
                layerData.Index = (short)gate.Id;
                layerData.CurrentPopulation = (uint)gate.CurrentPopulation + 1;
                layerData.MaxPopulation = (uint)gate.MaxPopulation;
                layerData.UnknownField7 = 0xff; // ???
                layerData.InCityPopulation = (uint)gate.InCityPopulation;
                layerData.State = LayerData.StateEnum.Enable;
            }
            else if (player.SelectedCity == null)
            {
                var gate = player.SelectedGate;
                var city = gate.Cities.FirstOrDefault(c => c.Id == reqLayerChildInfo.Index);
                if (city == default)
                {
                    session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                    return;
                }

                if (city.CurrentPopulation + 1 > city.MaxPopulation)
                {
                    session.Close(Constants.CITY_FULL_ERROR_MESSAGE);
                    return;
                }

                player.SelectedCity = city;
                gate.PlayerInGate.Remove(player);

                // How do I notify to the host this new player?
                player.SelectedCity.Players.Add(player);

                layerData.Name = city.Name;
                layerData.Index = (short)city.Id;
                layerData.CurrentPopulation = (uint)city.CurrentPopulation + 1;
                layerData.MaxPopulation = (uint)city.MaxPopulation;
                layerData.UnknownField7 = 0xff; // ???
                layerData.InCityPopulation = (uint)city.Players.Count - (uint)city.DepartedPlayer;
                layerData.State = LayerData.StateEnum.Enable;
            }

            session.SendPacket(new AnsLayerChildInfo(1, layerData, extraProperties));
        }

        public override void HandleNtcLayerBinary(NetworkSession session, NtcLayerBinary layerBinary)
        {
            // We don't know the actual purpose of this packet, we only know that
            // we must re-transmit it content to the other players in the gate

            var senderPlayer = session.GetPlayer();
            var gate = senderPlayer.SelectedGate;

            var unknownField2 = new NtcBinaryCompoundData()
            {
                UnknownField1 = 0,
                CapcomID = senderPlayer.SelectedHunter.SaveID,
                Name = senderPlayer.SelectedHunter.HunterName
            };

            // Should we include the player in cities?
            foreach (var playerInGate in gate.PlayerInGate.Concat(gate.PlayersInCity))
            {
                var playerSession = GetNetworkSession(playerInGate.RemoteEndPoint);
                playerSession.SendPacket(new NtcLayerBinary(senderPlayer.SelectedHunter.SaveID, unknownField2,
                    layerBinary.UnknownField3, layerBinary.UnknownField4));
            }
        }

        public override void HandleReqLayerUserList(NetworkSession session, ReqLayerUserList reqLayerUserList)
        {
            // Request user list from the current layer

            var player = session.GetPlayer();

            var currentUsers = new List<LayerUserData>();
            if (player.SelectedCity != null)
            {
                foreach (var playerInCity in player.SelectedCity.Players)
                {
                    var hunter = playerInCity.SelectedHunter;
                    currentUsers.Add(new LayerUserData()
                    {
                        CapcomID = hunter.SaveID,
                        Name = hunter.HunterName,
                        // TODO: Do others field too
                    });
                }
            }
            else if (player.SelectedGate != null)
            {
                var gate = player.SelectedGate;
                foreach (var playerInGate in gate.PlayerInGate.Concat(gate.PlayersInCity))
                {
                    var hunter = playerInGate.SelectedHunter;
                    currentUsers.Add(new LayerUserData()
                    {
                        CapcomID = hunter.SaveID,
                        Name = hunter.HunterName,
                        // TODO: Do others field too
                    });
                }
            }
            else if (player.SelectedServer != null)
            {
                // TODO: Should we really send the player that are connected to the server?
                var hunter = player.SelectedHunter;
                currentUsers.Add(new LayerUserData()
                {
                    CapcomID = hunter.SaveID,
                    Name = hunter.HunterName,
                    // TODO: Do others field too
                });
            }

            session.SendPacket(new AnsLayerUserList(currentUsers));
        }

        public override void HandleReqFriendList(NetworkSession session, ReqFriendList reqFriendList)
        {
            var friends = new List<FriendData>();
            session.SendPacket(new AnsFriendList(friends));
        }

        public override void HandleReqBlackList(NetworkSession session, ReqBlackList reqBlackList)
        {
            var blackList = new List<FriendData>();
            session.SendPacket(new AnsBlackList(blackList));
        }

        public override void HandleReqUserStatusSet(NetworkSession session, ReqUserStatusSet reqUserStatusSet)
        {
            session.SendPacket(new AnsUserStatusSet());
        }

        public override void HandleReqLayerChildListHead(NetworkSession session, ReqLayerChildListHead reqLayerChildListHead)
        {
            var player = session.GetPlayer();
            Debug.Assert(player.SelectedServer != null);

            var childElementCount = 0U;
            if (player.SelectedGate == null)
            {
                childElementCount = (uint)player.SelectedServer.Gates.Length;
            }
            else if (player.SelectedCity == null)
            {
                childElementCount = (uint)player.SelectedGate.Cities.Length;
            }

            session.SendPacket(new AnsLayerChildListHead(childElementCount));
        }

        public override void HandleReqLayerChildListData(NetworkSession session, ReqLayerChildListData reqLayerChildListData)
        {
            var player = session.GetPlayer();

            var childsData = new List<LayerChildData>();
            var selectedServer = player.SelectedServer;
            var startOffset = Math.Min(0, reqLayerChildListData.Offset - 1);
            if (player.SelectedGate == null)
            {
                for (var gateIndex = startOffset; gateIndex < selectedServer.Gates.Length; ++gateIndex)
                {
                    var gate = selectedServer.Gates[gateIndex];
                    childsData.Add(new LayerChildData()
                    {
                        ChildData = new LayerData()
                        {
                            Name = gate.Name,
                            Index = (short)gate.Id,
                            CurrentPopulation = (uint)gate.CurrentPopulation,
                            MaxPopulation = (uint)gate.MaxPopulation,
                            UnknownField7 = 0xff, // ???
                            InCityPopulation = (uint)gate.InCityPopulation,
                            State = LayerData.StateEnum.Enable,
                        }
                    });
                }
            }
            else if (player.SelectedCity == null)
            {
                var selectedGate = player.SelectedGate;
                for (var cityIndex = startOffset; cityIndex < selectedGate.Cities.Length; ++cityIndex)
                {
                    var city = selectedGate.Cities[cityIndex];
                    var state = LayerData.StateEnum.Empty;
                    if (city.Players.Count == city.MaxPopulation)
                    {
                        state = LayerData.StateEnum.Disable;
                    }
                    else if (city.Players.Count > 0)
                    {
                        state = LayerData.StateEnum.Enable;
                    }

                    childsData.Add(new LayerChildData()
                    {
                        ChildData = new LayerData()
                        {
                            Name = city.Name,
                            Index = (short)city.Id,
                            CurrentPopulation = (uint)city.CurrentPopulation,
                            MaxPopulation = city.MaxPopulation,
                            UnknownField7 = 0xff, // ???
                            InCityPopulation = (uint)(city.CurrentPopulation - city.DepartedPlayer),
                            State = state,
                        }
                    });
                }
            }

            session.SendPacket(new AnsLayerChildListData(childsData));
        }

        public override void HandleReqLayerChildListFoot(NetworkSession session, ReqLayerChildListFoot reqLayerChildListFoot)
        {
            session.SendPacket(new AnsLayerChildListFoot());
        }

        public override void HandleReqLayerDown(NetworkSession session, ReqLayerDown reqLayerDown)
        {
            session.SendPacket(new AnsLayerDown(2));
        }

        public override void HandleReqUserBinaryNotice(NetworkSession session, ReqUserBinaryNotice reqUserBinaryNotice)
        {
            var player = session.GetPlayer();   
            session.SendPacket(new AnsUserBinaryNotice());
        }

        public override void HandleReqLayerCreateHead(NetworkSession session, ReqLayerCreateHead reqLayerCreateHead)
        {
            // Notify the server that the client want to create a city
            var player = session.GetPlayer();

            player.SelectedCity = player.SelectedGate.Cities.FirstOrDefault(c => c.Id == reqLayerCreateHead.CityIndex);
            if (player.SelectedCity == default)
            {
                // I could not find a way of cancelling the transaction
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            player.SelectedCity.Leader = player;

            // TODO: Notify other players in the gate, that this city is disabled for the time being
            session.SendPacket(new AnsLayerCreateHead(reqLayerCreateHead.CityIndex));
        }

        public override void HandleReqLayerCreateSet(NetworkSession session, ReqLayerCreateSet reqLayerCreateSet)
        {
            var player = session.GetPlayer();
            var city = player.SelectedCity;
            if (city == null || city.Id != reqLayerCreateSet.CityIndex)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            city.Players.Add(player);

            session.SendPacket(new AnsLayerCreateSet(reqLayerCreateSet.CityIndex));
        }

        public override void HandleReqLayerCreateFoot(NetworkSession session, ReqLayerCreateFoot reqLayerCreateFoot)
        {
            var player = session.GetPlayer();
            var city = player.SelectedCity;

            if (city == null || city.Id != reqLayerCreateFoot.CityIndex)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            if (reqLayerCreateFoot.Cancelled)
            {
                city.Leader = null;
                player.SelectedCity = null;
            }
            else
            {
                player.SelectedGate.PlayerInGate.Remove(player);
            }
            
            session.SendPacket(new AnsLayerCreateFoot(reqLayerCreateFoot.CityIndex));
        }

        public override void HandleReqLayerUserListHead(NetworkSession session, ReqLayerUserListHead reqLayerUserListHead)
        {
            const int CITY_INDEX_INDEX = 2;
            // Request list of hunter in a city
            var player = session.GetPlayer();
            var gate = player.SelectedGate;
            var cityData = reqLayerUserListHead.CityData;
            if (cityData.UnknownField3.Count < 3)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            var cityIndex = cityData.UnknownField3[CITY_INDEX_INDEX];
            player.SelectedCity = gate.Cities.FirstOrDefault(c => c.Id == cityIndex);
            if (player.SelectedCity == null)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            session.SendPacket(new AnsLayerUserListHead(reqLayerUserListHead.Offset, (uint)player.SelectedCity.Players.Count));
        }

        public override void HandleReqLayerUserListData(NetworkSession session, ReqLayerUserListData reqLayerUserListData)
        {
            // Get the user list in the city

            var player = session.GetPlayer();
            var city = player.SelectedCity;
            if (city == null)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            var childData = new List<LayerUserListData>();
            foreach (var playerInCity in city.Players)
            {
                var hunter = playerInCity.SelectedHunter;
                childData.Add(new LayerUserListData()
                {
                    ChildData = new LayerUserData()
                    {
                        CapcomID = hunter.SaveID,
                        Name = hunter.HunterName,
                        UnknownField3 = new UnkShortArrayStruct()
                        {
                            UnknownField = 1,
                            UnknownField2 = 2,
                            UnknownField3 = new List<ushort>()
                            {
                                3, 4, 5
                            }
                        }
                    },
                    UnknownField2 = new List<UnkByteIntStruct>()
                    {
                        new UnkByteIntStruct() 
                        {
                            UnknownField = 6,
                            ContainUnknownField3 = true,
                            UnknownField3 = 7
                        }
                    }
                });
            }
            session.SendPacket(new AnsLayerUserListData(childData));
        }

        public override void HandleReqLayerUserListFoot(NetworkSession session, ReqLayerUserListFoot reqLayerUserListFoot)
        {
            // End hunter list request
            var player = session.GetPlayer();
            var city = player.SelectedCity;
            if (city == null)
            {
                session.Close(Constants.CITY_NOT_FOUND_ERROR_MESSAGE);
                return;
            }

            if (!city.Players.Contains(player))
            {
                player.SelectedCity = null;
            }

            session.SendPacket(new AnsLayerUserListFoot());
        }

        public override void HandleReqLayerHost(NetworkSession session, ReqLayerHost reqLayerHost)
        {
            // Sent when you join a city

            // Should we do something with this data?
            var cityData = reqLayerHost.CityData;

            var player = session.GetPlayer();
            var selectedCity = player.SelectedCity;
            var leader = selectedCity.Leader;
            var leaderSession = GetNetworkSession(leader.RemoteEndPoint);
            var leaderHunter = leader.SelectedHunter;

            // session.SendPacket(new NtcLayerHost(cityData, leaderHunter.SaveID, leaderHunter.HunterName));

            session.SendPacket(new AnsLayerHost(cityData, leaderHunter.SaveID, leaderHunter.HunterName));
        }

        public override void HandleReqLayerMediationList(NetworkSession session, ReqLayerMediationList reqLayerMediationList)
        {
            var mediationElements = new List<MediationData>()
            {
                new MediationData()
                {
                    UnknownField1 = "Test",
                    UnknownField2 = 0x1,
                    UnknownField3 = 0x2,
                }
            };

            session.SendPacket(new AnsLayerMediationList(reqLayerMediationList.UnknownField1, mediationElements));
        }

        public override void HandleReqCircleListLayer(NetworkSession session, ReqCircleListLayer reqCircleListLayer)
        {
            // This packet sent the list of created circle
            // Only a few fields are needed. Need more RE
            var circleElements = new List<CircleListData>();
            session.SendPacket(new AnsCircleListLayer(circleElements));
        }

        public override void HandleReqLayerMediationLock(NetworkSession session, ReqLayerMediationLock reqLayerMediationLock)
        {
            // Sent by the client, when player perform certain action like sit down, or arm wrestling...
            session.SendPacket(new AnsLayerMediationLock(reqLayerMediationLock.UnknownField1));
        }

        public override void HandleReqCircleCreate(NetworkSession session, ReqCircleCreate reqCircleCreate)
        {
            var player = session.GetPlayer();

            // TODO: We should not always send NtcCircleListLayerCreate,
            //  in some situations we want to use NtcCircleListLayerChange
            reqCircleCreate.UnknownField1.UnknownField1 = 1; // *Required* Used
            reqCircleCreate.UnknownField1.UnknownField2 = "JoeA"; // Used
            reqCircleCreate.UnknownField1.UnknownField7 = 2; // Used ???
            reqCircleCreate.UnknownField1.UnknownField8 = 3; // Used ???
            reqCircleCreate.UnknownField1.UnknownField9 = 8; // Used ???
            reqCircleCreate.UnknownField1.UnknownField10 = 5; // Used ???
            reqCircleCreate.UnknownField1.UnknownField12 = 1; // *Required* Used Quest Slot Index??
            reqCircleCreate.UnknownField1.LeaderID = player.SelectedHunter.SaveID;
            reqCircleCreate.UnknownField1.UnknownField15 = 0x01; // Used, flag ???

            // We need to create the quest in the list, because of this packet the quest is shown in the quest board
            session.SendPacket(new NtcCircleListLayerCreate(1, reqCircleCreate.UnknownField1, reqCircleCreate.UnknownField2));

            // Sent by the client, when the player want to submit a quest
            session.SendPacket(new AnsCircleCreate(1));
        }

        public override void HandleReqCircleMatchOptionSet(NetworkSession session, ReqCircleMatchOptionSet reqCircleMatchOptionSet)
        {
            // Sent by the client when the player succesfully submit a quest. 
            // This is to propagate the quest options
            session.SendPacket(new AnsCircleMatchOptionSet());
            // SendPacket(new NtcCircleMatchOptionSet(reqCircleMatchOptionSet.MatchOptions));
        }

        public override void HandleReqCircleInfo(NetworkSession session, ReqCircleInfo reqCircleInfo)
        {
            var circleData = new CircleData()
            {
                UnknownField1 = 1,
                UnknownField2 = "JoeA", // *Used* 0x424
                UnknownField7 = 1, // *Used* 0x528
                UnknownField8 = 3, // *Used* 0x530
                UnknownField9 = 1, // *Used* 0x524
                UnknownField10 = 5, // *Used* 0x52c
                UnknownField11 = 1, // *Used* 0x420
                UnknownField12 = 1,
            };

            var unknownData = new List<UnkByteIntStruct>() 
            {
                new UnkByteIntStruct() 
                {
                    UnknownField = 1,
                    ContainUnknownField3 = true,
                    UnknownField3 = 4
                }
            };

            session.SendPacket(new AnsCircleInfo(reqCircleInfo.UnknownField1, circleData, unknownData));
        }

        public override void HandleReqCircleLeave(NetworkSession session, ReqCircleLeave reqCircleLeave)
        {
            // Received when the player quit a quest group
            session.SendPacket(new AnsCircleLeave(reqCircleLeave.UnknownField1));
        }

        public override void HandleReqCircleInfoSet(NetworkSession session, ReqCircleInfoSet reqCircleInfoSet)
        {
            // Received when the player start a quest
            session.SendPacket(new AnsCircleInfoSet(reqCircleInfoSet.CircleIndex));
            // SendPacket(new NtcCircleInfoSet(reqCircleInfoSet.CircleIndex, reqCircleInfoSet.UnknownField1, reqCircleInfoSet.UnknownField2));
        }

        public override void HandleReqCircleMatchStart(NetworkSession session, ReqCircleMatchStart reqCircleMatchStart)
        {
            var player = session.GetPlayer();

            session.SendPacket(new AnsCircleMatchStart());

            var hunters = new List<NtcCircleMatchStart.HunterData>()
            {
                new NtcCircleMatchStart.HunterData()
                {
                    UnknownField1 = 1,
                    UnknownField2 = player.SelectedHunter.SaveID,
                    UnknownField3 = 0xff,
                    UnknownField4 = 2
                }
            };
            session.SendPacket(new NtcCircleMatchStart(hunters, 3));
        }

        public override void HandleReqCircleMatchEnd(NetworkSession session, ReqCircleMatchEnd reqCircleMatchEnd)
        {
            // Sent when the player finish/abandon a quest

            session.SendPacket(new AnsCircleMatchEnd());
        }

        public override void HandleNtcLayerChat(NetworkSession session, NtcLayerChat layerChat)
        {
            var player = session.GetPlayer();

            var messageProperties = layerChat.Properties;
            messageProperties.Color = 0xffffffff /* White */;
            messageProperties.SenderID = player.SelectedHunter.SaveID;
            messageProperties.SenderName = player.SelectedHunter.HunterName;

            if (player.SelectedCity != null)
            {
                foreach (var cityPlayer in player.SelectedCity.Players)
                {
                    if (player == cityPlayer)
                    {
                        continue;
                    }

                    var seesion = GetNetworkSession(cityPlayer.RemoteEndPoint);
                    seesion.SendPacket(new NtcLayerChat(layerChat.UnknownField1, messageProperties, layerChat.Message));
                }
            }
            else if (player.SelectedGate != null)
            {
                foreach (var gatePlayer in player.SelectedGate.PlayerInGate)
                {
                    if (player == gatePlayer)
                    {
                        continue;
                    }

                    var seesion = GetNetworkSession(gatePlayer.RemoteEndPoint);
                    seesion.SendPacket(new NtcLayerChat(layerChat.UnknownField1, messageProperties, layerChat.Message));
                }
            }
        }

        public override void HandleReqLayerEnd(NetworkSession session, ReqLayerEnd reqLayerEnd)
        {
            var player = session.GetPlayer();
            if (!player.RequestedFmpServerAddress)
            {
                // This means that it's leaving the current selected server
                player.SelectedServer = null;
            }

            if (!RemoveFromCity(player))
            {
                RemoveFromGate(player);
            }

            session.SendPacket(new AnsLayerEnd());
        }

        public override void HandleReqShut(NetworkSession session, ReqShut reqShut)
        {
            session.SendPacket(new AnsShut(0), true);
            RemoveSession(session);
        }

        public override void OnSessionRemoved(NetworkSession session)
        {
            // We can't use the session's tag, because at this point it may had been nulled

            if (!m_PlayerManager.TryGetPlayer(session.RemoteEndPoint, out var player))
            {
                return;
            }

            RemoveFromCity(player);
            RemoveFromGate(player);

            if (player.RequestedFmpServerAddress)
            {
                // This means that it's changing fmp server
                return;
            }

            player.SelectedServer = null;
            m_PlayerManager.UnloadPlayer(player);
        }

        private bool RemoveFromGate(Player player)
        {
            if (player.SelectedGate == null)
            {
                return false;
            }

            // TODO: Notify other user of this user departure
            player.SelectedGate.PlayerInGate.Remove(player);
            player.SelectedGate = null;

            return true;
        }

        private bool RemoveFromCity(Player player)
        {
            if (player.SelectedCity == null)
            {
                return false;
            }

            // TODO: Notify other user of this user departure
            player.SelectedCity.Players.Remove(player);
            player.SelectedCity = null;

            return true;
        }

        private void InitServerTypes()
        {
            InitServerPropertiesPayload();

            var gameConfig = MHTriServer.Config.Game;

            m_ServerTypes = gameConfig.ServerTypes;
            var serverIndex = 0U;
            foreach (var serverType in m_ServerTypes)
            {
                serverType.Init(ref serverIndex);

                Log.Info($"ServerType {serverType.Name} initialized, with a capacity {serverType.MaxPopulation} for hunters");

            }
        }

        private void InitServerPropertiesPayload()
        {
            const int BIG_BINARY_BLOB_SIZE = 0x140c;

            var gameConfig = MHTriServer.Config.Game;
            if (gameConfig.ServerTypes.Length > MAX_SERVER_TYPE) {
                Log.Warn($"Max ServerType allowed is {MAX_SERVER_TYPE}");
            }

            var asciiEncoder = Encoding.ASCII;
            using var serializer = new BEBinaryWriter(new MemoryStream(BIG_BINARY_BLOB_SIZE));

            // Max Length including null char
            void WriteCString(string value, int maxLength)
            {
                var bytes = asciiEncoder.GetBytes(value);
                var maxWritten = Math.Min(bytes.Length, maxLength);
                serializer.Write(bytes, 0, maxWritten);
                serializer.Position += Math.Max(0, maxLength - maxWritten) - 1; // Skip remaining
                serializer.Write('\0'); // Write Null Character
            }

            const int SERVER_TYPE_NAME_SIZE = 24;
            const int SERVER_TYPE_DESC_SIZE = 168;
            for (var index = 0; index < MAX_SERVER_TYPE; ++index)
            {
                if (index < gameConfig.ServerTypes.Length)
                {
                    var serverType = gameConfig.ServerTypes[index];
                    WriteCString(serverType.Name, SERVER_TYPE_NAME_SIZE);
                    WriteCString(serverType.Description, SERVER_TYPE_DESC_SIZE);
                }
                else 
                {
                    WriteCString(string.Empty, SERVER_TYPE_NAME_SIZE);
                    WriteCString(string.Empty, SERVER_TYPE_DESC_SIZE);
                }
            }

            // Unknown Field
            serializer.WriteUInt32(0);

            // Confirmed! It control how long last the day/night cycle
            // Need more RE in order to know how exactly it work
            // 2 uint array
            // Offset: 0x304
            // Used: @8042d91c
            serializer.WriteUInt32(0); // Current Time Tick
            serializer.WriteUInt32(0); // Max Tick Per Cycle, if 0 game default to 1500


            // Seeking City Strings
            const int SEEKING_LIST_COUNT = 32; // Confirmed max amount
            const int SEEKING_DESC_MAX_LENGTH = 48;
            for (var i = 0; i < SEEKING_LIST_COUNT; ++i)
            {
                if (i < gameConfig.Seekings.Length)
                {
                    var seeking = gameConfig.Seekings[i];
                    WriteCString(seeking.Name, SEEKING_DESC_MAX_LENGTH);
                    serializer.Write((byte)0x00); // Unknown
                    serializer.Write(seeking.Enabled);
                    serializer.Write(seeking.Flag);
                    serializer.Write((byte)0x00); // Unknown
                }
                else
                {
                    WriteCString(string.Empty, SEEKING_DESC_MAX_LENGTH);
                    serializer.Write((byte)0x00); // Unknown
                    serializer.Write(false);
                    serializer.Write((byte)0x00);
                    serializer.Write((byte)0x00); // Unknown
                }
            }

            const int HR_LIMIT_COUNT = 8;
            const int HR_LIMIT_NAME_MAX_LENGTH = 30;
            for (int i = 0; i < HR_LIMIT_COUNT; ++i)
            {
                if (i < gameConfig.HRLimits.Length) 
                {
                    var hrLimit = gameConfig.HRLimits[i];
                    WriteCString(hrLimit.Name, HR_LIMIT_NAME_MAX_LENGTH);
                    serializer.Write(hrLimit.Enabled);
                    serializer.Write((byte)0x00); // Unused
                    serializer.Write(hrLimit.Minimum);
                    serializer.Write(hrLimit.Maximum);
                }
                else
                {
                    WriteCString(string.Empty, HR_LIMIT_NAME_MAX_LENGTH);
                    serializer.Write(false);
                    serializer.Write((byte)0x00); // Unused
                    serializer.Write(0);
                    serializer.Write(0);
                }
            }

            const int GOAL_COUNT = 64;
            const int GOAL_NAME_MAX_LENGTH = 30;
            for(int i = 0; i < GOAL_COUNT; i++)
            {
                if (i < GOAL_COUNT)
                {
                    var goal = gameConfig.Goals[i];
                    WriteCString(goal.Name, GOAL_NAME_MAX_LENGTH);
                    serializer.Write(goal.Enabled);
                    serializer.Write(goal.RestrictionMode);
                    serializer.Write(goal.Minimum);
                    serializer.Write(goal.Maximum);
                }
                else
                {
                    WriteCString(string.Empty, GOAL_NAME_MAX_LENGTH);
                    serializer.Write(false);
                    serializer.Write((byte)0x00);
                    serializer.Write(0);
                    serializer.Write(0);
                }
            }

            for (int i = 0; i < MAX_SERVER_TYPE; ++i)
            {
                if (i < gameConfig.ServerTypes.Length)
                {
                    var serverType = gameConfig.ServerTypes[i];
                    serializer.Write(serverType.MinimumRank);
                    serializer.Write(serverType.MaximumRank);
                }
                else
                {
                    serializer.Write((ushort)0);
                    serializer.Write((ushort)0);
                }
            }

            // Timeout related short
            serializer.Write(gameConfig.Timeout1);
            serializer.Write(gameConfig.Timeout2);
            serializer.Write(gameConfig.Timeout3);
            serializer.Write(gameConfig.Timeout4);
            serializer.Write(gameConfig.Timeout5);
            serializer.Write(gameConfig.Timeout6);
            serializer.Write(gameConfig.Timeout7);
            serializer.Write(gameConfig.Timeout8);

            // TODO: Figure out the purpose of the 64 missing byte

            m_ServerTypesPropertiesPayload = (serializer.BaseStream as MemoryStream).GetBuffer();
        }
    }
}
