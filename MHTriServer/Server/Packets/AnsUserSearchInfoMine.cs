using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsUserSearchInfoMine : Packet
    {
        public const uint PACKET_ID = 0x66370200;

        public CompoundList UnknownField { get; private set; }

        public AnsUserSearchInfoMine(CompoundList unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public AnsUserSearchInfoMine(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
            return base.ToString() + $":\n\tUnknownField\n{UnknownField}";
        }
    }
}
