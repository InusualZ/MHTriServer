using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerChildListData : Packet
    {
        public const uint PACKET_ID = 0x64250100;

        public uint Offset { get; private set; }

        public uint ExpectedDataCount { get; private set; }

        public ReqLayerChildListData(uint unknownField, uint unknownField2) : base(PACKET_ID)
            => (Offset, ExpectedDataCount) = (unknownField, unknownField2);

        public ReqLayerChildListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Offset);
            writer.Write(ExpectedDataCount);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);
            Offset = reader.ReadUInt32();
            ExpectedDataCount = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerChildListData(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tOffset {Offset}\n\tExpectedDataCount {ExpectedDataCount}";
        }
    }
}
