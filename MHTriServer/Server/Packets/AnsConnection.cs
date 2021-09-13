using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsConnection : Packet
    {
        public const uint PACKET_ID = 0x60200200;

        public ConnectionData Data { get; private set; }

        public AnsConnection(ConnectionData connectionData) : base(PACKET_ID) => Data = connectionData;

        public AnsConnection(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Deserialize(BEBinaryReader reader)
        {
            Data = CompoundList.Deserialize<ConnectionData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + ":\n\tData\n" + Data.ToString();
        }
    }
}