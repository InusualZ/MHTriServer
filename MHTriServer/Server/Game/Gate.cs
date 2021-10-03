using System.Collections.Generic;
using System.Linq;
using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    [TommyTableName("Gate")]
    public class Gate
    {
        [TommyIgnore]
        public int Id { get; private set; }

        public string Name { get; set; }

        [TommyValue("City")]
        public City[] Cities { get; set; }

        [TommyIgnore]
        public int CurrentPopulation => Cities.Sum(c => c.CurrentPopulation) + PlayerInGate.Count;

        [TommyIgnore]
        public int MaxPopulation => Cities.Sum(c => c.MaxPopulation);

        [TommyIgnore]
        public int InCityPopulation => Cities.Sum(c => c.CurrentPopulation);

        [TommyIgnore]
        public bool IsFull => Cities.All(c => c.IsFull);

        [TommyIgnore]
        public IEnumerable<Player> PlayersInCity => Cities.SelectMany(c => c.Players);

        [TommyIgnore]
        public List<Player> PlayerInGate { get; set; }

        public void Init(int index)
        {
            Id = index;
            PlayerInGate = new List<Player>();

            var cityIndex = 0;
            foreach (var city in Cities)
            {
                city.Init(++cityIndex);
            }
        }
    }
}
