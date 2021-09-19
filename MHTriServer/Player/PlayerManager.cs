using MHTriServer.Database;
using MHTriServer.Database.Models;
using MHTriServer.Server.Packets.Properties;
using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, Player> m_Players;

        private readonly IBackend m_Database;

        public PlayerManager(IBackend database)
        {
            m_Players = new ConcurrentDictionary<string, Player>();
            m_Database = database;
        }

        public bool TryCreate(EndPoint endPoint, ConnectionData connectionData, out Player outPlayer)
        {
            outPlayer = default;
            var onlineSupportCode = GetOnlineSupportCodeFrom(connectionData);

            // Create Online Support Code
            if (!IsValidOnlineSupportCode(onlineSupportCode))
            {
                const int MAX_ITERATION = 10;

                var foundSlot = false;
                for (var i = 0; i < MAX_ITERATION; ++i)
                {
                    onlineSupportCode = GetNewOnlineCode();
                    if (!m_Database.PlayerExist(onlineSupportCode))
                    {
                        foundSlot = true;
                        break;
                    }
                }

                if (!foundSlot)
                {
                    return false;
                }
            }

            // TODO: Do something with the other connection data fields

            var offlinePlayer = new OfflinePlayer(onlineSupportCode);
            m_Database.AddPlayer(offlinePlayer);

            // TODO: Remove from here
            m_Database.SaveChanges();

            outPlayer = new Player(offlinePlayer, endPoint, true);
            return m_Players.TryAdd(outPlayer.OnlineSupportCode, outPlayer);
        }

        public bool TryLoadOrCreatePlayer(EndPoint endPoint, ConnectionData connectionData, out Player outPlayer)
        {
            var onlineSupportCode = GetOnlineSupportCodeFrom(connectionData);
            if (TryLoadPlayer(onlineSupportCode, endPoint, out outPlayer))
            {
                return true;
            }

            return TryCreate(endPoint, connectionData, out outPlayer);
        }

        public bool UnloadPlayer(Player toRemove) =>
            m_Players.Remove(toRemove.OnlineSupportCode, out var _);

        public bool UnloadPlayer(EndPoint endPoint)
        {
            foreach(var (uuid, player) in m_Players)
            {
                if (player.RemoteEndPoint == endPoint)
                {
                    return m_Players.Remove(uuid, out var _);
                }
            }

            return false;
        }

        public bool UnloadPlayer(string onlineSupportCode) =>
            m_Players.Remove(onlineSupportCode, out var player);

        /// <summary>
        /// This would remove the player completely, from the local list and the database
        /// </summary>
        /// <param name="onlineSupportCode"></param>
        public void RemovePlayer(string onlineSupportCode)
        {
            m_Players.Remove(onlineSupportCode, out var _);

            m_Database.RemovePlayer(onlineSupportCode);
            // TODO: Remove from here
            m_Database.SaveChanges();
        }

        public bool TryGetPlayer(EndPoint endpoint, out Player outPlayer)
        {
            Debug.Assert(endpoint != null);

            outPlayer = default;
            foreach (var (_, player) in m_Players)
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

            return m_Players.TryGetValue(onlineSupportCode, out outPlayer);
        }

        public bool TryLoadPlayer(string onlineSupportCode, EndPoint remoteEndpoint, out Player outPlayer)
        {
            outPlayer = default;

            var offlinePlayer = m_Database.GetPlayerByUUID(onlineSupportCode);
            if (offlinePlayer == null)
            {
                return false;
            }

            outPlayer = new Player(offlinePlayer, remoteEndpoint, false);
            return m_Players.TryAdd(onlineSupportCode, outPlayer);
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
