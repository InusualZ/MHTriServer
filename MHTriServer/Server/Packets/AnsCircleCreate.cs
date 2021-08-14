﻿using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleCreate : Packet
    {
        public const uint PACKET_ID = 0x65010200;

        public uint UnknownField1 { get; private set; }

        public AnsCircleCreate(uint unknownField1) : base(PACKET_ID)
            => (UnknownField1) = (unknownField1);

        public AnsCircleCreate(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            UnknownField1 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}";
        }
    }
}