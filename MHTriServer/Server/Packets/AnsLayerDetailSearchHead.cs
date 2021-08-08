using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerDetailSearchHead : Packet
    {
        public const uint PACKET_ID = 0x64900200;

        public uint UnknownField1 { get; private set; }

        public uint UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public AnsLayerDetailSearchHead(uint unknownField1, uint unknownField2, uint unknownField3) : base(PACKET_ID)
            => (UnknownField1, UnknownField2, UnknownField3) = (unknownField1, unknownField2, unknownField3);

        public AnsLayerDetailSearchHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
            writer.Write(UnknownField3);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            UnknownField3 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}\n\tUnknownField2 {UnknownField2}\n\tUnknownField3({UnknownField3})";
        }
    }
}
