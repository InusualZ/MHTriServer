using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    /// <summary>
    ///  Send cheat check data
    /// </summary>
    public class ReqCheatDataCheck : Packet
    {
        public class UnknownCheckData : CompoundList
        {
            private const byte FIELD_1 = 0x01;
            private const byte FIELD_2 = 0x02;

            public byte UnknownField1 { get => Get<byte>(FIELD_1); set => Set(FIELD_1, value); }

            public byte UnknownField2 { get => Get<byte>(FIELD_2); set => Set(FIELD_2, value); }
        }

        public const uint PACKET_ID = 0x60801000;

        public byte[] Format { get; private set; }

        public UnknownCheckData CheckData { get; private set; }

        public ReqCheatDataCheck() : base(PACKET_ID) { }

        public ReqCheatDataCheck(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteShortBytes(Format);
            CheckData.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            Format = reader.ReadShortBytes();
            CheckData = CompoundList.Deserialize<UnknownCheckData>(reader);
        }

        public override string ToString()
        {
            var str = base.ToString() + $":\n\tFormat '{Packet.Hexstring(Format, ' ')}'\n\tCheckData\n{CheckData}";
            return str;
        }
    }
}
