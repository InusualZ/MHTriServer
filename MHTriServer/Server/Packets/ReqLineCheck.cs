using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLineCheck : Packet
    {
        public const uint PACKET_ID = 0x60010100;

        public ReqLineCheck() : base(PACKET_ID) { }

        public ReqLineCheck(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
