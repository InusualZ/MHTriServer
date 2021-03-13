using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class AnsUserSelectedSlot : Packet
    {
        public const uint PACKET_ID = 0x61200100;

        public bool SlotIsEmpty { get; private set; }

        public uint SlotIndex { get; private set; }

        public UserSlot Slot { get; private set; }

        public AnsUserSelectedSlot(bool slotIsEmpty, uint slotIndex, UserSlot slot) : base(PACKET_ID) => (SlotIsEmpty, SlotIndex, Slot) = (slotIsEmpty, slotIndex, slot);

        public AnsUserSelectedSlot(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SlotIsEmpty);
            writer.Write(SlotIndex);
            Slot.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            SlotIsEmpty = reader.ReadBoolean();
            SlotIndex = reader.ReadUInt32();
            Slot = CompoundList.Deserialize<UserSlot>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tSlotIsEmpty {SlotIsEmpty}\n\tSlotIndex {SlotIndex}\n\tSlot\n{Slot}";
        }
    }
}
