using MHTriServer.Server.Packets.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace MHTriServer.Player
{
    /// <summary>
    ///  TODO: We need to take care of player that were left alive due to disconnection
    ///  when they were trying to connect to the Fmp Server. Cleanup Method
    /// </summary>
    public class PlayerManager
    {
        private const char SERVER_ONLINE_CODE_PREFIX = 'W';

        // TODO: Currently, is not thread-safe. Implement lock mechanism
        private readonly List<Player> m_Players;

        public PlayerManager()
        {

            m_Players = new List<Player>();
        }

        public Player Create(EndPoint endPoint, ConnectionData connectionData)
        {
            var onlineSupportCode = connectionData.OnlineSupportCode;

            // Create Online Support Code
            if (!IsValidOnlineSupportCode(onlineSupportCode))
            {
                const int MAX_ITERATION = 100;
                var foundSlot = false;
                for (var i = 0; i < MAX_ITERATION; ++i)
                {
                    onlineSupportCode = GetNewOnlineCode();
                    var foundDuplicate = false;
                    foreach (var player in m_Players)
                    {
                        if (player.OnlineSupportCode == onlineSupportCode)
                        {
                            foundDuplicate = true;
                            break;
                        }
                    }

                    if (!foundDuplicate)
                    {
                        foundSlot = true;
                        break;
                    }
                }

                if (!foundSlot)
                {
                    return null;
                }
            }

            // TODO: Do something with the other connection data fields

            var result = new Player(endPoint, onlineSupportCode);
            result.Created = true;

            m_Players.Add(result);
            return result;
        }

        public void RemovePlayer(Player toRemove) => m_Players.Remove(toRemove);

        public void RemovePlayer(EndPoint endPoint)
        {
            for (var i = 0; i < m_Players.Count; ++i)
            {
                var player = m_Players[i];
                if (player.RemoteEndPoint == endPoint)
                {
                    m_Players.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemovePlayer(string onlineSupportCode)
        {
            for (var i = 0; i < m_Players.Count; ++i)
            {
                var player = m_Players[i];
                if (player.OnlineSupportCode == onlineSupportCode)
                {
                    m_Players.RemoveAt(i);
                    return;
                }
            }
        }

        public bool TryGetPlayer(EndPoint endpoint, out Player outPlayer)
        {
            Debug.Assert(endpoint != null);

            outPlayer = default;
            foreach (var player in m_Players)
            {
                if (player.RemoteEndPoint == endpoint)
                {
                    outPlayer = player;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetPlayer(string onlineSupportCode, out Player outPlayer)
        {
            outPlayer = default;
            if (string.IsNullOrWhiteSpace(onlineSupportCode))
            {
                return false;
            }

            foreach (var player in m_Players)
            {
                if (player.OnlineSupportCode == onlineSupportCode)
                {
                    outPlayer = player;
                    return true;
                }
            }

            return false;
        }

        public static string GetOnlineSupportCodeFrom(ConnectionData connectionData)
        {
            var onlineSupportCode = connectionData.OnlineSupportCode;
            if (!IsValidOnlineSupportCode(onlineSupportCode))
            {
                if (connectionData.PatTicket != null)
                {
                    // We use the PAT Ticket, because in the first connection to the Lmp Server
                    // we set the PAT Ticket to be equal to OnlineSupportCode
                    try
                    {
                        onlineSupportCode = Encoding.ASCII.GetString(connectionData.PatTicket);
                    }
                    catch (Exception)
                    {
                        onlineSupportCode = null;
                    }
                }
                else
                {
                    onlineSupportCode = null;
                }
            }

            return onlineSupportCode;
        }

        public static bool IsValidOnlineSupportCode(string onlineSupportCode)
        {
            const string INVALID_ONLINE_CODE = "NoSupport";
            const string INVALID_ONLINE_CODE_MH3SP = "NoOnlineSupport";

            return !string.IsNullOrWhiteSpace(onlineSupportCode) && 
                onlineSupportCode != INVALID_ONLINE_CODE &&
                onlineSupportCode != INVALID_ONLINE_CODE_MH3SP;
        }

        private static string GetNewOnlineCode()
        {
            // Server generated online code length, client max online code lenth is 32 (0x20)
            const int SERVER_ONLINE_CODE_LENGTH = 28;

            var result = Guid.NewGuid().ToString().Substring(9);
            result = SERVER_ONLINE_CODE_PREFIX + result;
            Debug.Assert(result.Length == SERVER_ONLINE_CODE_LENGTH);
            return result;
        }
    }
}
