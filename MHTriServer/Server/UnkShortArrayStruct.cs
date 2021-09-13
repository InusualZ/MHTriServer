using System;
using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server
{
    public class UnkShortArrayStruct
    {
        public uint UnknownField { get; set; }

        public uint UnknownField2 { get; set; }

        public List<ushort> UnknownField3 { get; set; }

        public static UnkShortArrayStruct Deserialize(BEBinaryReader reader)
        {
            var byteCount = reader.ReadUInt16();
            var unknownField = reader.ReadUInt32();
            var unknownField2 = reader.ReadUInt32();

            var ushortCount = Math.Min((byteCount - (2 * sizeof(uint))) / sizeof(ushort), 3);
            var unknownField3 = new List<ushort>(ushortCount);
            for(var i = 0; i < ushortCount; ++i)
            {
                unknownField3.Add(reader.ReadUInt16());
            }

            return new UnkShortArrayStruct()
            {
                UnknownField = unknownField,
                UnknownField2 = unknownField2,
                UnknownField3 = unknownField3
            };
        }

        public void Serialize(BEBinaryWriter writer)
        {
            Debug.Assert(UnknownField3.Count <= 3);
            var byteCount = (2 * sizeof(uint)) + (sizeof(ushort) * UnknownField3.Count);
            writer.Write((ushort)byteCount);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);

            foreach(var val in UnknownField3)
            {
                writer.Write(val);
            }
        }
    }
}
