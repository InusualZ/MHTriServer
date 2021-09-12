using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerDetailSearchHead : Packet
    {
        public const uint PACKET_ID = 0x64900100;

        public bool UnknownField1 { get; private set; }

        public uint UnknownField2 { get; private set; }

        public List<Unk3Byte1Int> UnknownField3 { get; private set; }

        public uint UnknownField4 { get; private set; }

        public uint UnknownField5 { get; private set; }

        public byte[] Format1 { get; private set; }

        public byte[] Format2 { get; private set; }

        public ReqLayerDetailSearchHead(bool unknownField1, uint unknownField2, List<Unk3Byte1Int> unknownField3, uint unknownField4, 
            uint unkownField5, byte[] format1, byte[] format2) : base(PACKET_ID)
            => (UnknownField1, UnknownField2, UnknownField3, UnknownField4, UnknownField5, Format1, Format2) = (unknownField1, unknownField2, 
            unknownField3, unknownField4, UnknownField5, format1, format2);

        public ReqLayerDetailSearchHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);

            writer.Write(UnknownField2);
            Unk3Byte1Int.SerializeArray(UnknownField3, writer);

            writer.Write(UnknownField4);
            writer.Write(UnknownField5);

            writer.WriteByteBytes(Format1);
            writer.WriteByteBytes(Format2);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadBoolean();

            UnknownField2 = reader.ReadUInt32();
            UnknownField3 = Unk3Byte1Int.DeserializeArray(reader);

            UnknownField4 = reader.ReadUInt32();
            UnknownField5 = reader.ReadUInt32();

            Format1 = reader.ReadByteBytes();
            Format2 = reader.ReadByteBytes();
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField1 {UnknownField1}\n\tUnknownField2 {UnknownField2}\n\tUnknownField3({UnknownField3.Count})";
            for (var i = 0; i < UnknownField3.Count; ++i)
            {
                var data = UnknownField3[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField1 {data.UnknownField1}\n\t    UnknownField2 {data.UnknownField2}";
                if (data.ContainUnknownField4)
                {
                    str += $"\n\t UnknownField4 { data.UnknownField4}";
                }
            }

            str += $"\n\tUnknownField4 {UnknownField4}\n\tUnknownField5 {UnknownField5}\n\tFormat1 `{Hexstring(Format1, ' ')}`\n\tFormat2 `{Format2,' '}`";
            return base.ToString() + str;
        }
    }
}
