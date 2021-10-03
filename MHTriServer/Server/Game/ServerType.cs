using System.Linq;
using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    [TommyTableName("ServerType")]
    public class ServerType
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ushort MinimumRank { get; set; }

        public ushort MaximumRank { get; set; }

        [TommyValue("Server")]
        public GameServer[] Servers { get; set; }

        [TommyIgnore]
        public int MaxPopulation => Servers.Sum(c => c.MaxPopulation);

        [TommyIgnore]
        public bool IsFull => Servers.All(c => c.IsFull);

        public void Init (ref uint serverIndex)
        {
            foreach (var server in Servers)
            {
                server.Init(++serverIndex);
            }
        }
    }
}
