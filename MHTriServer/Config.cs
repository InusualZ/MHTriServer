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
}
