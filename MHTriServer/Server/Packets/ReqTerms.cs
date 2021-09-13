using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqTerms : Packet
    {
        public const uint PACKET_ID = 0x62110100;

        public uint TermsVersion { get; private set; }

        public uint TermsCurrentLength { get; private set; }

        public uint TermsExpectedLength { get; private set; }

        public ReqTerms(uint termsVersion, uint expectedLength, uint extraLength) : base(PACKET_ID) => (TermsVersion, TermsCurrentLength, TermsExpectedLength) = (termsVersion, expectedLength, extraLength);

        public ReqTerms(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(TermsVersion);
            writer.Write(TermsCurrentLength);
            writer.Write(TermsExpectedLength);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 12);
            TermsVersion = reader.ReadUInt32();
            TermsCurrentLength = reader.ReadUInt32();
            TermsExpectedLength = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tTermsVersion {TermsVersion}\n\tTermsCurrentLength {TermsCurrentLength}\n\tTermsExpectedLength {TermsExpectedLength}";
        }
    }
}
