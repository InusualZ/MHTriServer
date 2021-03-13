using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class AnsTicket : Packet
    {
        public const uint PACKET_ID = 0x60300200;

        public byte[] Ticket { get; private set; }

        // TODO: Remove asap
        public AnsTicket(string ticket) : this(Encoding.ASCII.GetBytes(ticket)) { }

        public AnsTicket(byte[] ticket) : base(PACKET_ID)
        {
            Debug.Assert(ticket.Length <= 0x400);
            Ticket = ticket;
        }

        public AnsTicket(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Ticket);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            var ticketLength = reader.ReadUInt16();
            Ticket = reader.ReadBytes(ticketLength);
        }
    }
}
