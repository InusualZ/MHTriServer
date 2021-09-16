using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerUserList : Packet
    {
        public const uint PACKET_ID = 0x64630100;

        public byte[] Format { get; private set; }

        public ReqLayerUserList(byte[] format) : base(PACKET_ID) 
            => (Format) = (format);

        public ReqLayerUserList(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Format = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerUserList(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
