using log4net;
using MHTriServer.Player;
using MHTriServer.Server.Packets;
using MHTriServer.Server.Packets.Properties;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace MHTriServer.Server
{
    public class LmpServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(LmpServer));

        private readonly PlayerManager m_PlayerManager = null;

        // TEMP
        private readonly Dictionary<string, int> m_CodeCounter;

        public LmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));

            m_PlayerManager = playerManager;
            m_CodeCounter = new Dictionary<string, int>();
        }

        public override void Start()
        {
            base.Start();

            Log.InfoFormat("Running on {0}:{1}", Address, Port);
        }

        public override void HandleAnsConnection(NetworkSession session, AnsConnection ansConnection)
        {
            // Very naive implmentation of how this should be implemented
            var onlineCode = "NoSupport"; // ansConnection.Data.OnlineSupportCode;
            if (!m_CodeCounter.TryGetValue(onlineCode, out var currentCounter))
            {
                currentCounter = 1;
                m_CodeCounter.Add(onlineCode, currentCounter);
            }
            else
            {
                m_CodeCounter[onlineCode] = ++currentCounter;
            }
            var afterFirstConnection = (currentCounter % 2) == 0;

            session.SendPacket(new NtcLogin(!afterFirstConnection ? ServerLoginType.LMP_NORMAL_FIRST : ServerLoginType.LMP_NORMAL_SECOND));
        }
        // TODO: Do something with the data
        public override void HandleReqLoginInfo(NetworkSession session, ReqLoginInfo reqLoginInfo)
        {
            var chargeInfo = new ChargeInfo()
            {
                TicketValidity1 = 1,
                TicketValidity2 = 2,
                UnknownField5 = new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x31 }, // "Hello World1"
                OnlineSupportCode = "NoSupport"
            };

            // TODO: Figure out what login info byte 1 means?
            // Depending on the state of this argument. The client would go different paths
            // during the login process
            const byte loginInfoByte = 0x01;

            session.SendPacket(new AnsLoginInfo(loginInfoByte, "Hello World3", chargeInfo));
        }

        public override void HandleReqTicketClient(NetworkSession session, ReqTicketClient reqTicketClient)
        {
            // I doubt this is correct? Need more RE
            session.SendPacket(new AnsTicketClient(Player.Player.PlayerToken));
        }
        // TODO: Do something with the data
        public override void HandleReqUserListHead(NetworkSession session, ReqUserListHead reqUserListHead)
        {
            session.SendPacket(new AnsUserListHead(0, 6));
        }

        public override void HandleReqUserListData(NetworkSession session, ReqUserListData reqUserListData)
        {
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
            session.SendPacket(new AnsServerTime(1500, 0));
        }

        public override void HandleReqUserListFoot(NetworkSession session, ReqUserListFoot reqUserListFoot)
        {
            session.SendPacket(new AnsUserListFoot());
        }

        public override void HandleReqUserObject(NetworkSession session, ReqUserObject reqUserObject)
        {
            reqUserObject.Slot.SlotIndex = 1;
            reqUserObject.Slot.SaveID = Player.Player.DEFAULT_USER_ID; // Guid.NewGuid().ToString().Substring(0, 7); // TODO: Replace this token
            session.SendPacket(new AnsUserObject(1, string.Empty, reqUserObject.Slot));
        }

        public override void HandleReqFmpListVersion(NetworkSession session, ReqFmpListVersion reqFmpListVersion)
        {
            session.SendPacket(new AnsFmpListVersion(1));
        }

        public override void HandleReqFmpListHead(NetworkSession session, ReqFmpListHead reqFmpListHead)
        {
            session.SendPacket(new AnsFmpListHead(0, 1));
        }

        public override void HandleReqFmpListData(NetworkSession session, ReqFmpListData reqFmpListData)
        {
            var servers = new List<FmpData>() 
            {
                FmpData.Server(1, 2, 3, 2, "Valor333333", 1),
            };
            session.SendPacket(new AnsFmpListData(servers));
        }

        public override void HandleReqFmpListFoot(NetworkSession session, ReqFmpListFoot reqFmpListFoot)
        {
            session.SendPacket(new AnsFmpListFoot());
        }

        public override void HandleReqFmpInfo(NetworkSession session, ReqFmpInfo reqFmpInfo)
        {
            session.SendPacket(new AnsFmpInfo(FmpData.Address(MHTriServer.Config.FmpServer.Address, MHTriServer.Config.FmpServer.Port)));
        }

        public override void HandleReqBinaryHead(NetworkSession session, ReqBinaryHead reqBinaryHead)
        {
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

        public override void OnSessionRemoved(NetworkSession session) { }
    }
}
