using Tommy.Serializer;

namespace MHTriServer.Server.Game
{
    public class Seeking
    {
        [TommyIgnore]
        public uint Index { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }

        public byte Flag { get; set; }
    }
}
