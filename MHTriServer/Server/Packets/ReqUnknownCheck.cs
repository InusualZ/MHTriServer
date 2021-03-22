using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class ReqUnknownCheck : Packet
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

        public ReqUnknownCheck() : base(PACKET_ID) { }

        public ReqUnknownCheck(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteShortBytes(Format);
            CheckData.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
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
