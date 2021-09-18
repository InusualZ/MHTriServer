using MHTriServer.Server;
using MHTriServer.Server.Packets.Properties;

namespace MHTriServer.Player
{
    public class Player
    {
        public const string DEFAULT_USER_ID = "AAAA";

        // TODO: Replace with a proper db to store this token
        public static string PlayerToken;

        public static uint BINARY_VERSION_COUNT = 0;

        public ConnectionType ConnectionType { get; private set; }

        public static readonly string BINARY_DATA_5_TEST = "\t\tWhat!!!!\n\t\tHello World\n\t\tInusualZ\n\t\tHello Dev\n\t\tMore\n\t\tDude\n\t\tStop\n\t\tPlease";
        public static readonly byte[] BINARY_DATA_1;

        static Player()
        {
            BINARY_DATA_1 = ServerType.GenerateBinaryData();
        }

        /*
         * TEMP VARIABLES
         */

        public static bool AfterLayerChildData = false;
        public static bool AfterUserBinaryNotice = false;

        internal Player(ConnectionType connectionType)
        {
            SetConnection(connectionType);
        }

        public void SetConnection(ConnectionType connectionType)
        {
            ConnectionType = connectionType;
        }
    }
}
