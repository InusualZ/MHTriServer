using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerUserNum : Packet
    {
        public const uint PACKET_ID = 0x64031000;

        public byte UnknownField { get; private set; }

        public UserNumData UnknownField2 { get; private set; }

        public NtcLayerUserNum(byte unknownField, UserNumData unknownField2) : base(PACKET_ID) => (UnknownField, UnknownField2) = (unknownField, unknownField2);

        public NtcLayerUserNum(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            UnknownField2.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            UnknownField = reader.ReadByte();
            UnknownField2 = CompoundList.Deserialize<UserNumData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tUnknownField2\n{UnknownField2}";
        }
    }
}
