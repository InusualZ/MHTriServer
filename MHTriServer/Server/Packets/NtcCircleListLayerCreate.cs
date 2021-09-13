using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcCircleListLayerCreate : Packet
    {
        public const uint PACKET_ID = 0x65811000;

        public uint CircleIndex { get; private set; }

        public CircleData CircleData { get; private set; }

        public List<UnkByteIntStruct> ExtraProperties { get; private set; }

        public NtcCircleListLayerCreate(uint unknownField, CircleData circleData, List<UnkByteIntStruct> unknownField3) : base(PACKET_ID)
            => (CircleIndex, CircleData, ExtraProperties) = (unknownField, circleData, unknownField3);

        public NtcCircleListLayerCreate(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CircleIndex);
            CircleData.Serialize(writer);
            UnkByteIntStruct.SerializeArray(ExtraProperties, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            CircleIndex = reader.ReadUInt32();
            CircleData = CompoundList.Deserialize<CircleData>(reader);
            ExtraProperties = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tCircleIndex {CircleIndex}\n\tCircleData\n{CircleData}\n\tExtraProperties({ExtraProperties.Count})";
            for (var i = 0; i < ExtraProperties.Count; ++i)
            {
                var data = ExtraProperties[i];
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
