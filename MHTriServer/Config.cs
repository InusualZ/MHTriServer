using MHTriServer.Server.Game;
using Tommy.Serializer;

namespace MHTriServer
{
    [TommyRoot]
    public class ServerConfig
    {
        public OpnServerConfig OpnServer { get; set; }

        public LmpServerConfig LmpServer { get; set; }

        public FmpServerConfig FmpServer { get; set; }

        public DatabaseConfig Database { get; set; }

        public GameConfig Game { get; set; }
    }

    public class BaseServerConfig
    {
        public string Address { get; set; }

        public ushort Port { get; set; }
    }

    [TommyTableName("OpnServer")]
    public class OpnServerConfig : BaseServerConfig
    {
        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }
    }

    [TommyTableName("LmpServer")]
    public class LmpServerConfig : BaseServerConfig
    {

    }

    [TommyTableName("FmpServer")]
    public class FmpServerConfig : BaseServerConfig
    {

    }

    [TommyTableName("Database")]
    public class DatabaseConfig
    {
        public string Connection { get; set; }
    }

    [TommyTableName("Game")]
    public class GameConfig
    {
        public uint MaxHunterSlots { get; set; }

        public short Timeout1 { get; set; }

        public short Timeout2 { get; set; }

        public short Timeout3 { get; set; }

        public short Timeout4 { get; set; }

        public short Timeout5 { get; set; }

        public short Timeout6 { get; set; }

        public short Timeout7 { get; set; }

        public short Timeout8 { get; set; }


        [TommyValue("ServerType")]
        public ServerType[] ServerTypes { get; set; }

        [TommyValue("HRLimit")]
        public HRLimit[] HRLimits { get; set; }

        [TommyValue("Seeking")]
        public Seeking[] Seekings { get; set; }

        [TommyValue("Goal")]
        public Goal[] Goals { get; set; }
    }
}
