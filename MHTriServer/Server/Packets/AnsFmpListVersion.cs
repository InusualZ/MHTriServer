using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsFmpListVersion : Packet
    {
        public const uint PACKET_ID = 0x61300200;
        public const uint PACKET_ID_FMP = 0x63100200;

        public uint Version { get; private set; }

        public AnsFmpListVersion(uint version) : base(PACKET_ID) => (Version) = (version);

        public AnsFmpListVersion(uint version, bool isServerFmp) : base(PACKET_ID_FMP) => (Version) = (version);

        public AnsFmpListVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);
            Version = reader.ReadUInt32();
        }
    }
}
