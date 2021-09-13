using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqBinaryHead : Packet
    {
        public const uint PACKET_ID = 0x63020100;

        public byte BinaryType { get; private set; }

        public ReqBinaryHead(byte binaryType) : base(PACKET_ID) => BinaryType = binaryType;

        public ReqBinaryHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(BinaryType);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);

            BinaryType = reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tBinaryType {BinaryType}";
        }
    }
}
