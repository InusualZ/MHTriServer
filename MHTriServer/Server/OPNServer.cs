using log4net;
using MHTriServer.Player;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace MHTriServer.Server
{
    public class OpnServer : BaseServer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(OpnServer));

        private readonly PlayerManager m_PlayerManager = null;
        private readonly X509Certificate m_ServerCertificate = null;

        public OpnServer(PlayerManager playerManager, string address, int port, X509Certificate certificate) : base(address, port) 
        {
            Debug.Assert(playerManager != null);
            Debug.Assert(!string.IsNullOrEmpty(address));
            Debug.Assert(certificate!= null);

            m_PlayerManager = playerManager;
            m_ServerCertificate = certificate;
        }

        public override void Start()
        {
            base.Start();

            Log.InfoFormat("Running on {0}:{1}", Address, Port);
        }

        public override bool AcceptNewConnection(Socket newSocket)
        {
            var sslStream = new SslStream(new NetworkStream(newSocket), false);

            try
            {
                sslStream.AuthenticateAsServer(m_ServerCertificate);

                var player = m_PlayerManager.AddPlayer(ConnectionType.OPN, newSocket, sslStream);
                Log.InfoFormat("New player connected {0}", newSocket.RemoteEndPoint);
            }
            catch(Exception e)
            {
                Log.ErrorFormat("OPNServer: Unable to authenticate client. {0}", e.Message);
                sslStream.Close();
                return false;
            }

            return true;
        }

        public override void HandleSocketRead(Socket socket)
        {
            var endpoint = socket.RemoteEndPoint;
            if (!m_PlayerManager.TryGetPlayer(endpoint, out var player))
            {
                Log.FatalFormat("Unable to find player session for {0}", endpoint);
                RemoveClient(socket);
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
