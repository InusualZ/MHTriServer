using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class AnsBinaryHead : Packet
    {
        public const uint PACKET_ID = 0x63020200;

        public uint SomeVersion { get; private set; }

        public uint BinaryLength { get; private set; }

        public AnsBinaryHead(uint someVersion, uint termsSomeLength) : base(PACKET_ID) => (SomeVersion, BinaryLength) = (someVersion, termsSomeLength);

        public AnsBinaryHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(SomeVersion);
            writer.Write(BinaryLength);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);
            SomeVersion = reader.ReadUInt32();
            BinaryLength = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tSomeVersion {SomeVersion}\n\tBinaryLength {BinaryLength}";
        }
    }
}
