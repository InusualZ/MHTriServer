using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server
{
    public class LayerUserData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_6 = 0x06;
        private const byte FIELD_7 = 0x07;

        public string UnknownField {
            get => Get<string>(FIELD_1);
            set {
                Debug.Assert(value.Length < 8);
                Set(FIELD_1, value);
            }
        }


        public string UnknownField2 {
            get => Get<string>(FIELD_2);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(FIELD_2, value);
            }
        }

        public UnkShortArrayStruct UnknownField3 { get => Get<UnkShortArrayStruct>(FIELD_3); set => Set(FIELD_3, value); }

        public uint UnknownField6 {
            get => Get<uint>(FIELD_6);
            set => Set(FIELD_6, value);
        }

        public byte[] UnknownField7 {
            get => Get<byte[]>(FIELD_7);
            set {
                Debug.Assert(value.Length < 0x100);
                Set(FIELD_7, value);
            }
        }

        protected override bool TryRead(byte key, byte type, BEBinaryReader reader, out object value)
        {
            if (key == FIELD_3)
            {
                // TEST type?
                value = UnkShortArrayStruct.Deserialize(reader);
                return true;
            }
            return base.TryRead(key, type, reader, out value);
        }

        protected override bool TryWrite(byte key, object value, BEBinaryWriter writer)
        {
            if (key == FIELD_3 && value is UnkShortArrayStruct unknownData)
            {
                // The client write this byte, since technically is a binary payload 
                writer.Write((byte)ElementType.Binary);
                unknownData.Serialize(writer);
                return true;
            }

            return base.TryWrite(key, value, writer);
        }
    }
}
