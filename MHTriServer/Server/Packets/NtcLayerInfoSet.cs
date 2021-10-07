using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerInfoSet : Packet
    {
        public const uint PACKET_ID = 0x64201000;

        public UnkShortArrayStruct UnknownField1 { get; private set; }

        public LayerData ChildInfo { get; private set; }

        public List<UnkByteIntStruct> UnknownField3 { get; private set; }

        public NtcLayerInfoSet(UnkShortArrayStruct unknownField1, LayerData layerData, List<UnkByteIntStruct> unknownField3) : base(PACKET_ID)
            => (UnknownField1, ChildInfo, UnknownField3) = (unknownField1, layerData, unknownField3);

        public NtcLayerInfoSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            UnknownField1.Serialize(writer);
            ChildInfo.Serialize(writer);
            UnkByteIntStruct.SerializeArray(UnknownField3, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = UnkShortArrayStruct.Deserialize(reader);
            ChildInfo = CompoundList.Deserialize<LayerData>(reader);
            UnknownField3 = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField1 {UnknownField1}\n\tChildInfo\n\t{ChildInfo})\n\tUnknownField3({UnknownField3.Count})";
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
