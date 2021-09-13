using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    /*
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
    class ReqTicketClient : Packet
    {
        public const uint PACKET_ID = 0x60300100;

        public ReqTicketClient() : base(PACKET_ID) { }

        public ReqTicketClient(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
