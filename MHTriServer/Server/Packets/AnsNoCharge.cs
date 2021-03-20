using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    class AnsNoCharge : Packet
    {
        public const uint PACKET_ID = 0x62310200;

        public string Message { get; set; }

        public AnsNoCharge(string message) : base(PACKET_ID) => Message = message;

        public AnsNoCharge(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Message);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Message = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tMessage {Message}";
        }
    }
}
