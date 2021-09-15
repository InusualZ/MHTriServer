using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MHTriServer.Server
{
    public abstract class BaseServer
    {
        private Task m_ServerTask = null;

        protected TcpListener m_TcpListener = null;

        protected List<Socket> m_Clients = null;

        public string Address { get; }

        public int Port { get; }

        public bool Running { get; private set; } = false;

        public BaseServer(string address, int port) => (Address, Port) = (address, port);

        public void Listen()
        {
            m_TcpListener = new TcpListener(IPAddress.Parse(Address), Port);
            m_TcpListener.Start();
            m_Clients = new List<Socket>();
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

            var selectReadList = new List<Socket>();
            var selectWriteList = new List<Socket>();
            while (Running)
            {
                selectReadList.Add(m_TcpListener.Server);
                selectReadList.AddRange(m_Clients);

                selectWriteList.AddRange(m_Clients);

                // Blocking 
                Socket.Select(selectReadList, selectWriteList, null, WAIT_INDEFINITELY);

                foreach(var socket in selectReadList)
                {
                    if (socket == m_TcpListener.Server)
                    {
                        var newClient = m_TcpListener.AcceptSocket();
                        if (AcceptNewConnection(newClient))
                        {
                            m_Clients.Add(newClient);
                        }
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

            foreach (var socket in m_Clients)
            {
                socket.Close();
            }

            m_Clients.Clear();
        }

        public abstract bool AcceptNewConnection(Socket newSocket);

        public abstract void HandleSocketRead(Socket socket);

        public abstract void HandleSocketWrite(Socket socket);

        public bool RemoveClient(Socket socket)
        {
            for (var i = 0; i < m_Clients.Count; ++i) 
            {
                var entry = m_Clients[i];
                if (entry != socket)
                {
                    continue;
                }

                m_Clients.RemoveAt(i);
                entry.Close();
                entry.Dispose();
                return true;
            }

            return false;
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
