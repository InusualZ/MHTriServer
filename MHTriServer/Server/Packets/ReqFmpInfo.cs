using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqFmpInfo : Packet
    {
        public const uint PACKET_ID = 0x61340100;

        public const uint PACKET_ID_FMP = 0x63140100;

        public uint SelectedFmpIndex { get; private set; }

        public byte[] Format { get; private set; }

        public ReqFmpInfo(uint unknownField, byte[] format) : base(PACKET_ID) 
            => (SelectedFmpIndex, Format) = (unknownField, format);

        public ReqFmpInfo(uint unknownField, byte[] format, bool isServerFmp) : base(PACKET_ID_FMP)
            => (SelectedFmpIndex, Format) = (unknownField, format);

        public ReqFmpInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SelectedFmpIndex);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || ID == PACKET_ID_FMP);
            SelectedFmpIndex = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqFmpInfo(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tSelectedFmpIndex {SelectedFmpIndex}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
