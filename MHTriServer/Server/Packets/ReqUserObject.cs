using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqUserObject : Packet
    {
        public const uint PACKET_ID = 0x61200100;

        public bool SlotIsEmpty { get; private set; }

        public uint SlotIndex { get; private set; }

        public HunterSlot Slot { get; private set; }

        public ReqUserObject(bool slotIsEmpty, uint slotIndex, HunterSlot slot) : base(PACKET_ID) => (SlotIsEmpty, SlotIndex, Slot) = (slotIsEmpty, slotIndex, slot);

        public ReqUserObject(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SlotIsEmpty);
            writer.Write(SlotIndex);
            Slot.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            SlotIsEmpty = reader.ReadBoolean();
            SlotIndex = reader.ReadUInt32();
            Slot = CompoundList.Deserialize<HunterSlot>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqUserObject(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tSlotIsEmpty {SlotIsEmpty}\n\tSlotIndex {SlotIndex}\n\tSlot\n{Slot}";
        }
    }
}
