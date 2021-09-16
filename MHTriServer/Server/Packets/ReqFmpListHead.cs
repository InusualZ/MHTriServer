using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqFmpListHead : Packet
    {
        public const uint PACKET_ID = 0x61310100;
        public const uint PACKET_ID_FMP = 0x63110100;

        public uint FmpListVersion { get; private set; }

        // Seems to be always 1
        public uint UnknownField { get; private set; }

        public uint MaxServerCount { get; private set; }

        public byte[] Format { get; private set; }

        public ReqFmpListHead() : base(PACKET_ID) { }

        public ReqFmpListHead(bool isServerFmp) : base(PACKET_ID_FMP) { }

        public ReqFmpListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(FmpListVersion);
            writer.Write(UnknownField);
            writer.Write(MaxServerCount);
            writer.WriteByteBytes(Format);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || ID == PACKET_ID_FMP);
            FmpListVersion = reader.ReadUInt32();
            UnknownField = reader.ReadUInt32();
            MaxServerCount = reader.ReadUInt32();
            Format = reader.ReadByteBytes();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqFmpListHead(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tFmpListVersion {FmpListVersion}\n\tUnknownField {UnknownField}" +
                $"\n\tMaxServerCount {MaxServerCount}\n\tFormat '{Packet.Hexstring(Format, ' ')}'";
        }
    }
}
