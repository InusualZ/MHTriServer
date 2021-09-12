using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsUserSearchSet : Packet
    {
        public const uint PACKET_ID = 0x66300200;

        public AnsUserSearchSet() : base(PACKET_ID) { }

        public AnsUserSearchSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
