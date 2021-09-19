using log4net;
using MHTriServer.Player;
using MHTriServer.Server.Packets;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// OpnServer (TLS)
/// This is where the initial handshake with the game happen. This server is responsible for:
///     * Maintenance announce (optional) - Announce if there would be any maintenance in the future
///     * General Announcement (optional) - Any announce that the server owner would want to use
///     * NASToken Exchange - Nintendo WFC connection token
///     * Terms Exchange - Terms and Condition exchange terms and codition
///     * VulgarityInfo (???) - Probably related to chat messages
///     * CommonKey(optional) - This key is used to encrypt / decrypt packet with the other game related server.
///         In the current state of the server, we don't use encryption, hopefully in the future we can start using it
///     * LMP Server Address Exchange
/// </summary>
namespace MHTriServer.Server
{
    public class OpnServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(OpnServer));

        private readonly PlayerManager m_PlayerManager = null;

        public OpnServer(PlayerManager playerManager, string address, int port, X509Certificate certificate) : base(address, port, certificate) 
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));
            Debug.Assert(certificate!= null);

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
            if (m_PlayerManager.TryGetPlayer(connectionData.OnlineSupportCode, out var player))
            {
                if (session.RemoteEndPoint != player.RemoteEndPoint)
                {
                    if (ContainSessionWith(player.RemoteEndPoint))
                    {
                        Log.FatalFormat("Kicked {0}, there is a player already connected with {1}", session.RemoteEndPoint, connectionData.OnlineSupportCode);
                        session.Close(Constants.MULTIPLE_ACCOUNT_ERROR_MESSAGE);
                        return;
                    }
                }
                else
                {
                    Log.FatalFormat("Kicked {0}, session is doing funny business", session.RemoteEndPoint, connectionData.OnlineSupportCode);
                    session.Close(Constants.OUT_OF_ORDER_ERROR_MESSAGE);
                    return;
                }

                // This is a dangling player object
                m_PlayerManager.UnloadPlayer(player);
            }

            // TODO: Load player if there is one associated with the OnlineSupportCode
            //  This is to check if we have to send the terms and condition again, because
            //  they have changed

            session.SendPacket(new NtcLogin(ServerLoginType.OPN_SERVER_ANOUNCE));
        }

        public override void HandleReqAuthenticationToken(NetworkSession session, ReqAuthenticationToken reqAuthenticationToken)
        {
            // Ignore NAS Token?
            session.SendPacket(new AnsAuthenticationToken());
        }

        public override void HandleReqMaintenance(NetworkSession session, ReqMaintenance reqMaintenance)
        {
            session.SendPacket(new AnsMaintenance(Constants.MAINTENANCE_MESSAGE));
        }

        public override void HandleReqTermsVersion(NetworkSession session, ReqTermsVersion reqTermsVersion)
        {
            session.SendPacket(new AnsTermsVersion(Constants.TERMS_AND_CONDITIONS_VERSION, (uint)Constants.TERMS_AND_CONDITIONS.Length));
        }

        public override void HandleReqTerms(NetworkSession session, ReqTerms reqTerms)
        {
            session.SendPacket(new AnsTerms(reqTerms.TermsCurrentLength, (uint)Constants.TERMS_AND_CONDITIONS.Length, Constants.TERMS_AND_CONDITIONS));
        }

        public override void HandleReqMediaVersionInfo(NetworkSession session, ReqMediaVersionInfo reqMediaVersionInfo)
        {
            // It seems that there is a bug or something in their network loop.
            // I can bypass a lot of thing by sending LmpConnect next

            // SendPacket(new LmpConnect("127.0.0.1", LmpServer.DefaultPort));

            session.SendPacket(new AnsMediaVersionInfo("V1.0.0", "Alpha", "Hello World1"));
        }

        public override void HandleReqAnnounce(NetworkSession session, ReqAnnounce reqAnnounce)
        {
            session.SendPacket(new AnsAnnounce(Constants.ANNOUNCEMENT_MESSAGE));
        }

        public override void HandleReqNoCharge(NetworkSession session, ReqNoCharge reqNoCharge)
        {
            session.SendPacket(new AnsNoCharge("hello Wordl2"));
        }

        public override void HandleReqVulgarityInfoLow(NetworkSession session, ReqVulgarityInfoLow reqVulgarityInfoLow)
        {
            const string message = "HelloWorld3";
            session.SendPacket(new AnsVulgarityInfoLow(1, reqVulgarityInfoLow.UnknownField, (uint)message.Length));
        }

        public override void HandleReqVulgarityLow(NetworkSession session, ReqVulgarityLow reqVulgarityLow)
        {
            const string message = "HelloWorld3";
            session.SendPacket(new AnsVulgarityLow(reqVulgarityLow.InfoType, reqVulgarityLow.CurrentLength, (uint)message.Length, message));
        }

        public override void HandleReqCommonKey(NetworkSession session, ReqCommonKey reqCommonKey)
        {
            // This probably have to do with encryption
            // TODO: Extract the encryption/decryption method that way, we can send the packet.
            // SendPacket(new AnsCommonKey(""));
            session.SendPacket(new AnsAuthenticationToken());
        }

        public override void HandleReqLmpConnect(NetworkSession session, ReqLmpConnect reqLmpConnect)
        {
            session.SendPacket(new AnsLmpConnect(MHTriServer.Config.LmpServer.Address, MHTriServer.Config.LmpServer.Port));
        }

        public override void HandleReqShut(NetworkSession session, ReqShut reqShut)
        {
            session.SendPacket(new AnsShut(0), true);
            RemoveSession(session);
        }
    }
}
