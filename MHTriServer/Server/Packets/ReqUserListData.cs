using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqUserListData : Packet
    {
        public const uint PACKET_ID = 0x61110100;

        public uint UnknownField { get; private set; }

        public uint SlotCount { get; private set; }

        public ReqUserListData(uint unknownField, uint slotCount) : base(PACKET_ID) => (UnknownField, SlotCount) = (unknownField, slotCount);

        public ReqUserListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(SlotCount);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);

            UnknownField = reader.ReadUInt32();
            SlotCount = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqUserListData(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField}\n\tSlotCount {SlotCount}";
        }
    }
}
