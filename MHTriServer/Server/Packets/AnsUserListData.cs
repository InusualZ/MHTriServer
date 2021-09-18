using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsUserListData : Packet
    {
        public const uint PACKET_ID = 0x61110200;

        private List<UserSlot> m_slots;

        public IReadOnlyList<UserSlot> Slots => m_slots;

        public AnsUserListData(List<UserSlot> slots) : base(PACKET_ID) => m_slots = slots;

        public AnsUserListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((uint)0); // This field is read, but not used.
            writer.Write((uint) Slots.Count);

            foreach(var slot in m_slots)
            {
                slot.Serialize(writer);
            }
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            _ = reader.ReadUInt32();

            var slotListCount = reader.ReadUInt32();
            m_slots = new List<UserSlot>((int)slotListCount);
            for (var i = 0; i < slotListCount; ++i)
            {
                m_slots.Add(CompoundList.Deserialize<UserSlot>(reader));
            }
        }
    }
}
