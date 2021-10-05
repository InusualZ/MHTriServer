using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerUp : Packet
    {
        public const uint PACKET_ID = 0x64150200;

        public AnsLayerUp() : base(PACKET_ID) { }

        public AnsLayerUp(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
        }
    }
}
