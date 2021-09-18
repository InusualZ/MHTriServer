using log4net;
using MHTriServer.Server.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace MHTriServer.Server
{
    public abstract class BaseServer : PacketHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(BaseServer));

        private Task m_ServerTask = null;

        protected TcpListener m_TcpListener = null;

        protected List<NetworkSession> m_Sessions = null;

        public string Address { get; }

        public int Port { get; }

        public X509Certificate Certificate { get; } = null;

        public bool Running { get; private set; } = false;

        public BaseServer(string address, int port) => (Address, Port) = (address, port);

        public BaseServer(string address, int port, X509Certificate certificate) =>
            (Address, Port, Certificate) = (address, port, certificate);

        private void Listen()
        {
            m_TcpListener = new TcpListener(IPAddress.Parse(Address), Port);
            m_TcpListener.Start();
            m_Sessions = new List<NetworkSession>();
        }

        public virtual void Start()
        {
            Debug.Assert(Running == false);
            Running = true;

            Listen();

            m_ServerTask = Task.Run(Run);
        }

        public void Run()
        {
            const int WAIT_INDEFINITELY = -1;

            Thread.CurrentThread.Name = this.GetType().Name;

            var selectReadList = new List<Socket>();
            var selectWriteList = new List<Socket>();
            while (Running)
            {
                selectReadList.Add(m_TcpListener.Server);

                foreach (var session in m_Sessions)
                {
                    Debug.Assert(session.Socket?.Connected == true);
                    selectReadList.Add(session.Socket);
                }

                foreach (var session in m_Sessions)
                {
                    Debug.Assert(session.Socket?.Connected == true);
                    selectWriteList.Add(session.Socket);
                }

                // Blocking 
                Socket.Select(selectReadList, selectWriteList, null, WAIT_INDEFINITELY);

                foreach (var socket in selectReadList)
                {
                    if (socket == m_TcpListener.Server)
                    {
                        var newSocket = m_TcpListener.AcceptSocket();

                        Stream networkStream = new NetworkStream(newSocket);

                        if (Certificate != null)
                        {
                            var sslStream = new SslStream(networkStream, false);

                            try
                            {
                                sslStream.AuthenticateAsServer(Certificate);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Unable to authenticate as server {newSocket.RemoteEndPoint}", ex);
                                newSocket.Dispose();
                                continue;
                            }

                            networkStream = sslStream;
                        }

                        var newSession = new NetworkSession(newSocket, networkStream);
                        m_Sessions.Add(newSession);

                        Log.InfoFormat("New session {0}", newSession.RemoteEndPoint);

                        newSession.SendHandshake();

                        continue;
                    }

                    HandleSocketRead(socket);
                }

                foreach (var socket in selectWriteList)
                {
                    // We detect that the client has gracefully closed the connection when
                    // we try to read from the socket, and we receive 0 bytes back. That means,
                    // that the socket has have passed through the previous foreach loop
                    // which means that the socket has been closed already
                    if (!socket.Connected)
                    {
                        continue;
                    }

                    HandleSocketWrite(socket);
                }

                selectReadList.Clear();
                selectWriteList.Clear();
            }

            foreach (var socket in m_Sessions)
            {
                socket.Dispose();
            }

            m_Sessions.Clear();
        }

        private void HandleSocketRead(Socket socket)
        {
            var session = GetNetworkSession(socket);
            Debug.Assert(session != null);

            if (session.Socket.Available < 1)
            {
                Log.InfoFormat("Session `{0}` was closed gracefully", session.RemoteEndPoint);
                RemoveSession(session);
                return;
            }

            var packets = session.ReadPackets();
            Debug.Assert(packets.Count > 0);

            foreach (var packet in packets)
            {
                session.NextResponseCounter = packet.Counter;

                try
                {
                    packet.Handle(this, session);
                }
                catch (NotImplementedException)
                {
                    Log.Error($"Packet `{packet.GetType().Name}` left unhandled by server");
                }
                catch (Exception e)
                {
                    Log.Fatal($"Error ocurred while handling packet {packet.GetType().Name}", e);
                }
            }

            if (!socket.Connected)
            {
                RemoveSession(session);
            }
        }

        public override void HandleAnsLineCheck(NetworkSession session, AnsLineCheck ansLineCheck) {}

        private void HandleSocketWrite(Socket socket)
        {
            var session = GetNetworkSession(socket);
            Debug.Assert(session != null);

            if (session.CouldWrite())
            {
                try
                {
                    session.FlushWriteBuffer();
                }
                catch (Exception e)
                {
                    Log.Fatal($"Error ocurred while writing to `{session.RemoteEndPoint}`", e);
                    RemoveSession(session);
                }
            }

            try
            {
                session.CheckPingSystem();
            }
            catch (Exception e)
            {
                Log.Fatal($"PingSystem - Error ocurred while writing to`{session.RemoteEndPoint}`", e);
                RemoveSession(session);
            }

            if (!socket.Connected)
            {
                RemoveSession(session);
            }
        }

        private NetworkSession GetNetworkSession(Socket socket)
        {
            foreach (var session in m_Sessions)
            {
                if (session.Socket == socket)
                {
                    return session;
                }
            }

            // TEST: Under current condition this should be imposible
            Debug.Assert(false);
            return null;
        }

        public virtual void OnSessionRemoved(NetworkSession session) { }

        public bool RemoveSession(NetworkSession session)
        {
            if (m_Sessions.Remove(session))
            {
                OnSessionRemoved(session);

                if (session.Socket?.Connected == true)
                {
                    Log.InfoFormat("Closed {0} connection", session.RemoteEndPoint);
                }

                session.Dispose();
                return true;
            }

            return false;
        }

        private void RemoveSessionBySocket(Socket socket)
        {
            for (var i = 0; i < m_Sessions.Count; ++i)
            {
                var session = m_Sessions[i];
                if (session.Socket != socket)
                {
                    continue;
                }
                
                OnSessionRemoved(session);

                session.Dispose();

                m_Sessions.RemoveAt(i);
                return;
            }
        }

        public virtual void Stop()
        {
            if (!Running)
            {
                return;
            }

            Running = false;
            m_ServerTask.Wait();
            m_ServerTask = null;

            m_TcpListener.Stop();
            m_TcpListener = null;
        }
    }
}
