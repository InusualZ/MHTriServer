using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerChildInfo : Packet
    {
        public const uint PACKET_ID = 0x64230200;

        public ushort UnknownField { get; private set; }

        public LayerData UnknownField2 { get; private set; }

        public List<UnkByteIntStruct> UnknownField3 { get; private set; }

        public AnsLayerChildInfo(ushort unknownField, LayerData layerData, List<UnkByteIntStruct> unknownField3) : base(PACKET_ID)
            => (UnknownField, UnknownField2, UnknownField3) = (unknownField, layerData, unknownField3);

        public AnsLayerChildInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            UnknownField2.Serialize(writer);

            UnkByteIntStruct.SerializeArray(UnknownField3, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt16();
            UnknownField2 = CompoundList.Deserialize<LayerData>(reader);
            UnknownField3 = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField {UnknownField}\n\tUnknownField2\n\t{UnknownField2})\n\tUnknownField3({UnknownField3.Count})";
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
