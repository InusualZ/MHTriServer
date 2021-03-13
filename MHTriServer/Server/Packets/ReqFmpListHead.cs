using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqFmpListHead : Packet
    {
        public const uint PACKET_ID = 0x61310100;
        public const uint PACKET_ID_FMP = 0x63110100;

        public uint FmpListVersion { get; private set; }

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public byte[] Format { get; private set; }

        public ReqFmpListHead() : base(PACKET_ID) { }

        public ReqFmpListHead(bool isServerFmp) : base(PACKET_ID_FMP) { }

        public ReqFmpListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(FmpListVersion);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || ID == PACKET_ID_FMP);
            FmpListVersion = reader.ReadUInt32();
            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tFmpListVersion {FmpListVersion}\n\tUnknownField {UnknownField}" +
                $"\n\tUnknownField2 {UnknownField2}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
