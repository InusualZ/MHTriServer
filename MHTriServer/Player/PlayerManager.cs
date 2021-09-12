using MHTriServer.Server;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace MHTriServer.Player
{
    public class PlayerManager
    {
        private List<Player> m_Players;

        public IReadOnlyList<Player> Players => m_Players;

        public PlayerManager()
        {
            m_Players = new List<Player>();
        }

        public Player AddPlayer(ConnectionType connectionType, Socket socket, Stream stream)
        {
            var player = new Player(connectionType, socket, stream);
            m_Players.Add(player);
            return player;
        }

        public void RemovePlayer(Player toRemove)
        {
            m_Players.Remove(toRemove);
        }

        public void RemovePlayer(EndPoint endPoint) => m_Players.RemoveAll(p => p.RemoteEndPoint == endPoint);

        public bool TryGetPlayer(EndPoint endpoint,  out Player outPlayer)
        {
            outPlayer = default;
            foreach(var player in m_Players)
            {
                if (player.RemoteEndPoint == endpoint)
                {
                    outPlayer = player;
                    return true;
                }
            }

            return false;
        }
    }
}
