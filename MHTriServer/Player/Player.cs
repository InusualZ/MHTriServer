using MHTriServer.Server;
using MHTriServer.Server.Packets.Properties;
using System.Diagnostics;
using System.Net;

namespace MHTriServer.Player
{
    public class Player
    {
        public const string DEFAULT_USER_ID = "AAAA";

        // TEMP STATIC VARIABLE
        public static uint BINARY_VERSION_COUNT = 0;
        public static readonly string BINARY_DATA_5_TEST = "\t\tWhat!!!!\n\t\tHello World\n\t\tInusualZ\n\t\tHello Dev\n\t\tMore\n\t\tDude\n\t\tStop\n\t\tPlease";
        public static readonly byte[] BINARY_DATA_1;

        public EndPoint RemoteEndPoint { get; set; }

        public string OnlineSupportCode { get; }

        public bool Created { get; set; }

        public bool Loaded => !Created;

        public bool SentOnlineSupportCode { get; set; }

        public bool RequestedUserList { get; set; }

        public bool RequestedFmpServerAddress { get; set; }

        /*
         * TEMP VARIABLES
         */

        public bool AfterLayerChildData = false;
        public bool AfterUserBinaryNotice = false;

        static Player()
        {
            BINARY_DATA_1 = ServerType.GenerateBinaryData();
        }

        public Player(EndPoint remoteEndPoint, string onlineSupportCode)
        {
            RemoteEndPoint = remoteEndPoint;
            OnlineSupportCode = onlineSupportCode;

            Created = false;
            RequestedUserList = false;
            RequestedUserList = false;
        }
    }

    public static class PlayerExtension
    {
        public static Player GetPlayer(this NetworkSession networkSession)
        {
            var player = networkSession.GetTag<Player>();
            Debug.Assert(player != null);
            return player;
        }
    }
}
