using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerCreateSet : Packet
    {
        public const uint PACKET_ID = 0x64120100;

        public ushort CityIndex { get; private set; }

        public LayerDownData UnknownField2 { get; private set; }

        public CompoundList UnknownField3 { get; private set; }

        public ReqLayerCreateSet(ushort cityIndex, LayerDownData unknownField2, CompoundList unknownField3) : base(PACKET_ID)
            => (CityIndex, UnknownField2, UnknownField3) = (cityIndex, unknownField2, unknownField3);

        public ReqLayerCreateSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex + 1);
            UnknownField2.Serialize(writer);
            UnknownField3.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            CityIndex = reader.ReadUInt16();
            --CityIndex;

            UnknownField2 =  CompoundList.Deserialize<LayerDownData>(reader);
            UnknownField3 = CompoundList.Deserialize<CompoundList>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCityIndex {CityIndex}\n\tUnknownField2 {UnknownField2}";
        }
    }
}
