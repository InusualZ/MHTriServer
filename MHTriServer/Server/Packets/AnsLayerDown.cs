using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerDown : Packet
    {
        public const uint PACKET_ID = 0x64140200;

        public ushort UnknownField { get; private set; }

        public AnsLayerDown(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public AnsLayerDown(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 2);

            UnknownField = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
