using MHTriServer.Database.Models;
using MHTriServer.Server.Packets.Properties;
using System;
using System.Collections.Generic;
using System.Net;

namespace MHTriServer.Server
{
    public class Player
    {
        public const int HUNTER_SAVE_ID_LENGTH = 8 - 1 /* null char */;

        // TEMP STATIC VARIABLE
        public static uint BINARY_VERSION_COUNT = 0;
        public static readonly string BINARY_DATA_5_TEST = "\t\tWhat!!!!\n\t\tHello World\n\t\tInusualZ\n\t\tHello Dev\n\t\tMore\n\t\tDude\n\t\tStop\n\t\tPlease";
        public static readonly byte[] BINARY_DATA_1;

        private readonly OfflinePlayer m_OfflinePlayer;

        public EndPoint RemoteEndPoint { get; set; }

        public string OnlineSupportCode { get => m_OfflinePlayer.UUID; }

        public bool Created { get; set; }

        public bool Loaded => !Created;

        public bool SentOnlineSupportCode { get; set; }

        public bool RequestedUserList { get; set; }

        public bool RequestedFmpServerAddress { get; set; }

        public DateTime? LasLogin { get => m_OfflinePlayer.LastLogin; set => m_OfflinePlayer.LastLogin = value; }

        // TODO: Refactor, I don't like exposing the databse interface
        public List<OfflineHunter> Hunters { get => m_OfflinePlayer.Hunters; set => m_OfflinePlayer.Hunters = value; }

        public OfflineHunter SelectedHunter { get; set; }

        /*
         * TEMP VARIABLES
         */

        public bool AfterLayerChildData = false;
        public bool AfterUserBinaryNotice = false;

        static Player()
        {
            BINARY_DATA_1 = ServerType.GenerateBinaryData();
        }

        public Player(OfflinePlayer offlinePlayer, EndPoint remoteEndPoint, bool created)
        {
            m_OfflinePlayer = offlinePlayer;
            
            RemoteEndPoint = remoteEndPoint;

            Created = created;
            RequestedUserList = false;
            RequestedUserList = false;
        }
    }

    public static class PlayerExtension
    {
        public static Player GetPlayer(this NetworkSession networkSession) 
            => networkSession.GetTag<Player>();
    }
}
