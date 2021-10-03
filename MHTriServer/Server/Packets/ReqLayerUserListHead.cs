
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerUserListHead : Packet
    {
        public const uint PACKET_ID = 0x64640100;

        public byte UnknownField1 { get; private set; }

        public UnkShortArrayStruct CityData { get; private set; }

        public uint Offset { get; private set; }

        public uint MaxElementCount { get; private set; }

        public byte[] Format { get; private set;  }

        public ReqLayerUserListHead(byte unknownField1, UnkShortArrayStruct unknownField2, uint unknownField3, uint unknownField4, byte[] format1) : base(PACKET_ID)
            => (UnknownField1, CityData, Offset, MaxElementCount, Format) = (unknownField1, unknownField2, unknownField3, unknownField4, format1);

        public ReqLayerUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            CityData.Serialize(writer);
            writer.Write(Offset);
            writer.Write(MaxElementCount);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadByte();
            CityData = UnkShortArrayStruct.Deserialize(reader);
            Offset = reader.ReadUInt32();
            MaxElementCount = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerUserListHead(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tUnknownField1 {UnknownField1}\n\tCityData\n\t  UnknownField1 {CityData.UnknownField}\n\t  UnknownField2 {CityData.UnknownField2}\n\t  UnknownField3 ({CityData.UnknownField3.Count})";
            for (var i = 0; i < CityData.UnknownField3.Count; ++i)
            {
                str += $"\n\t    [{i}] => {CityData.UnknownField3[i]}";
            }
            str += $"\n\tOffset {Offset}\n\tMaxElementCount {MaxElementCount}\n\tFormat `{Hexstring(Format, ' ')}`";
            return base.ToString() + str;
        }
    }
}
