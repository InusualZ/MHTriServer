
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerUserListHead : Packet
    {
        public const uint PACKET_ID = 0x64640200;

        public uint UnknownField1 { get; private set; }

        public uint UnknownField2 { get; private set; }

        public AnsLayerUserListHead(uint unknownField1, uint unknownField2) : base(PACKET_ID)
            => (UnknownField1, UnknownField2) = (unknownField1, unknownField2);

        public AnsLayerUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}\n\tUnknownField2 {UnknownField2}";
        }
    }
}

