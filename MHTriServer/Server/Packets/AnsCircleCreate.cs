using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleCreate : Packet
    {
        public const uint PACKET_ID = 0x65010200;

        public uint CircleIndex { get; private set; }

        public AnsCircleCreate(uint circleIndex) : base(PACKET_ID)
            => (CircleIndex) = (circleIndex);

        public AnsCircleCreate(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CircleIndex);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            CircleIndex = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tCircleIndex {CircleIndex}";
        }
    }
}
