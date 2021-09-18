using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcCircleBinary : Packet
    {
        public const uint PACKET_ID = 0x65701000;

        public uint CircleIndex { get; private set; }

        // Capcom ID
        public string HunterID { get; private set; }

        public NtcBinaryCompoundData UnknownField2 { get; private set; }

        public ushort UnknownField3 { get; private set; }

        public CompoundExtendedList UnknownField4 { get; private set; }

        public NtcCircleBinary(string hunterID, NtcBinaryCompoundData unknownField2, ushort unknownField3, CompoundExtendedList unknownField4) : base(PACKET_ID)
            => (HunterID, UnknownField2, UnknownField3, UnknownField4) = (hunterID, unknownField2, unknownField3, unknownField4);

        public NtcCircleBinary(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(HunterID));

            base.Serialize(writer);

            writer.Write(HunterID);
            UnknownField2.Serialize(writer);
            writer.Write(UnknownField3);
            UnknownField4.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            CircleIndex = reader.ReadUInt32();
            UnknownField2 = CompoundList.Deserialize<NtcBinaryCompoundData>(reader);
            UnknownField3 = reader.ReadUInt16();
            UnknownField4 = CompoundExtendedList.Deserialize<CompoundExtendedList>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tCircleIndex {CircleIndex}\n\tUnknownField2\n{UnknownField2}\n\tUnknownField3 {UnknownField3}\n\tUnknownField4\n{UnknownField4}";
        }
    }
}
