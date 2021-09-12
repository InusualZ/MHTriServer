using MHTriServer.Player;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace MHTriServer.Server
{
    public class OPNServer : BaseServer
    {
        public const int DefaultPort = 8200;

        private readonly X509Certificate m_ServerCertificate = null;

        private readonly PlayerManager m_PlayerManager = null;

        public OPNServer(PlayerManager playerManager) : this(playerManager, "0.0.0.0", DefaultPort) { }

        public OPNServer(PlayerManager playerManager, string address) : this(playerManager, address, DefaultPort) { }

        public OPNServer(PlayerManager playerManager, string address, int port) : base(address, port) {
            Debug.Assert(playerManager != null);
            m_ServerCertificate = new X509Certificate2(ResourceUtils.GetResourceBytes("ServerCertificate.p12"), "1234");
            m_PlayerManager = playerManager;
        }

        public override void Start()
        {
            base.Start();

            Console.WriteLine($"OPNServer: {Address}:{Port}");
        }

        public override bool AcceptNewConnection(TcpClient client)
        {
            var sslStream = new SslStream(client.GetStream(), false);

            try
            {
                sslStream.AuthenticateAsServer(m_ServerCertificate, false, false); // TODO: In the future check certificates

                var player = m_PlayerManager.AddPlayer(ConnectionType.OPN, client.Client, sslStream);
                Console.WriteLine($"OPNServer: New player connected {client.Client.RemoteEndPoint}");
            }
            catch(Exception e)
            {
                Console.WriteLine($"OPNServer: Unable to authenticate client. {e.Message}");
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
                Console.Error.WriteLine($"OPNServer: Unable to find player {endpoint}");
                return;
            }

            if (socket.Available == 0)
            {
                Console.WriteLine($"OPNServer: Connection {endpoint} was closed gracefully");
                RemoveClient(socket);
                m_PlayerManager.RemovePlayer(player);
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
                Console.Error.WriteLine($"LMPServer: Unable to find player {endpoint}");
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
