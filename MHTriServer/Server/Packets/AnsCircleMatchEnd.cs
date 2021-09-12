using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleMatchEnd : Packet
    {
        public const uint PACKET_ID = 0x65130200;

        public AnsCircleMatchEnd() : base(PACKET_ID) { }

        public AnsCircleMatchEnd(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
        }
    }
}
