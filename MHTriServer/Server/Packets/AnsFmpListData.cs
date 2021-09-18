
using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsFmpListData : Packet
    {
        public const uint PACKET_ID = 0x61320200;
        public const uint PACKET_ID_FMP = 0x63120200;

        private List<FmpData> m_slots;

        public IReadOnlyList<FmpData> Slots => m_slots;

        public AnsFmpListData(List<FmpData> slots) : base(PACKET_ID) => m_slots = slots;

        public AnsFmpListData(List<FmpData> slots, bool isServerFmp) : base(PACKET_ID_FMP) => m_slots = slots;

        public AnsFmpListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((uint)0); // This field is read, but not used.
            writer.Write((uint)Slots.Count);

            foreach (var slot in m_slots)
            {
                slot.Serialize(writer);
            }
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            _ = reader.ReadUInt32();

            var slotListCount = reader.ReadUInt32();
            m_slots = new List<FmpData>((int)slotListCount);
            for (var i = 0; i < slotListCount; ++i)
            {
                m_slots.Add(CompoundList.Deserialize<FmpData>(reader));
            }
        }
    }
}
