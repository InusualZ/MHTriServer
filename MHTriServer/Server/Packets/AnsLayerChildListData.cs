using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerChildListData : Packet
    {
        public const uint PACKET_ID = 0x64250200;

        private List<LayerData> m_slots;

        public IReadOnlyList<LayerData> Slots => m_slots;

        public AnsLayerChildListData(List<LayerData> slots) : base(PACKET_ID) => m_slots = slots;

        public AnsLayerChildListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((uint)0); // This field is read, but not used.
            writer.Write((uint) Slots.Count);

            foreach(var slot in m_slots)
            {
                slot.Serialize(writer);
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            _ = reader.ReadUInt32();

            var slotListCount = reader.ReadUInt32();
            m_slots = new List<LayerData>((int)slotListCount);
            for (var i = 0; i < slotListCount; ++i)
            {
                m_slots.Add(CompoundList.Deserialize<LayerData>(reader));
            }
        }
    }
}
