using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerHost : Packet
    {
        public const uint PACKET_ID = 0x64411000;

        public UnkShortArrayStruct CityData { get; private set;  }

        public string LeaderCapcomID { get; private set; }

        public string LeaderName { get; private set; }

        public NtcLayerHost(UnkShortArrayStruct cityData, string leaderCapcomID, string leaderName) : base(PACKET_ID)
            => (CityData, LeaderCapcomID, LeaderName) = (cityData, leaderCapcomID, leaderName);

        public NtcLayerHost(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            CityData.Serialize(writer);
            writer.Write(LeaderCapcomID);
            writer.Write(LeaderName);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            CityData = UnkShortArrayStruct.Deserialize(reader);
            LeaderCapcomID = reader.ReadString();
            LeaderName = reader.ReadString();
        }

        public override string ToString()
        {
            var str = $":\n\tCityData\n\t  UnknownField1 {CityData.UnknownField}\n\t  UnknownField2 {CityData.UnknownField2}\n\t  UnknownField3 ({CityData.UnknownField3.Count})";
            for (var i = 0; i < CityData.UnknownField3.Count; ++i)
            {
                str += $"\n\t    [{i}] => {CityData.UnknownField3[i]}";
            }
            str += $"\n\tLeaderCapcomID {LeaderCapcomID}\n\tLeaderName {LeaderName}";
            return base.ToString() + str;
        }
    }
}
