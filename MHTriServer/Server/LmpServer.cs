using MHTriServer.Player;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace MHTriServer.Server
{
    public class LmpServer : BaseServer
    {
        public const int DefaultPort = 8210;

        private Player.Player m_PendingPlayer = null;

        private readonly PlayerManager m_PlayerManager = null;

        public LmpServer(PlayerManager playerManager) : this(playerManager, "0.0.0.0", DefaultPort) { }

        public LmpServer(PlayerManager playerManager, string address) : this(playerManager, address, DefaultPort) { }

        public LmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            m_PlayerManager = playerManager;
        }

        public override void Start()
        {
            base.Start();

            Console.WriteLine($"LmpServer: {Address}:{Port}");
        }

        public override bool AcceptNewConnection(TcpClient client)
        {
            try
            {
                if (m_PendingPlayer == null)
                {
                    var player = m_PlayerManager.AddPlayer(ConnectionType.LMP, client.Client, client.GetStream());
                    Console.WriteLine($"LmpServer: New player connected {client.Client.RemoteEndPoint}");
                }
                else
                {
                    m_PendingPlayer.SetConnection(ConnectionType.LMP, client.Client, client.GetStream());
                    Console.WriteLine($"LmpServer: Player reconnected {client.Client.RemoteEndPoint}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"LmpServer: Unable to accept new player. {e.Message}");
                return false;
            }

            return true;
        }

        public override void HandleSocketRead(Socket socket)
        {
            var endpoint = socket.RemoteEndPoint;
            if (!m_PlayerManager.TryGetPlayer(endpoint, out var player))
            {
                Console.Error.WriteLine($"LmpServer: Unable to find player {endpoint}");
                return;
            }

            if (socket.Available == 0)
            {
                Console.WriteLine($"LmpServer: Connection {endpoint} was closed gracefully");

                RemoveClient(socket);
                // At this point we have to handle the later reconnection of this player. To the same server!!!

                if (player.NetworkState == 35 && m_PendingPlayer == null)
                {
                    m_PendingPlayer = player;
                }
                else
                {
                    m_PlayerManager.RemovePlayer(player);
                    m_PendingPlayer = null;
                }

                return;
            }

            player.HandlePacket();

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
                Console.Error.WriteLine($"LmpServer: Unable to find player {endpoint}");
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
