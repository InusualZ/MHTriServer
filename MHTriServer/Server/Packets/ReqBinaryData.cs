using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqBinaryData : Packet
    {
        public const uint PACKET_ID = 0x63030100;

        public byte BinaryType { get; private set; }

        public uint UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public uint BinaryDataExpectedSize { get; private set; }

        public ReqBinaryData(byte unknownField, uint unknownField2, uint unknownField3, uint unknownField4) : base(PACKET_ID) 
            => (BinaryType, UnknownField2, UnknownField3, BinaryDataExpectedSize) = (unknownField, unknownField2, unknownField3, unknownField4);

        public ReqBinaryData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(BinaryType);
            writer.Write(UnknownField2);
            writer.Write(UnknownField3);
            writer.Write(BinaryDataExpectedSize);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 13);
            BinaryType = reader.ReadByte();
            UnknownField2 = reader.ReadUInt32();
            UnknownField3 = reader.ReadUInt32();
            BinaryDataExpectedSize = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tBinaryType {BinaryType}\n\tUnknownField2 {UnknownField2}\n\tUnknownField3 {UnknownField3}" +
                $"\n\tUnknownField4 {BinaryDataExpectedSize}";
        }
    }
}
