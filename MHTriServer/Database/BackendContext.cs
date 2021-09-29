using MHTriServer.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MHTriServer.Database
{
    public class BackendContext : DbContext, IBackend
    {
        public DbSet<OfflinePlayer> Players { get; set; }

        public BackendContext() : base()
        {
            Database.EnsureCreated();
        }

        public OfflinePlayer GetPlayerByUUID(string uuid) =>
            Players.Include(p => p.Hunters).FirstOrDefault(p => p.UUID == uuid);

        public bool PlayerExist(string uuid) =>
            Players.Any(p => p.UUID == uuid);

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(MHTriServer.Config.Database.Connection);

        public void AddPlayer(OfflinePlayer player) => Players.Add(player);

        public void RemovePlayer(string uuid)
        {
            var player = new OfflinePlayer(uuid);
            Players.Attach(player);
            Players.Remove(player);
        }

        public OfflinePlayer GetPlayer(string uuid, bool loadRelatedData)
        {
            if (loadRelatedData)
            {
                return Players.Include(p => p.Hunters).FirstOrDefault(p => p.UUID == uuid);
            }

            return Players.FirstOrDefault(p => p.UUID == uuid);
        }

        void IBackend.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
