using MHTriServer.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace MHTriServer.Server
{
    public class FmpServer : BaseServer
    {
        public const int DefaultPort = 8220;

        private readonly PlayerManager m_PlayerManager = null;

        public FmpServer(PlayerManager playerManager) : this(playerManager, "0.0.0.0", DefaultPort) { }

        public FmpServer(PlayerManager playerManager, string address) : this(playerManager, address, DefaultPort) { }

        public FmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            m_PlayerManager = playerManager;
        }

        public override void Start()
        {
            base.Start();

            Console.WriteLine($"FmpServer: {Address}:{Port}");
        }

        public override bool AcceptNewConnection(TcpClient client)
        {
            try
            {
                var player = m_PlayerManager.AddPlayer(ConnectionType.FMP, client.Client, client.GetStream());
                Console.WriteLine($"FmpServer: New player connected {client.Client.RemoteEndPoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"FmpServer: Unable to accept new player. {e.Message}");
                return false;
            }

            return true;
        }

        public override void HandleSocketRead(Socket socket)
        {
            var endpoint = socket.RemoteEndPoint;
            if (!m_PlayerManager.TryGetPlayer(endpoint, out var player))
            {
                Console.Error.WriteLine($"FmpServer: Unable to find player {endpoint}");
                return;
            }

            if (socket.Available == 0)
            {
                Console.WriteLine($"FmpServer: Connection {endpoint} was closed gracefully");

                RemoveClient(socket);
                m_PlayerManager.RemovePlayer(player);
                return;
            }

            try
            {
                player.HandlePacket();
            }
            catch (Exception e) { }


            // Handle player closing his own socket.
            // Refactor asap
            if (!socket.Connected)
            {
                RemoveClient(socket);
                m_PlayerManager.RemovePlayer(player);
            }
        }

        public override void HandleSocketWrite(Socket socket)
        {
            var endpoint = socket.RemoteEndPoint;
            if (!m_PlayerManager.TryGetPlayer(endpoint, out var player))
            {
                Console.Error.WriteLine($"FmpServer: Unable to find player {endpoint}");
                return;
            }

            player.HandleState();

            // Handle player closing his own socket.
            // Refactor asap
            if (!socket.Connected)
            {
                RemoveClient(socket);
                m_PlayerManager.RemovePlayer(player);
            }
        }

        public override void HandleSocketError(Socket socket)
        {
            RemoveClient(socket);
            m_PlayerManager.RemovePlayer(socket.RemoteEndPoint);
        }
    }
}
