using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqTermsVersion : Packet
    {
        public const uint PACKET_ID = 0x62100100;

        public ReqTermsVersion() : base(PACKET_ID) { }

        public ReqTermsVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
