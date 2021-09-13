using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqUserListHead : Packet
    {
        public const uint PACKET_ID = 0x61100100;

        public uint UnknownField { get; private set; }

        public uint SlotCount { get; private set; }

        public byte[] Format { get; set; }

        public ReqUserListHead(uint unknownField, uint slotCount, byte[] format) : base(PACKET_ID) => (UnknownField, SlotCount, Format) = (unknownField, slotCount, format);

        public ReqUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(SlotCount);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt32();
            SlotCount = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tSlotCount {SlotCount}\n\tFormat {Packet.Hexstring(Format, ' ')}";
        }
    }
}
