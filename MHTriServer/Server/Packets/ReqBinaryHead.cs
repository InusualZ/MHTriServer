using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class ReqBinaryHead : Packet
    {
        public const uint PACKET_ID = 0x63020100;

        public byte UnknownField { get; private set; }

        public ReqBinaryHead(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqBinaryHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);

            UnknownField = reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
