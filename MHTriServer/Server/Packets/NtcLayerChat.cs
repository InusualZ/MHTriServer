using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerChat : Packet
    {
        public const uint PACKET_ID = 0x64721000;

        public byte UnknownField1 { get; private set; }

        public MessageData Properties { get; private set; }

        public string Message { get; private set; }

        public NtcLayerChat(byte unknownField1, MessageData properties, string message) : base(PACKET_ID)
            => (UnknownField1, Properties, Message) = (unknownField1, properties, message);

        public NtcLayerChat(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(UnknownField1);
            Properties.Serialize(writer);
            writer.Write(Message);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadByte();
            Properties = CompoundList.Deserialize<MessageData>(reader);
            Message = reader.ReadString();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession)
        {
            handler.HandleNtcLayerChat(networkSession, this);
        }

        public override string ToString() 
            => base.ToString() + $"\n\tUnknownField1 {UnknownField1}\n\tProperties\n{Properties}\n\tMessage '{Message}'";
    }
}
