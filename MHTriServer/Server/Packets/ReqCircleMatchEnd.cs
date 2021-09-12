﻿using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqCircleMatchEnd : Packet
    {
        public const uint PACKET_ID = 0x65130100;

        public byte UnknownField1 { get; private set; }

        public ReqCircleMatchEnd(byte unknownField1) : base(PACKET_ID)
            => (UnknownField1) = (unknownField1);

        public ReqCircleMatchEnd(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);

            UnknownField1 = reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1:X2}";
        }
    }
}