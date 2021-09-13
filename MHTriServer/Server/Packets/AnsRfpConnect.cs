using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsRfpConnect : Packet
    {
        public const uint PACKET_ID = 0x61400200;

        public string Address { get; set; }

        public ushort Port { get; set; }

        public AnsRfpConnect(string address, ushort port) : base(PACKET_ID) => (Address, Port) = (address, port);

        public AnsRfpConnect(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Address);
            writer.Write(Port);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Address = reader.ReadString();
            Port = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tAddress {Address}\n\tPort {Port}";
        }
    }
}
