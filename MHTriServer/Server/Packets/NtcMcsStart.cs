using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcMcsStart : Packet
    {
        public const uint PACKET_ID = 0x65911000;

        public string Address { get; set; }

        public ushort Port { get; set; }

        // Doubtful
        public string Name { get; set; }

        public NtcMcsStart(string address, ushort port, string name) : base(PACKET_ID) => (Address, Port, Name) = (address, port, name);

        public NtcMcsStart(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Address);
            writer.Write(Port);
            writer.Write(Name);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Address = reader.ReadString();
            Port = reader.ReadUInt16();
            Name = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tAddress {Address}\n\tPort {Port}\n\tName {Name}";
        }
    }
}
