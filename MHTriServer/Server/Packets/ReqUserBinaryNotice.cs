using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserBinaryNotice : Packet
    {
        public const uint PACKET_ID = 0x66320100;

        public byte UnknownField1 { get; private set; }

        public string UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public uint UnknownField4 { get; private set; }

        public ReqUserBinaryNotice(byte unknownField1, string unknownField2, uint unknownField3, uint unknownField4) : base(PACKET_ID)
            => (UnknownField1, UnknownField2, UnknownField3, UnknownField4) = (unknownField1, unknownField2, unknownField3, unknownField4);

        public ReqUserBinaryNotice(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
            writer.Write(UnknownField3);
            writer.Write(UnknownField4);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size >= 11);
            UnknownField1 = reader.ReadByte();
            UnknownField2 = reader.ReadString();
            UnknownField3 = reader.ReadUInt32();
            UnknownField4 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField1 {UnknownField1}\n\tUnknownField2 \"{UnknownField2}\"\n\tUnknownField3 {UnknownField3}" +
                $"\n\tUnknownField4 {UnknownField4}";
        }
    }
}
