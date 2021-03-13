using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server
{
    public class UnknownDataStruct
    {
        public uint UnknownField { get; private set; }

        public uint UnknownField2 { get; private set; }

        private List<ushort> m_unknownField3;
        public IReadOnlyList<ushort> UnknownField3 => m_unknownField3;

        public static UnknownDataStruct Deserialize(ExtendedBinaryReader reader)
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

            return new UnknownDataStruct()
            {
                UnknownField = unknownField,
                UnknownField2 = unknownField2,
                m_unknownField3 = unknownField3
            };
        }

        public void Serialize(ExtendedBinaryWriter writer)
        {
            Debug.Assert(UnknownField3.Count <= 3);
            var byteCount = (2 * sizeof(uint)) + (sizeof(ushort) * UnknownField3.Count);
            writer.Write((ushort)byteCount);
            writer.Write(UnknownField);
            writer.Write(UnknownField2);

            foreach(var val in m_unknownField3)
            {
                writer.Write(val);
            }
        }
    }
}
