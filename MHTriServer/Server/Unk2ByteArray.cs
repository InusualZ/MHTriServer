using System.Collections.Generic;
using MHTriServer.Utils;

namespace MHTriServer.Server
{
    public class Unk2ByteArray
    {
        public byte UnknownField1 { get; set; }

        public byte UnknownField2 { get; set; }

        public static Unk2ByteArray Deserialize(BEBinaryReader reader)
        {
            return new Unk2ByteArray()
            {
                UnknownField1 = reader.ReadByte(),
                UnknownField2 = reader.ReadByte()
            };
        }

        public void Serialize(BEBinaryWriter writer)
        {
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
        }

        public static List<Unk2ByteArray> DeserializeArray(BEBinaryReader reader)
        {
            var result = new List<Unk2ByteArray>();

            var count = (int)reader.ReadByte();
            for (var i = 0; i < count; ++i)
            {
                result.Add(Deserialize(reader));
            }
            return result;
        }

        public static void SerializeArray(List<Unk2ByteArray> arr, BEBinaryWriter writer)
        {
            if (arr == null)
            {
                writer.Write((byte)0);
                return;
            }

            var count = arr.Count;
            writer.Write((byte)count);

            for (var i  = 0; i < count; ++i)
            {
                arr[i].Serialize(writer);
            }
        }
    }
}
