using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerUserList : Packet
    {
        public const uint PACKET_ID = 0x64630200;

        private List<LayerUserData> m_slots;

        public IReadOnlyList<LayerUserData> Slots => m_slots;

        public AnsLayerUserList(List<LayerUserData> slots) : base(PACKET_ID) => m_slots = slots;

        public AnsLayerUserList(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write((uint)Slots.Count);
            foreach (var slot in m_slots)
            {
                slot.Serialize(writer);
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            var slotListCount = reader.ReadUInt32();
            m_slots = new List<LayerUserData>((int)slotListCount);
            for (var i = 0; i < slotListCount; ++i)
            {
                m_slots.Add(CompoundList.Deserialize<LayerUserData>(reader));
            }
        }
    }
}
