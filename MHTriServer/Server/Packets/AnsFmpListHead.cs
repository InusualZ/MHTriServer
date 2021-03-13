using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsFmpListHead : Packet
    {
        public const uint PACKET_ID = 0x61310200;

        public const uint PACKET_ID_FMP = 0x63110200;

        public uint ServerCount { get; private set; }

        public AnsFmpListHead(uint serverCount) : base(PACKET_ID) => (ServerCount) = (serverCount);

        public AnsFmpListHead(uint serverCount, bool isServerFmp) : base(PACKET_ID_FMP) => (ServerCount) = (serverCount);

        public AnsFmpListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Ignored field
            writer.Write(ServerCount);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || PACKET_ID == PACKET_ID_FMP);
            Debug.Assert(Size == 8);
            _ = reader.ReadUInt32();
            ServerCount = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tServerCount {ServerCount}";
        }
    }
}
