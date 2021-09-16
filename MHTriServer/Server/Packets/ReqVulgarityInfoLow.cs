using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqVulgarityInfoLow : Packet
    {
        public const uint PACKET_ID = 0x62560100;

        public uint UnknownField { get; private set; }

        public ReqVulgarityInfoLow(uint unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqVulgarityInfoLow(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            UnknownField = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqVulgarityInfoLow(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
