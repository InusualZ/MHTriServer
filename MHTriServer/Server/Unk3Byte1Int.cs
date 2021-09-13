using System.Collections.Generic;
using MHTriServer.Utils;

namespace MHTriServer.Server
{
    public class Unk3Byte1Int
    {
        public byte UnknownField1 { get; set; }
        public byte UnknownField2{ get; set; }
        public bool ContainUnknownField4{ get; set; }
        public uint UnknownField4 { get; set; }

        public void Serialize(BEBinaryWriter writer)
        {
            writer.Write(UnknownField1);
            writer.Write(UnknownField2);
            writer.Write(ContainUnknownField4);
            if (ContainUnknownField4)
            {
                writer.Write(UnknownField4);
            }
        }

        public void Deserialize(BEBinaryReader reader)
        {
            UnknownField1 = reader.ReadByte();
            UnknownField2 = reader.ReadByte();
            ContainUnknownField4 = reader.ReadBoolean();
            if (ContainUnknownField4)
            {
                UnknownField4 = reader.ReadUInt32();
            }
        }


        public static List<Unk3Byte1Int> DeserializeArray(BEBinaryReader reader)
        {
            var result = new List<Unk3Byte1Int>();
            var count = reader.ReadUInt32();
            for (var i = 0; i < count; ++i)
            {
                var element = new Unk3Byte1Int();
                element.Deserialize(reader);
                result.Add(element);
            }
            return result;
        }

        public static void SerializeArray(List<Unk3Byte1Int> arr, BEBinaryWriter writer)
        {
            if (arr == null)
            {
                writer.Write(0);
                return;
            }

            writer.Write((uint)arr.Count);
            foreach (var element in arr)
            {
                element.Serialize(writer);
            }
        }
    }
}
