using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    public class Goal
    {
        [TommyIgnore]
        public uint Index { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public byte RestrictionMode { get; set; }

        public ushort Minimum { get; set; }

        public ushort Maximum { get; set; }
    }
}
