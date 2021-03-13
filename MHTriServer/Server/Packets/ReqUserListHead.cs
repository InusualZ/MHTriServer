using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserListHead : Packet
    {
        public const uint PACKET_ID = 0x61100100;

        public uint UnknownField { get; private set; }

        public uint ClientSlotCount { get; private set; }

        public byte[] Format { get; set; }

        public ReqUserListHead(uint unknownField, uint clientSlotCount, byte[] format) : base(PACKET_ID) => (UnknownField, ClientSlotCount, Format) = (unknownField, clientSlotCount, format);

        public ReqUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(ClientSlotCount);

            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt32();
            ClientSlotCount = reader.ReadUInt32();

            Format = reader.ReadByteBytes();
        }
    }
}
