using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerEnd : Packet
    {
        public const uint PACKET_ID = 0x64020200;

        public AnsLayerEnd() : base(PACKET_ID) { }

        public AnsLayerEnd(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
