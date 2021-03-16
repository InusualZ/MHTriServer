using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsBinaryData : Packet
    {
        public const uint PACKET_ID = 0x63030200;

        public uint BinaryDataVersion { get; private set; }

        public uint UnknownField2 { get; private set; }

        public uint BinarySize { get; private set; }

        public byte[] BinaryData { get; private set; }

        public AnsBinaryData(uint binaryDataVersion, uint unknownField2, uint binarySize, byte[] binaryData) : base(PACKET_ID) 
            => (BinaryDataVersion, UnknownField2, BinarySize, BinaryData) = (binaryDataVersion, unknownField2, binarySize, binaryData);

        public AnsBinaryData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(BinaryDataVersion);
            writer.Write(UnknownField2);
            writer.Write(BinarySize);

            writer.WriteShortBytes(BinaryData);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            BinaryDataVersion = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
            BinarySize = reader.ReadUInt32();
            BinaryData = reader.ReadShortBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tBinaryDataVersion {BinaryDataVersion}\n\tUnknownField2 {UnknownField2}\n\tBinarySize {BinarySize}" +
                $"\n\tUnknownField4 {Packet.Hexstring(BinaryData, ' ')}";
        }
    }
}
