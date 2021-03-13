using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsServerWrong : Packet
    {
        public const uint PACKET_ID = 0x60020100;

        public uint UnknownField { get; private set; }

        public AnsServerWrong(uint unknownField) : base(PACKET_ID) => (UnknownField) = (unknownField);

        public AnsServerWrong(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            UnknownField = reader.ReadUInt32();
        }
    }
}
