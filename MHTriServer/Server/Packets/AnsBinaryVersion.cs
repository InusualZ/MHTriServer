

using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsBinaryVersion : Packet
    {
        public const uint PACKET_ID = 0x63010200;

        public byte BinaryType { get; private set; }

        public uint BinaryVersion { get; private set; }

        public AnsBinaryVersion(byte unknownField, uint unknownField2) : base(PACKET_ID) => (BinaryType, BinaryVersion) = (unknownField, unknownField2);

        public AnsBinaryVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(BinaryType);
            writer.Write(BinaryVersion);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 5);
            BinaryType = reader.ReadByte();
            BinaryVersion = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField {BinaryType}\n\tUnknownField2 {BinaryVersion}";
        }
    }
}
