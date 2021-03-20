using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsVulgarityLow : Packet
    {
        public const uint PACKET_ID = 0x62570200;

        public uint InfoType { get; private set; }

        public uint Offset { get; private set; }

        public uint UnknownField { get; private set; }

        public string Message { get; private set; }

        public AnsVulgarityLow(uint infoType, uint offset, uint unknownField, string termsMessage) : base(PACKET_ID) => (InfoType, Offset, UnknownField, Message) = (infoType, offset, unknownField, termsMessage);

        public AnsVulgarityLow(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(InfoType);
            writer.Write(Offset);
            writer.Write(UnknownField);
            writer.Write(Message);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size >= 14);
            InfoType = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            UnknownField = reader.ReadUInt32();
            Message = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tInfoType {InfoType}\n\tOffset {Offset}\n\tUnknownField {UnknownField}\n\tMessage {Message}";
        }
    }
}
