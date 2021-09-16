using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsConnection : Packet
    {
        public const uint PACKET_ID = 0x60200200;

        public ConnectionData Data { get; private set; }

        public AnsConnection(ConnectionData connectionData) : base(PACKET_ID) => Data = connectionData;

        public AnsConnection(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Data = CompoundList.Deserialize<ConnectionData>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleAnsConnection(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + ":\n\tData\n" + Data.ToString();
        }
    }
}