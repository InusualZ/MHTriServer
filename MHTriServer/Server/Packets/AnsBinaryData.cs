using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsBinaryData : Packet
    {
        public const uint PACKET_ID = 0x63030200;

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public byte[] UnknownField4 { get; private set; }

        public AnsBinaryData(uint unknownField, uint unknownField2, uint unknownField3, byte[] unknownField4) : base(PACKET_ID) 
            => (UnknownField, UnknownField2, UnknownField3, UnknownField4) = (unknownField, unknownField2, unknownField3, unknownField4);

        public AnsBinaryData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
            writer.Write(UnknownField3);

            writer.WriteShortBytes(UnknownField4);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            UnknownField3 = reader.ReadUInt32();
            UnknownField4 = reader.ReadShortBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField {UnknownField}\n\tUnknownField2 {UnknownField2}\ntUnknownField3 {UnknownField3}" +
                $"\n\tUnknownField4 {Packet.Hexstring(UnknownField4, ' ')}";
        }
    }
}
