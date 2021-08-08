using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerCreateHead : Packet
    {
        public const uint PACKET_ID = 0x64110200;

        public ushort CityIndex { get; private set; }

        public AnsLayerCreateHead(ushort cityIndex) : base(PACKET_ID)
            => (CityIndex) = (cityIndex);

        public AnsLayerCreateHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex + 1);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
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
