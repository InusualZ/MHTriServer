using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerMediationList : Packet
    {
        public const uint PACKET_ID = 0x64820200;

        private List<MediationData> m_elements;

        public byte UnknownField1 { get; private set; }

        public IReadOnlyList<MediationData> Elements => m_elements;

        public AnsLayerMediationList(byte unknownField1, List<MediationData> elements) : base(PACKET_ID) => (UnknownField1, m_elements) = (unknownField1, elements);

        public AnsLayerMediationList(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            Debug.Assert(m_elements.Count < 0x20);

            base.Serialize(writer);

            writer.Write(UnknownField1);
            writer.Write((byte)Elements.Count);

            foreach (var element in m_elements)
            {
                element.Serialize(writer);
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            UnknownField1 = reader.ReadByte();
            var elementCount = (int)reader.ReadByte();

            Debug.Assert(elementCount < 0x20);

            m_elements = new List<MediationData>(elementCount);
            for (var i = 0; i < elementCount; ++i)
            {
                m_elements.Add(CompoundList.Deserialize<MediationData>(reader));
            }
        }
    }
}
