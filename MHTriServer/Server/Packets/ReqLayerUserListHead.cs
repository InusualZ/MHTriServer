
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerUserListHead : Packet
    {
        public const uint PACKET_ID = 0x64640100;

        public byte UnknownField1 { get; private set; }

        public UnkShortArrayStruct UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public uint UnknownField4 { get; private set; }

        public byte[] Format1 { get; private set;  }

        public ReqLayerUserListHead(byte unknownField1, UnkShortArrayStruct unknownField2, uint unknownField3, uint unknownField4, byte[] format1) : base(PACKET_ID)
            => (UnknownField1, UnknownField2, UnknownField3, UnknownField4, Format1) = (unknownField1, unknownField2, unknownField3, unknownField4, format1);

        public ReqLayerUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            UnknownField2.Serialize(writer);
            writer.Write(UnknownField3);
            writer.Write(UnknownField4);
            writer.WriteByteBytes(Format1);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadByte();
            UnknownField2 = UnkShortArrayStruct.Deserialize(reader);
            UnknownField3 = reader.ReadUInt32();
            UnknownField4 = reader.ReadUInt32();
            Format1 = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerUserListHead(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tUnknownField1 {UnknownField1}\n\tUnknownField2\n\t  UnknownField1 {UnknownField2.UnknownField}\n\t  UnknownField2 {UnknownField2.UnknownField2}\n\t  UnknownField3{UnknownField2.UnknownField3.Count}";
            for (var i = 0; i < UnknownField2.UnknownField3.Count; ++i)
            {
                str += $"\n\t    [{i}] =>{UnknownField2.UnknownField3[i]}";
            }
            str += $"\n\tUnknownField3 {UnknownField3}\n\tUnknownField4 {UnknownField4}\n\tFormat1 `{Hexstring(Format1, ' ')}`";
            return base.ToString() + str;
        }
    }
}
