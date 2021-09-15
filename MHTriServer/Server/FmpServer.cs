using log4net;
using MHTriServer.Player;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace MHTriServer.Server
{
    public class FmpServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(FmpServer));

        private readonly PlayerManager m_PlayerManager = null;

        public FmpServer(PlayerManager playerManager, string address, int port) : base(address, port)
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

        public override bool AcceptNewConnection(Socket newSocket)
        {
            try
            {
                var player = m_PlayerManager.AddPlayer(ConnectionType.FMP, newSocket, new NetworkStream(newSocket));
                Log.InfoFormat("New player connected {0}", newSocket.RemoteEndPoint);
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
                return;
            }

            if (socket.Available == 0)
            {
                Log.InfoFormat("Connection {0} was closed gracefully", endpoint);

                RemoveClient(socket);
                m_PlayerManager.RemovePlayer(player);

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
    }
}
