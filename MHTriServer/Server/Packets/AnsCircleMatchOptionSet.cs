using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleMatchOptionSet : Packet
    {
        public const uint PACKET_ID = 0x65100200;

        public AnsCircleMatchOptionSet() : base(PACKET_ID) { }

        public AnsCircleMatchOptionSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
        }
    }
}
