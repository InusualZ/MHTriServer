using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerCreateFoot : Packet
    {
        public const uint PACKET_ID = 0x64130200;

        public ushort CityIndex { get; private set; }

        public AnsLayerCreateFoot(ushort cityIndex) : base(PACKET_ID)
            => (CityIndex) = (cityIndex);

        public AnsLayerCreateFoot(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex + 1);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 2);

            CityIndex = reader.ReadUInt16();

            --CityIndex;
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCityIndex {CityIndex}";
        }
    }
}
