using System;
using System.Collections.Generic;
using MHTriServer.Utils;

namespace MHTriServer.Server
{
    public class UnkByteIntStruct
    {
        private const int MAX_ARRAY_SIZE = 0x20;

        public byte UnknownField { get; set; }

        public bool ContainUnknownField3 { get; set; }

        public uint UnknownField3 { get; set; }

        public static UnkByteIntStruct Deserialize(BEBinaryReader reader)
        {
            var unknownField = reader.ReadByte();
            var containUnkField3 = reader.ReadByte() == 1;
            uint unknownField3 = 0;
            if (containUnkField3)
            {
                unknownField3 = reader.ReadUInt32();
            }

            return new UnkByteIntStruct()
            {
                UnknownField = unknownField,
                ContainUnknownField3 = containUnkField3,
                UnknownField3 = unknownField3
            };
        }

        public void Serialize(BEBinaryWriter writer)
        {
            writer.Write(UnknownField);
            writer.Write(ContainUnknownField3);
            if (ContainUnknownField3)
            {
                writer.Write(UnknownField3);
            }
        }

        public static List<UnkByteIntStruct> DeserializeArray(BEBinaryReader reader)
        {
            var result = new List<UnkByteIntStruct>();

            var count = (int)reader.ReadByte();
            var i = 0;
            
            // In the client there seems to be a hard cap on how many this it can read
            for (; i < Math.Min(count, MAX_ARRAY_SIZE); ++i)
            {
                result.Add(Deserialize(reader));
            }

            // Discard the remaining ones
            for (; i < count; ++i)
            {
                _ = Deserialize(reader);
            }
            return result;
        }

        public static void SerializeArray(List<UnkByteIntStruct> arr, BEBinaryWriter writer)
        {
            if (arr == null)
            {
                writer.Write((byte)0);
                return;
            }

            var count = Math.Min(arr.Count, MAX_ARRAY_SIZE);

            writer.Write((byte)count);

            for (var i  = 0; i < count; ++i)
            {
                arr[i].Serialize(writer);
            }
        }
    }
}
