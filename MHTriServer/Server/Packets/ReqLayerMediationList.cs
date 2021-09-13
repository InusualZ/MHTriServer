using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerMediationList : Packet
    {
        public const uint PACKET_ID = 0x64820100;

        public byte UnknownField1 { get; private set; }

        public byte UnknownField2 { get; private set; }

        public byte[] Format { get; private set; }

        public ReqLayerMediationList(byte unknownField1, byte unknownField2, byte[] format) : base(PACKET_ID) 
            => (UnknownField1, UnknownField2, Format) = (unknownField1, unknownField2, format);

        public ReqLayerMediationList(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadByte();
            UnknownField2 = reader.ReadByte();
            Format = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1:02X}\n\tUnknownField2 {UnknownField2:02X}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
