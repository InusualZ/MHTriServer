using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerChildInfo : Packet
    {
        public const uint PACKET_ID = 0x64230100;

        public short Index { get; private set; }

        public byte[] Format { get; private set; }

        public List<Unk2ByteArray> UnknownField2 { get; private set; }

        public ReqLayerChildInfo(short index, byte[] format, List<Unk2ByteArray> data) : base(PACKET_ID)
            => (Index, Format, UnknownField2) = (index, format, data);

        public ReqLayerChildInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Index);
            writer.WriteByteBytes(Format);

            Unk2ByteArray.SerializeArray(UnknownField2, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Index = reader.ReadInt16();

            Format = reader.ReadByteBytes();
            UnknownField2 = Unk2ByteArray.DeserializeArray(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerChildInfo(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tIndex {Index}\n\tFormat '{Packet.Hexstring(Format, ' ')}'\n\tUnknownField2({UnknownField2.Count})";
            for (var i = 0; i < UnknownField2.Count; ++i)
            {
                var data = UnknownField2[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField1}\n\t    UnknownField2 {data.UnknownField2}";
            }
            return base.ToString() +  str;
        }
    }
}
