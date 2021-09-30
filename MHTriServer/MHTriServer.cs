using log4net;
using log4net.Config;
using MHTriServer.Database;
using MHTriServer.Server;
using MHTriServer.Utils;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Tommy.Serializer;

namespace MHTriServer
{
    public class MHTriServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(MHTriServer));

        private const string CONFIG_NAME = "MHTriServer.toml";

        public static ServerConfig Config;

        public static int Main(string[] args)
        {
            try
            {
                InitializeLogger();
            }
            catch (Exception e)
            {
                Log.Fatal($"Unable to initialize the logging system", e);
                return 1;
            }

            try
            {
                Config = InitializeConfig();
            }
            catch (Exception e)
            {
                Log.Fatal($"Unable to read config `{CONFIG_NAME}` file", e);
                return 1;
            }

            BackendContext backend;
            try
            {
                backend = new BackendContext();
            }
            catch (Exception e)
            {
                Log.Fatal($"Failed to initialize the backend", e);
                return 1;
            }

            var playerManager = new PlayerManager(backend);

            X509Certificate2 opnServerCertificate;
            try
            {
                opnServerCertificate = new X509Certificate2(Config.OpnServer.CertificatePath, Config.OpnServer.CertificatePassphrase);
            }
            catch (CryptographicException e)
            {
                Log.FatalFormat("Unable to parse `{0}` certificate. {1}", Config.OpnServer.CertificatePath, e.Message);
                return 1;
            }

            var opnServer = new OpnServer(playerManager, Config.OpnServer.Address, Config.OpnServer.Port, opnServerCertificate);
            var lmpServer = new LmpServer(playerManager, Config.LmpServer.Address, Config.LmpServer.Port);
            var fmpServer = new FmpServer(playerManager, Config.FmpServer.Address, Config.FmpServer.Port);

            opnServer.Start();
            lmpServer.Start();
            fmpServer.Start();

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    Log.Warn("Closing servers...");
                    break;
                }
            }

            fmpServer.Stop();
            lmpServer.Stop();
            opnServer.Stop();

            backend.SaveChanges();
            backend.Dispose();

            return 0;
        }

        public static void InitializeLogger()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var filepath = Path.Join(Path.GetDirectoryName(executingAssembly.Location), "log4net.xml");
            if (!File.Exists(filepath))
            {
                throw new ApplicationException("Please verify that `log4net.xml` exist at the root folder of the program binary");
            }

            var repo = LogManager.GetRepository(executingAssembly);
            XmlConfigurator.Configure(repo, new FileInfo(filepath));

            // Setup Thread Name
            Thread.CurrentThread.Name = nameof(MHTriServer);
        }

        public static ServerConfig InitializeConfig()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var configPath = Path.Join(Path.GetDirectoryName(executingAssembly.Location), CONFIG_NAME);
            if (!File.Exists(configPath))
            {
                var defaultConfigBytes = ResourceUtils.GetResourceBytes(CONFIG_NAME);
                File.WriteAllBytes(configPath, defaultConfigBytes);
                Log.Warn("Default config has been written");
            }

            return TommySerializer.FromTomlFile<ServerConfig>(configPath);
        }
    }
}
