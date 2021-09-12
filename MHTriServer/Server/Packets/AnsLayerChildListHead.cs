using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerChildListHead : Packet
    {
        public const uint PACKET_ID = 0x64240200;

        public uint Count { get; private set; }

        public AnsLayerChildListHead(uint count) : base(PACKET_ID) => (Count) = (count);

        public AnsLayerChildListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Ignored field
            writer.Write(Count);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);
            _ = reader.ReadUInt32();
            Count = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCount {Count}";
        }
    }
}
