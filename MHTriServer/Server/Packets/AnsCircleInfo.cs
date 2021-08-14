using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleInfo : Packet
    {
        public const uint PACKET_ID = 0x65020200;

        public uint UnknownField1 { get; private set; }

        public CircleData CircleData { get; private set; }

        public UnkByteIntStruct UnknownField3 { get; private set; }

        public AnsCircleInfo(uint unknownField, CircleData circleData, UnkByteIntStruct unknownField3) : base(PACKET_ID)
            => (UnknownField1, CircleData, UnknownField3) = (unknownField, circleData, unknownField3);

        public AnsCircleInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            CircleData.Serialize(writer);
            UnknownField3.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadUInt32();
            CircleData = CompoundList.Deserialize<CircleData>(reader);
            UnknownField3 = UnkByteIntStruct.Deserialize(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField1 {UnknownField1}\n\tCircleData\n{CircleData}\n\tUnknownField3";
            str += $"\n\t    UnknownField {UnknownField3.UnknownField}";
            if (UnknownField3.ContainUnknownField3)
            {
                str += $"\n\t    UnknownField3 {UnknownField3.UnknownField3}";
            }
            return base.ToString() + str;
        }
    }
}
