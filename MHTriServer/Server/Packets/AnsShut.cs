using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsShut : Packet
    {
        public const uint PACKET_ID = 0x60100200;

        public byte UnknownField { get; set; }

        public AnsShut(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public AnsShut(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);
            UnknownField = reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField:X2}";
        }
    }
}
