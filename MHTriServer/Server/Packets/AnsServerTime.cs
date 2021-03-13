using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsServerTime : Packet
    {
        public const uint PACKET_ID = 0x60020200;

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public AnsServerTime(uint unknownField, uint unknownField2) : base(PACKET_ID) => (UnknownField, UnknownField2) = (unknownField, unknownField2);

        public AnsServerTime(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);

            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tUnknownField2 {UnknownField2}";
        }
    }
}
