using System.Collections.Generic;
using System.Linq;
using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    [TommyTableName("Server")]
    public class GameServer
    {
        public string Name { get; set; }

        [TommyValue("Gate")]
        public Gate[] Gates { get; set; }

        [TommyIgnore]
        public uint ServerIndex { get; private set; }

        [TommyIgnore]
        public int CurrentPopulation => Gates.Sum(g => g.CurrentPopulation);

        [TommyIgnore]
        public int InCityPopulation => Gates.Sum(g => g.InCityPopulation);

        [TommyIgnore]
        public int MaxPopulation => Gates.Sum(c => c.MaxPopulation);

        [TommyIgnore]
        public bool IsFull => Gates.All(c => c.IsFull);

        [TommyIgnore]
        public IEnumerable<Player> Players => Gates.SelectMany(g => g.PlayersInCity.Concat(g.PlayerInGate));

        public void Init(uint serverIndex)
        {
            ServerIndex = serverIndex;

            var gateIndex = 0;
            foreach (var gate in Gates)
            {
                gate.Init(++gateIndex);
            }
        }
    }
}
