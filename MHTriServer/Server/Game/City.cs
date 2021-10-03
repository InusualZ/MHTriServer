using System.Collections.Generic;
using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    [TommyTableName("City")]
    public class City
    {
        [TommyIgnore]
        public int Id { get; private set; }
        
        public string Name { get; set; }

        [TommyIgnore]
        public int CurrentPopulation { get => Players.Count; }

        public ushort MaxPopulation { get; set; }

        [TommyIgnore]
        public bool IsFull => Players.Count >= MaxPopulation;

        [TommyIgnore]
        public int DepartedPlayer { get; set; }

        [TommyIgnore]
        public List<Player> Players { get; set; }

        public void Init(int index)
        {
            Id = index;
            Players = new List<Player>(MaxPopulation);
        }
    }
}
