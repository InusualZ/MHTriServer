using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqBinaryVersion : Packet
    {
        public const uint PACKET_ID = 0x63010100;

        public byte UnknownField { get; private set; }

        public ReqBinaryVersion(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqBinaryVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
