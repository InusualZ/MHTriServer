using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    public class HRLimit
    {
        [TommyIgnore]
        public uint Index { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public ushort Minimum { get; set; }

        public ushort Maximum { get; set; }
    }
}
