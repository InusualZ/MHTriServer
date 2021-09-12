using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class AnsTicketClient : Packet
    {
        public const uint PACKET_ID = 0x60300200;

        public byte[] Ticket { get; private set; }

        // TODO: Remove asap
        public AnsTicketClient(string ticket) : this(Encoding.ASCII.GetBytes(ticket)) { }

        public AnsTicketClient(byte[] ticket) : base(PACKET_ID)
        {
            Debug.Assert(ticket.Length <= 0x400);
            Ticket = ticket;
        }

        public AnsTicketClient(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteShortBytes(Ticket);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            Ticket = reader.ReadShortBytes();
        }
    }
}
