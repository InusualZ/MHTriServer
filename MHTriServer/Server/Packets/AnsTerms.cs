using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsTerms : Packet
    {
        public const uint PACKET_ID = 0x62110200;

        public uint Offset { get; private set; }

        public uint UnknownField { get; private set; }

        public string TermsMessage { get; private set; }

        public AnsTerms(uint termsLength, uint unknownField, string termsMessage) : base(PACKET_ID) => (Offset, UnknownField, TermsMessage) = (termsLength, unknownField, termsMessage);

        public AnsTerms(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Offset);
            writer.Write(UnknownField);
            writer.Write(TermsMessage);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 12);
            Offset = reader.ReadUInt32();
            UnknownField = reader.ReadUInt32();
            TermsMessage = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tOffset {Offset}\n\tUnknownField {UnknownField}\n\tTermsMessage {TermsMessage}";
        }
    }
}
