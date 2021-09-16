using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqCircleInfoNoticeSet : Packet
    {
        public const uint PACKET_ID = 0x65800100;

        public byte[] Format { get; private set; }

        public byte[] Format2 { get; private set; }

        public ReqCircleInfoNoticeSet() : base(PACKET_ID) { }

        public ReqCircleInfoNoticeSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteByteBytes(Format);
            writer.WriteByteBytes(Format2);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Format = reader.ReadByteBytes();
            Format2 = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqCircleInfoNoticeSet(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tFormat {Packet.Hexstring(Format, ' ')}\n\tFormat2 {Packet.Hexstring(Format2, ' ')}";
        }
    }
}
