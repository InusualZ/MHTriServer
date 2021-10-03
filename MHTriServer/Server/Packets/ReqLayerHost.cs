using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerHost : Packet
    {
        public const uint PACKET_ID = 0x64410100;

        public UnkShortArrayStruct CityData { get; private set; }

        public ReqLayerHost(UnkShortArrayStruct cityData) : base(PACKET_ID)
            => (CityData) = (cityData);

        public ReqLayerHost(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            CityData.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            CityData = UnkShortArrayStruct.Deserialize(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerHost(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tCityData\n\t  UnknownField1 {CityData.UnknownField}\n\t  UnknownField2 {CityData.UnknownField2}\n\t  UnknownField3 ({CityData.UnknownField3.Count})";
            for (var i = 0; i < CityData.UnknownField3.Count; ++i)
            {
                str += $"\n\t    [{i}] => {CityData.UnknownField3[i]}";
            }
            return base.ToString() + str;
        }
    }
}
