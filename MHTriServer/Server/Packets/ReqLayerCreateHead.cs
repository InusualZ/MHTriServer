using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerCreateHead : Packet
    {
        public const uint PACKET_ID = 0x64110100;

        public ushort CityIndex { get; private set; }

        public ReqLayerCreateHead(ushort cityIndex) : base(PACKET_ID)
            => (CityIndex) = (cityIndex);

        public ReqLayerCreateHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
