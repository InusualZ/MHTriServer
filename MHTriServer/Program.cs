using MHTriServer.Player;
using MHTriServer.Server;
using MHTriServer.Server.Packets;
using PcapngUtils.Common;
using PcapngUtils.PcapNG;
using System;
using System.IO;
using System.Threading;

namespace MHTriServer
{
    class Program
    {

        public static void OpenPcapFile(string filename, CancellationToken token)
        {
            using (var reader = new PcapNGReader(filename, false))
            {
                reader.OnReadPacketEvent += reader_OnReadPacketEvent;
                reader.ReadPackets(token);
                reader.OnReadPacketEvent -= reader_OnReadPacketEvent;
            }
        }

        static void reader_OnReadPacketEvent(object context, IPacket packet)
        {
            using var packetStream =  new MemoryStream(packet.Data[54..]);
            var packetReader = new ExtendedBinaryReader(packetStream, Endianness.Big);
            var size = packetReader.ReadUInt16();
            var counter = packetReader.ReadUInt16();
            var packetId = packetReader.ReadUInt32();
            var packetData = Packet.CreateFrom(packetId, size, counter);
            if (packetData != null)
            {
                Console.WriteLine($"Packet {packetData.GetType().Name} {packetData.ID:X8}");
            }
            else
            {
                Console.WriteLine($"Unknown Packet {packetId:X8}");
            }
        }


        public static int Main(string[] args)
        {
            var token = new CancellationToken();
            OpenPcapFile(@"D:\Monster Hunter Tri\ngevan2k\selected.pcapng", token);
            return 0;

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
