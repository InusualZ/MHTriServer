using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserBinarySet : Packet
    {
        public const uint PACKET_ID = 0x66310100;

        public uint UnknownField { get; private set; }

        public byte[] UnknownField2 { get; private set; }

        public ReqUserBinarySet(uint unknownField, byte[] unknownField2) : base(PACKET_ID) => (UnknownField, UnknownField2) = (unknownField, unknownField2);

        public ReqUserBinarySet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.WriteShortBytes(UnknownField2);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt32();
            UnknownField2 = reader.ReadShortBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField {UnknownField}\n\tUnknownField2 '{Packet.Hexstring(UnknownField2, ' ')}'";
        }
    }
}
