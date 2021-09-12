using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsVulgarityInfoLow : Packet
    {
        public const uint PACKET_ID = 0x62560200;

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public uint InfoLength { get; private set; }

        public AnsVulgarityInfoLow(uint unknownField, uint unknownField2, uint infoLength) : base(PACKET_ID) => (UnknownField, UnknownField2, InfoLength) = (unknownField, unknownField2, infoLength);

        public AnsVulgarityInfoLow(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
            writer.Write(InfoLength);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 12);
            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            InfoLength = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tUnknownField2 {UnknownField2}\n\tInfoLength {InfoLength}";
        }
    }
}
