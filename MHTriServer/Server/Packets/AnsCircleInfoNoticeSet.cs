using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleInfoNoticeSet : Packet
    {
        public const uint PACKET_ID = 0x65800200;

        public AnsCircleInfoNoticeSet() : base(PACKET_ID) { }

        public AnsCircleInfoNoticeSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
