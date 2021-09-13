using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerChildListHead : Packet
    {
        public const uint PACKET_ID = 0x64240100;

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public byte[] Format { get; private set; }

        public ReqLayerChildListHead() : base(PACKET_ID) { }

        public ReqLayerChildListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}" +
                $"\n\tUnknownField2 {UnknownField2}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
