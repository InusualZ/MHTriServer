using log4net;
using MHTriServer.Player;
using MHTriServer.Server.Packets;
using MHTriServer.Server.Packets.Properties;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server
{
    public class FmpServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(FmpServer));

        private readonly PlayerManager m_PlayerManager = null;

        public FmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));

            m_PlayerManager = playerManager;
        }

        public override void Start()
        {
            base.Start();

            Log.InfoFormat("Running on {0}:{1}", Address, Port);
        }

        public override void HandleAnsConnection(NetworkSession session, AnsConnection ansConnection)
        {
            // TODO: Figure out what login type 3/5 means.
            // Value need to be 3 or 5 in order to proceed with the login process
            session.SendPacket(new NtcLogin(ServerLoginType.FMP_NORMAL));
        }

        public override void HandleReqServerTime(NetworkSession session, ReqServerTime reqServerTime)
        {
            session.SendPacket(new AnsServerTime(1500, 0));
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
            session.SendPacket(new AnsFmpListHead(0, 4));
        }

        public override void HandleReqFmpListData(NetworkSession session, ReqFmpListData reqFmpListData)
        {
            var servers = new List<FmpData>() 
            {
                FmpData.Server(1, 0, 4, 1, "Valor1", 5),
                FmpData.Server(2, 0, 4, 2, "Rookies1", 6),
                FmpData.Server(3, 0, 4, 3, "Veterans1", 7),
                FmpData.Server(4, 0, 4, 4, "Greed1", 8)
            };

            session.SendPacket(new AnsFmpListData(servers));
        }

        public override void HandleReqFmpListFoot(NetworkSession session, ReqFmpListFoot reqFmpListFoot)
        {
            session.SendPacket(new AnsFmpListFoot());
        }

        public override void HandleReqFmpInfo(NetworkSession session, ReqFmpInfo reqFmpInfo)
        {
            // Make it connect to the same server, I don't know what is the purpose of this
            session.SendPacket(new AnsFmpInfo(FmpData.Address(MHTriServer.Config.FmpServer.Address, MHTriServer.Config.FmpServer.Port)));
        }

        public override void HandleReqBinaryVersion(NetworkSession session, ReqBinaryVersion reqBinaryVersion)
        {
            // This packet means that the client is asking the server 
            // If the binary data of type {reqBinaryVersion.BinaryType} has change.

            // For now, we are going to always send a new version, because we want to know
            // every single binary requets there is...

            session.SendPacket(new AnsBinaryVersion(reqBinaryVersion.BinaryType, ++Player.Player.BINARY_VERSION_COUNT));
        }

        public override void HandleReqBinaryHead(NetworkSession session, ReqBinaryHead reqBinaryHead)
        {
            uint binaryLength = 0;
            if (reqBinaryHead.BinaryType == 5)
            {
                // Unknown request, max size is 0x1ff
                binaryLength = (uint)Player.Player.BINARY_DATA_5_TEST.Length;
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
                binaryLength = (uint)Player.Player.BINARY_DATA_1.Length;
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
                binaryData = Encoding.ASCII.GetBytes(Player.Player.BINARY_DATA_5_TEST);

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
                binaryData = Player.Player.BINARY_DATA_1;
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

        public override void HandleReqUserSearchInfoMine(NetworkSession session, ReqUserSearchInfoMine reqUserSearchInfoMine)
        {
            // The client won't even read the data that we would send, so why bother.
            session.SendPacket(new AnsUserSearchInfoMine(new CompoundList()));
        }

        public override void HandleReqBinaryFoot(NetworkSession session, ReqBinaryFoot reqBinaryFoot)
        {
            session.SendPacket(new AnsBinaryFoot());
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

            var data = new LayerData()
            {
                UnknownField1 = 1,
                UnknownField2 = new UnkShortArrayStruct()
                {
                    UnknownField = 2,
                    UnknownField2 = 3,
                    UnknownField3 = new List<ushort>()
                    {
                        4, 5, 6
                    }
                },
                Name = "Joe",
                UnknownField5 = (ushort)"Joe".Length,
                CurrentPopulation = 1,
                UnknownField7 = 7,
                UnknownField8 = 8,
                MaxPopulation = 4,
                UnknownField10 = 10,
                InCityPopulation = 11,
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

        public override void HandleReqLayerChildInfo(NetworkSession session, ReqLayerChildInfo reqLayerChildInfo)
        {
            if (Player.Player.AfterLayerChildData)
            {
                {
                    var userNumData = new UserNumData()
                    {
                        UnknownField = new UnkShortArrayStruct()
                        {
                            UnknownField = 0,
                            UnknownField2 = 1,
                            UnknownField3 = new List<ushort>() 
                            {
                                2, 3, 4
                            },
                        },
                        UnknownField2 = 2,
                        UnknownField3 = 3,
                        UnknownField4 = 4,
                        UnknownField5 = 5,
                        UnknownField6 = 6,
                        UnknownField7 = 7,
                    };

                    // TODO: Figure out what this packet does
                    session.SendPacket(new NtcLayerUserNum(4, userNumData));
                }

                var data = new LayerData()
                {
                    Name = "Joe",
                    UnknownField5 = 2,
                    CurrentPopulation = 0,
                    UnknownField7 = 100,
                    UnknownField10 = 3,
                    InCityPopulation = 0,
                    UnknownField12 = 2,
                    UnknownField17 = 4,
                    UnknownField18 = 1
                };

                var unkData = new List<UnkByteIntStruct>() 
                {
                    new UnkByteIntStruct() 
                    {
                        UnknownField = 1,
                        ContainUnknownField3 = true,
                        UnknownField3 = 8
                    }
                };

                session.SendPacket(new AnsLayerChildInfo(1, data, unkData));
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
                    InCityPopulation = 2,
                    UnknownField12 = 2,
                    UnknownField17 = 4,
                    UnknownField18 = 1
                };

                var unkData = new List<UnkByteIntStruct>() 
                {
                    new UnkByteIntStruct() 
                    {
                        UnknownField = 1,
                        ContainUnknownField3 = true,
                        UnknownField3 = 8
                    }
                };

                session.SendPacket(new AnsLayerChildInfo(1, data, unkData));
            }
        }

        public override void HandleReqLayerChildListHead(NetworkSession session, ReqLayerChildListHead reqLayerChildListHead)
        {
            if (Player.Player.AfterUserBinaryNotice)
            {
                session.SendPacket(new AnsLayerChildListHead(10));
            }
            else
            {
                session.SendPacket(new AnsLayerChildListHead(1));
            }
        }

        public override void HandleReqLayerChildListData(NetworkSession session, ReqLayerChildListData reqLayerChildListData)
        {
            var childsData = new List<LayerChildData>();

            if (Player.Player.AfterUserBinaryNotice)
            {
                for (var cityIndex = 0; cityIndex < reqLayerChildListData.ExpectedDataCount; ++cityIndex)
                {
                    childsData.Add(new LayerChildData()
                    {
                        ChildData = new LayerData()
                        {
                            Name = $"City {cityIndex + 1}",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            MaxPopulation = 4,
                            UnknownField7 = 0,
                            UnknownField10 = 0,
                            InCityPopulation = 0,
                            UnknownField12 = 0,
                            State = LayerData.StateEnum.Empty,
                            UnknownField17 = 0,
                        }
                    });
                }
            }
            else
            {
                for (var gateIndex = 0; gateIndex < reqLayerChildListData.ExpectedDataCount; ++gateIndex)
                {
                    childsData.Add(new LayerChildData()
                    {
                        ChildData = new LayerData()
                        {
                            Name = $"City Gate {gateIndex + 1}",
                            UnknownField5 = 1,
                            CurrentPopulation = 0,
                            MaxPopulation = 100,
                            UnknownField7 = 5,
                            UnknownField10 = 3,
                            InCityPopulation = 0,
                            UnknownField12 = 1,
                            State = LayerData.StateEnum.Enable,
                            UnknownField17 = 4,
                            UnknownField18 = 1
                        },
                        UnknownField2 = new List<UnkByteIntStruct>() 
                        {
                            new UnkByteIntStruct() 
                            {
                                UnknownField = 5,
                                ContainUnknownField3 = true,
                                UnknownField3 = 6
                            }
                        }
                    });
                }

                Player.Player.AfterLayerChildData = true;
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

        public override void HandleReqUserStatusSet(NetworkSession session, ReqUserStatusSet reqUserStatusSet)
        {
            session.SendPacket(new AnsUserStatusSet());
        }

        public override void HandleReqUserBinaryNotice(NetworkSession session, ReqUserBinaryNotice reqUserBinaryNotice)
        {
            Player.Player.AfterUserBinaryNotice = true;
            session.SendPacket(new AnsUserBinaryNotice());
        }

        public override void HandleReqLayerCreateHead(NetworkSession session, ReqLayerCreateHead reqLayerCreateHead)
        {
            // Notify the server that the client want to create a city

            session.SendPacket(new AnsLayerCreateHead(reqLayerCreateHead.CityIndex));
        }

        public override void HandleReqLayerCreateFoot(NetworkSession session, ReqLayerCreateFoot reqLayerCreateFoot)
        {
            // Notify the server that the client want to create a city

            session.SendPacket(new AnsLayerCreateFoot(reqLayerCreateFoot.CityIndex));
        }

        public override void HandleReqLayerUserListHead(NetworkSession session, ReqLayerUserListHead reqLayerUserListHead)
        {
            // Request list of hunter in a city
            session.SendPacket(new AnsLayerUserListHead(reqLayerUserListHead.UnknownField3, 1));
        }

        public override void HandleReqLayerUserListData(NetworkSession session, ReqLayerUserListData reqLayerUserListData)
        {
            var childData = new List<LayerUserListData>();
            for (var i = 0; i < 1 /*4*/; i++)
            {
                childData.Add(new LayerUserListData()
                {
                    ChildData = new LayerUserData()
                    {
                        UnknownField = "Hello",
                        UnknownField2 = "World",
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
            session.SendPacket(new AnsLayerUserListFoot());
        }

        public override void HandleReqLayerCreateSet(NetworkSession session, ReqLayerCreateSet reqLayerCreateSet)
        {
            // Mark city as created
            session.SendPacket(new AnsLayerCreateSet(reqLayerCreateSet.CityIndex));
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
            // TODO: We should not always send NtcCircleListLayerCreate,
            //  in some situations we want to use NtcCircleListLayerChange
            reqCircleCreate.UnknownField1.UnknownField1 = 1; // *Required* Used
            reqCircleCreate.UnknownField1.UnknownField2 = "JoeA"; // Used
            reqCircleCreate.UnknownField1.UnknownField7 = 2; // Used ???
            reqCircleCreate.UnknownField1.UnknownField8 = 3; // Used ???
            reqCircleCreate.UnknownField1.UnknownField9 = 8; // Used ???
            reqCircleCreate.UnknownField1.UnknownField10 = 5; // Used ???
            reqCircleCreate.UnknownField1.UnknownField12 = 1; // *Required* Used Quest Slot Index??
            reqCircleCreate.UnknownField1.LeaderID = Player.Player.DEFAULT_USER_ID;
            reqCircleCreate.UnknownField1.UnknownField15 = 0x01; // Used, flag ???

            // We need to create the quest in the list, because of this packet the quest is shown in the quest board
            session.SendPacket(new NtcCircleListLayerCreate(1, reqCircleCreate.UnknownField1, reqCircleCreate.UnknownField2));

            // Sent by the client, when the player want to submit a quest
            session.SendPacket(new AnsCircleCreate(1));
        }

        public override void HandleReqLayerUserList(NetworkSession session, ReqLayerUserList reqLayerUserList)
        {
            var currentUsers = new List<LayerUserData>();
            currentUsers.Add(new LayerUserData()
            {
                UnknownField = Player.Player.DEFAULT_USER_ID,
            });
            session.SendPacket(new AnsLayerUserList(currentUsers));
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
            session.SendPacket(new AnsCircleMatchStart());

            var hunters = new List<NtcCircleMatchStart.HunterData>()
            {
                new NtcCircleMatchStart.HunterData()
                {
                    UnknownField1 = 1,
                    UnknownField2 = Player.Player.DEFAULT_USER_ID,
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

        public override void HandleReqLayerEnd(NetworkSession session, ReqLayerEnd reqLayerEnd)
        {
            session.SendPacket(new AnsLayerEnd());
        }


        public override void HandleReqShut(NetworkSession session, ReqShut reqShut)
        {
            session.SendPacket(new AnsShut(0), true);
            RemoveSession(session);
        }
    }
}
