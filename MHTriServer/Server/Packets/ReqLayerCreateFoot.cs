using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerCreateFoot : Packet
    {
        public const uint PACKET_ID = 0x64130100;

        public ushort CityIndex { get; private set; }

        public byte UnknownField2 { get; private set; }

        public ReqLayerCreateFoot(ushort cityIndex, byte unknownField2) : base(PACKET_ID)
            => (CityIndex, UnknownField2) = (cityIndex, unknownField2);

        public ReqLayerCreateFoot(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex + 1);
            writer.Write(UnknownField2);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 3);

            CityIndex = reader.ReadUInt16();
            UnknownField2 = reader.ReadByte();

            --CityIndex;
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCityIndex {CityIndex}\n\tUnknownField2 {UnknownField2}";
        }
    }
}
