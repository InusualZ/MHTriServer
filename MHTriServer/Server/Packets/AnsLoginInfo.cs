using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLoginInfo : Packet
    {
        public const uint PACKET_ID = 0x61010200;

        public byte UnknownByte { get; private set; }

        public string UnknownString { get; private set; }

        public ChargeInfo Data { get; private set; }

        public AnsLoginInfo(byte unknownByte, string unknownString, ChargeInfo chargeInfo) : base(PACKET_ID) 
            => (UnknownByte, UnknownString, Data) = (unknownByte, unknownString, chargeInfo);

        public AnsLoginInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownByte);
            writer.Write(UnknownString);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownByte = reader.ReadByte();
            UnknownString = reader.ReadString();
            Data = CompoundList.Deserialize<ChargeInfo>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownByte {UnknownByte}\n\tUnknownString {UnknownString}\n\tChargeInfo\n\t{Data}";
        }
    }
}
