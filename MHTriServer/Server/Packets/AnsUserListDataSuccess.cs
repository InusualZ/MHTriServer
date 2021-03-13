using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    /*
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
    class AnsLoginInfoSucess : Packet
    {
        public const uint PACKET_ID = 0x60300100;

        public AnsLoginInfoSucess() : base(PACKET_ID) { }

        public AnsLoginInfoSucess(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
