using MHTriServer.Player;
using MHTriServer.Server;
using System;

namespace MHTriServer
{
    class Program
    {

        public static int Main(string[] args)
        {
            var playerManager = new PlayerManager();

            var opnServer = new OPNServer(playerManager);
            var lmpServer = new LmpServer(playerManager);
            var fmpServer = new FmpServer(playerManager);

            opnServer.Start();
            lmpServer.Start();
            fmpServer.Start();

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            fmpServer.Stop();
            lmpServer.Stop();
            opnServer.Stop();

            return 0;
        }
    }
}
