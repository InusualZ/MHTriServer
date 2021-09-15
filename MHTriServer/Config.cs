namespace MHTriServer
{

    public interface IConfig
    {
        public IOpnServerConfig OpnServer { get; }

        public ILmpServerConfig LmpServer { get; }

        public IFmpServerConfig FmpServer { get; }
    }

    public interface IServerConfig
    {
        public string Address { get; }

        public ushort Port { get; }
    }

    public interface IOpnServerConfig : IServerConfig
    {
        public string CertificatePath { get; }

        public string CertificatePassphrase { get; }
    }

    public interface ILmpServerConfig : IServerConfig
    {

    }

    public interface IFmpServerConfig : IServerConfig
    {

    }
}
