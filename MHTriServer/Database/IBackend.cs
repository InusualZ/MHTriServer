using MHTriServer.Database.Models;

namespace MHTriServer.Database
{
    public interface IBackend
    {
        public bool PlayerExist(string uuid);

        public void AddPlayer(OfflinePlayer player);

        public OfflinePlayer GetPlayerByUUID(string uuid);

        public void RemovePlayer(string uuid);

        public void SaveChanges();
    }
}
