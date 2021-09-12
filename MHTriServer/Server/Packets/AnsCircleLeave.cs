using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleLeave : Packet
    {
        public const uint PACKET_ID = 0x65040200;

        public uint UnknownField1 { get; private set; }

        public AnsCircleLeave(uint unknownField1) : base(PACKET_ID)
            => (UnknownField1) = (unknownField1);

        public AnsCircleLeave(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            UnknownField1 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}";
        }
    }
}
