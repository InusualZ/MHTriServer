using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsTermsVersion : Packet
    {
        public const uint PACKET_ID = 0x62100200;

        public uint TermsVersion { get; private set; }

        public uint TermsLength { get; private set; }

        public AnsTermsVersion(uint termsVersion, uint termsLength) : base(PACKET_ID) => (TermsVersion, TermsLength) = (termsVersion, termsLength);

        public AnsTermsVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(TermsVersion);
            writer.Write(TermsLength);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);
            TermsVersion = reader.ReadUInt32();
            TermsLength = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tTermsVersion {TermsVersion}\n\tTermsLength {TermsLength}";
        }
    }
}
