using System.Collections.Generic;
using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerCreateSet : Packet
    {
        public const uint PACKET_ID = 0x64120100;

        public ushort CityIndex { get; private set; }

        public LayerDownData UnknownField2 { get; private set; }

        public List<UnkByteIntStruct> UnknownField3 { get; private set; }

        public ReqLayerCreateSet(ushort cityIndex, LayerDownData unknownField2, List<UnkByteIntStruct> unknownField3) : base(PACKET_ID)
            => (CityIndex, UnknownField2, UnknownField3) = (cityIndex, unknownField2, unknownField3);

        public ReqLayerCreateSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex + 1);
            UnknownField2.Serialize(writer);
            UnkByteIntStruct.SerializeArray(UnknownField3, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            CityIndex = reader.ReadUInt16();
            --CityIndex;

            UnknownField2 =  CompoundList.Deserialize<LayerDownData>(reader);
            UnknownField3 = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerCreateSet(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tCityIndex {CityIndex}\n\tUnknownField2 {UnknownField2}\n\tUnknownField3({UnknownField3.Count})";
            for (var i = 0; i < UnknownField3.Count; ++i)
            {
                var data = UnknownField3[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField}";
                if (data.ContainUnknownField3)
                {
                    str += $"\n\t    UnknownField3 {data.UnknownField3}";
                }
            }

            return base.ToString() + str;
        }
    }
}
