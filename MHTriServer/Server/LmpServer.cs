using log4net;
using MHTriServer.Player;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace MHTriServer.Server
{
    public class LmpServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(LmpServer));

        private Player.Player m_PendingPlayer = null; 

        private readonly PlayerManager m_PlayerManager = null;

        public LmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));

            m_PlayerManager = playerManager;
        }

        public override void Start()
        {
            base.Start();

            Log.InfoFormat("Running on {0}:{1}", Address, Port);
        }

        public override bool AcceptNewConnection(TcpClient client)
        {
            try
            {
                if (m_PendingPlayer == null)
                {
                    var player = m_PlayerManager.AddPlayer(ConnectionType.LMP, client.Client, client.GetStream());
                    Log.InfoFormat("New player connected {0}", client.Client.RemoteEndPoint);
                }
                else
                {
                    m_PendingPlayer.SetConnection(ConnectionType.LMP, client.Client, client.GetStream());
                    Log.InfoFormat("Player reconnected {0}", client.Client.RemoteEndPoint);
                }
            }
            catch (Exception e)
            {
                Log.Fatal("Unable to accept new player", e);
                return false;
            }

            return true;
        }

        public override void HandleSocketRead(Socket socket)
        {
            var endpoint = socket.RemoteEndPoint;
            if (!m_PlayerManager.TryGetPlayer(endpoint, out var player))
            {
                Log.FatalFormat("Unable to find player {0}", endpoint);
                RemoveClient(socket);
                return;
            }

            if (socket.Available == 0)
            {
                Log.InfoFormat("Connection {0} was closed gracefully", endpoint);

                RemoveClient(socket);

                // At this point we have to handle the later reconnection of this player. To the same server!!!

                if (m_PendingPlayer == null)
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

            player.ReadPacketFromStream();

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
                Log.FatalFormat("Unable to find player {0}", endpoint);
                RemoveClient(socket);
                return;
            }

            player.HandleWrite();

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
