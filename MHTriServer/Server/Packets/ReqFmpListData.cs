
using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqFmpListData : Packet
    {
        public const uint PACKET_ID = 0x61320100;
        public const uint PACKET_ID_FMP = 0x63120100;

        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        public ReqFmpListData(uint unknownField, uint unknownField2) : base(PACKET_ID) 
            => (UnknownField, UnknownField2) = (unknownField, unknownField2);

        public ReqFmpListData(uint unknownField, uint unknownField2, bool isServerFmp) : base(PACKET_ID_FMP) 
            => (UnknownField, UnknownField2) = (unknownField, unknownField2);

        public ReqFmpListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID || ID == PACKET_ID_FMP);
            Debug.Assert(Size == 8);

            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqFmpListData(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tUnknownField2 {UnknownField2}";
        }
    }
}
