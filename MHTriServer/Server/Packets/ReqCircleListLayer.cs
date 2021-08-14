using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqCircleListLayer : Packet
    {
        public const uint PACKET_ID = 0x65270100;

        public class ReqData
        {
            public byte UnknownField { get; internal set; }

            public byte UnknownField2 { get; internal set; }
        }

        public byte[] Format { get; private set; }

        public ReqData[] UnknownField2 { get; private set; }

        public ReqCircleListLayer(byte[] format, ReqData[] data) : base(PACKET_ID)
            => (Format, UnknownField2) = (format, data);

        public ReqCircleListLayer(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteByteBytes(Format);

            writer.Write((byte)UnknownField2.Length);
            foreach (var data in UnknownField2)
            {
                writer.Write(data.UnknownField);
                writer.Write(data.UnknownField2);
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Format = reader.ReadByteBytes();
            var count = reader.ReadByte();
            UnknownField2 = new ReqData[count];
            for (var i = 0; i < count; ++i)
            {
                UnknownField2[i] = new ReqData()
                {
                    UnknownField = reader.ReadByte(),
                    UnknownField2 = reader.ReadByte()
                };
            }
        }

        public override string ToString()
        {
            var str = $":\n\tFormat '{Packet.Hexstring(Format, ' ')}'\n\tUnknownField2({UnknownField2.Length})";
            for (var i = 0; i < UnknownField2.Length; ++i)
            {
                var data = UnknownField2[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField}\n\t    UnknownField2 {data.UnknownField2}";
            }
            return base.ToString() +  str;
        }
    }
}
