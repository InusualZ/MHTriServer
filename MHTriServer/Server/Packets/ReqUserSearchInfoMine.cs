using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqUserSearchInfoMine : Packet
    {
        public const uint PACKET_ID = 0x66370100;

        public CompoundList UnknownField { get; private set; }

        public ReqUserSearchInfoMine(CompoundList unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqUserSearchInfoMine(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            UnknownField.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = CompoundList.Deserialize<FmpData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
