using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqFmpInfo : Packet
    {
        public const uint PACKET_ID = 0x61340100;

        public const uint PACKET_ID_FMP = 0x63140100;

        public uint UnknownField { get; private set; }

        public byte[] Format { get; private set; }

        public ReqFmpInfo(uint unknownField, byte[] format) : base(PACKET_ID) 
            => (UnknownField, Format) = (unknownField, format);

        public ReqFmpInfo(uint unknownField, byte[] format, bool isServerFmp) : base(PACKET_ID_FMP)
            => (UnknownField, Format) = (unknownField, format);

        public ReqFmpInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || ID == PACKET_ID_FMP);
            UnknownField = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
