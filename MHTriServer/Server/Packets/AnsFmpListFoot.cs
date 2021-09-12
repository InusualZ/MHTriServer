using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsFmpListFoot : Packet
    {
        public const uint PACKET_ID = 0x61330200;
        public const uint PACKET_ID_FMP = 0x63130200;

        public AnsFmpListFoot() : base(PACKET_ID) { }

        public AnsFmpListFoot(bool isServerFmp) : base(PACKET_ID_FMP) { }

        public AnsFmpListFoot(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
