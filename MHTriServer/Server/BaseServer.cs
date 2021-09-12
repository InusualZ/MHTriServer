using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MHTriServer.Server
{
    public abstract class BaseServer
    {
        private Task m_ServerTask = null;

        public string Address { get; private set; }

        public int Port { get; private set; }

        protected TcpListener m_TcpListener = null;

        protected List<TcpClient> m_Clients = null;

        public bool Running { get; set; } = false;

        public BaseServer(string address, int port) => (Address, Port) = (address, port);

        public void Listen()
        {
            m_TcpListener = new TcpListener(IPAddress.Parse(Address), Port);
            m_TcpListener.Start(); // TODO: Backlog configurable
            m_Clients = new List<TcpClient>();
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
            List<Socket> GetSockets()
            {
                return m_Clients.Select(c => c.Client).Append(m_TcpListener.Server).ToList();
            }

            List<Socket> GetSocketsClient()
            {
                return m_Clients.Select(c => c.Client).ToList();
            }

            while (Running)
            {
                var socketRead = GetSockets();
                var socketWrite = GetSocketsClient();
                var socketError = GetSocketsClient();

                // Blocking 
                Socket.Select(socketRead, socketWrite, socketError, -1);

                foreach(var socket in socketRead)
                {
                    if (socket == m_TcpListener.Server)
                    {
                        var newClient = m_TcpListener.AcceptTcpClient();
                        if (AcceptNewConnection(newClient))
                        {
                            m_Clients.Add(newClient);
                        }
                        continue;
                    }

                    HandleSocketRead(socket);
                }

                foreach (var socket in socketWrite)
                {
                    if (!socket.Connected)
                    {
                        continue;
                    }

                    HandleSocketWrite(socket);
                }

                foreach (var socket in socketError)
                {
                    HandleSocketError(socket);
                }
            }
        }

        public abstract bool AcceptNewConnection(TcpClient client);

        public abstract void HandleSocketRead(Socket socket);

        public abstract void HandleSocketWrite(Socket socket);

        public abstract void HandleSocketError(Socket socket);

        public bool RemoveClient(TcpClient clientToRemove)
        {
            if (m_Clients.Remove(clientToRemove))
            {
                clientToRemove.Close();
                clientToRemove.Dispose();

                return true;
            }

            return false;
        }

        public bool RemoveClient(Socket socket)
        {
            for (var i = 0; i < m_Clients.Count; ++i) 
            {
                var client = m_Clients[i];
                if (client.Client == socket)
                {
                    m_Clients.RemoveAt(i);
                    client.Close();
                    client.Dispose();
                    return true;
                }
            }

            return false;
        }

        public virtual void Stop()
        {
            if (Running)
            {
                Running = false;
                m_ServerTask.Wait();
            }
        }
    }
}
