using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerCreateHead : Packet
    {
        public const uint PACKET_ID = 0x64110200;

        public ushort CityIndex { get; private set; }

        public AnsLayerCreateHead(ushort cityIndex) : base(PACKET_ID)
            => (CityIndex) = (cityIndex);

        public AnsLayerCreateHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 2);

            CityIndex = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCityIndex {CityIndex}";
        }
    }
}
