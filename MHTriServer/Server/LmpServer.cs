using log4net;
using MHTriServer.Player;
using MHTriServer.Server.Packets;
using MHTriServer.Server.Packets.Properties;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server
{
    public class LmpServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(LmpServer));

        private readonly PlayerManager m_PlayerManager = null;

        public LmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
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

        public override void HandleNtcCollectionLog(NetworkSession session, NtcCollectionLog collectionLog)
        {
            var data = collectionLog.Data;
            Log.WarnFormat("Session {0} Error {1:X8} {2} {3}", session.RemoteEndPoint, data.ErrorCode, data.UnknownField2, data.Timeout);
        }

        public override void HandleAnsConnection(NetworkSession session, AnsConnection ansConnection)
        {
            var connectionData = ansConnection.Data;
            var onlineSupportCode = PlayerManager.GetOnlineSupportCodeFrom(connectionData);

            var afterFirstConnection = true;
            if (!m_PlayerManager.TryGetPlayer(onlineSupportCode, out var player))
            {
                afterFirstConnection = false;
                player = m_PlayerManager.Create/* OrLoad */(session.RemoteEndPoint, connectionData);
                if (player == null)
                {
                    Log.FatalFormat("Kicked {0}, the number of account have reach its limit", session.RemoteEndPoint);
                    session.Close(Constants.MAX_ACCOUNT_REACHED_ERROR_MESSAGE);
                    return;
                }
            }
            else
            {
                Debug.Assert(player.RequestedUserList);

                // Make sure to update the remote endpoint so future player lookup work
                player.RemoteEndPoint = session.RemoteEndPoint;
                player.RequestedUserList = false;
            }

            session.SetTag(player);
            session.SendPacket(new NtcLogin(!afterFirstConnection ? ServerLoginType.LMP_NORMAL_FIRST : ServerLoginType.LMP_NORMAL_SECOND));
        }

        public override void HandleReqLoginInfo(NetworkSession session, ReqLoginInfo reqLoginInfo)
        {
            var player = session.GetPlayer();

            var chargeInfo = new ChargeInfo()
            {
                TicketValidity1 = 1,
                TicketValidity2 = 2,
                UnknownField5 = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x31 }, // "Hello World1"
                OnlineSupportCode = player.OnlineSupportCode
            };

            const byte NeedPATTicket = 0x01;

            session.SendPacket(new AnsLoginInfo(NeedPATTicket, "Hello World3", chargeInfo), true);

            player.SentOnlineSupportCode = true;
        }

        public override void HandleReqTicketClient(NetworkSession session, ReqTicketClient reqTicketClient)
        {
            var player = session.GetPlayer();
            session.SendPacket(new AnsTicketClient(player.OnlineSupportCode));
        }
        public override void HandleReqUserListHead(NetworkSession session, ReqUserListHead reqUserListHead)
        {
            var player = session.GetPlayer();
            session.SendPacket(new AnsUserListHead(0, 6));
        }

        public override void HandleReqUserListData(NetworkSession session, ReqUserListData reqUserListData)
        {
            var player = session.GetPlayer();

            // TODO: Load from database,
            var slots = new List<UserSlot>();
            for (var i = 0; i < reqUserListData.SlotCount; ++i)
            {
                slots.Add(UserSlot.NoData((uint)i));
            }
            session.SendPacket(new AnsUserListData(slots));
        }

        public override void HandleReqServerTime(NetworkSession session, ReqServerTime reqServerTime)
        {
            var player = session.GetPlayer();

            session.SendPacket(new AnsServerTime(1500, 0));
        }

        public override void HandleReqUserListFoot(NetworkSession session, ReqUserListFoot reqUserListFoot)
        {
            var player = session.GetPlayer();
            session.SendPacket(new AnsUserListFoot(), true);
            player.RequestedUserList = true;
        }

        public override void HandleReqUserObject(NetworkSession session, ReqUserObject reqUserObject)
        {
            var player = session.GetPlayer();

            reqUserObject.Slot.SlotIndex = 1;
            reqUserObject.Slot.SaveID = Player.Player.DEFAULT_USER_ID; // Guid.NewGuid().ToString().Substring(0, 7); // TODO: Replace this token
            session.SendPacket(new AnsUserObject(1, string.Empty, reqUserObject.Slot));
        }

        public override void HandleReqFmpListVersion(NetworkSession session, ReqFmpListVersion reqFmpListVersion)
        {
            var player = session.GetPlayer();

            session.SendPacket(new AnsFmpListVersion(1));
        }

        public override void HandleReqFmpListHead(NetworkSession session, ReqFmpListHead reqFmpListHead)
        {
            var player = session.GetPlayer();

            session.SendPacket(new AnsFmpListHead(0, 1));
        }

        public override void HandleReqFmpListData(NetworkSession session, ReqFmpListData reqFmpListData)
        {
            var player = session.GetPlayer();

            var servers = new List<FmpData>() 
            {
                FmpData.Server(1, 2, 3, 2, "Valor333333", 1),
            };
            session.SendPacket(new AnsFmpListData(servers));
        }

        public override void HandleReqFmpListFoot(NetworkSession session, ReqFmpListFoot reqFmpListFoot)
        {
            var player = session.GetPlayer();
            session.SendPacket(new AnsFmpListFoot());
        }

        public override void HandleReqFmpInfo(NetworkSession session, ReqFmpInfo reqFmpInfo)
        {
            var player = session.GetPlayer();
            player.RequestedFmpServerAddress = true;
            session.SendPacket(new AnsFmpInfo(FmpData.Address(MHTriServer.Config.FmpServer.Address, MHTriServer.Config.FmpServer.Port)));
        }

        public override void HandleReqBinaryHead(NetworkSession session, ReqBinaryHead reqBinaryHead)
        {
            var player = session.GetPlayer();

            uint binaryLength = 0;
            if (reqBinaryHead.BinaryType == 5)
            {
                // Arbitrary Length
                binaryLength = (uint)(Player.Player.BINARY_DATA_5_TEST.Length);
            }
            else
            {
                Log.DebugFormat("ReqBinaryRead Type {0}", reqBinaryHead);
            }

            session.SendPacket(new AnsBinaryHead(reqBinaryHead.BinaryType, binaryLength));
        }

        public override void HandleReqBinaryData(NetworkSession session, ReqBinaryData reqBinaryData)
        {
            var player = session.GetPlayer();

            uint offset = 0;
            byte[] binaryData = null;
            if (reqBinaryData.Type == 5)
            {
                // Unknown request, max size is 0x1ff
                binaryData = Encoding.ASCII.GetBytes(Player.Player.BINARY_DATA_5_TEST);
            }

            session.SendPacket(new AnsBinaryData(reqBinaryData.Type, offset, binaryData));
        }

        public override void HandleReqUserSearchInfoMine(NetworkSession session, ReqUserSearchInfoMine reqUserSearchInfoMine)
        {
            var player = session.GetPlayer();

            // The client won't even read the data that we would send, so why bother.
            session.SendPacket(new AnsUserSearchInfoMine(new CompoundList()));
        }

        public override void HandleReqBinaryFoot(NetworkSession session, ReqBinaryFoot reqBinaryFoot)
        {
            session.SendPacket(new AnsBinaryFoot());
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

            if (!player.RequestedUserList && !player.RequestedFmpServerAddress)
            {
                if (player.Created && !player.SentOnlineSupportCode)
                {
                    // This means that the player disconnected before we even could send the 
                    // OnlineSupportCode to it. So don't flush the player to database because
                    // when the player reconnect, it would still have an invalid OnlineSupportCode
                    // TODO: Mark player to not be flush to database
                }
                m_PlayerManager.RemovePlayer(player);
            }

            // TODO: We have to make sure to cleanup this player reference in case the client 
            //  never got to connect to the FmpServer
        }
    }
}
