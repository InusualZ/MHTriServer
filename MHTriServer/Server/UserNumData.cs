using System;
using System.Collections.Generic;
using System.Text;

namespace MHTriServer.Server
{
    public class UserNumData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_4 = 0x04;
        private const byte FIELD_5 = 0x05;
        private const byte FIELD_6 = 0x06;
        private const byte FIELD_7 = 0x07;

        public UnknownDataStruct UnknownField1 { get => Get<UnknownDataStruct>(FIELD_1); set => Set(FIELD_1, value); }

        public uint UnknownField2 { get => Get<uint>(FIELD_2); set => Set(FIELD_2, value); }

        public uint UnknownField3 { get => Get<uint>(FIELD_3); set => Set(FIELD_3, value); }

        public uint UnknownField4 { get => Get<uint>(FIELD_4); set => Set(FIELD_4, value); }

        public uint UnknownField5 { get => Get<uint>(FIELD_5); set => Set(FIELD_5, value); }

        public uint UnknownField6 { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        public uint UnknownField7 { get => Get<uint>(FIELD_7); set => Set(FIELD_7, value); }


        protected override bool TryRead(byte key, byte type, ExtendedBinaryReader reader, out object value)
        {
            if (key == FIELD_1)
            {
                // TEST type?
                value = UnknownDataStruct.Deserialize(reader);
                return true;
            }
            return base.TryRead(key, type, reader, out value);
        }

        protected override bool TryWrite(byte key, object value, ExtendedBinaryWriter writer)
        {
            if (!(value is UnknownDataStruct unknownData))
            {
                return base.TryWrite(key, value, writer);
            }

            if (key == FIELD_1)
            {
                // The client write this byte, since technically is a binary payload 
                writer.Write((byte)0x06);
                unknownData.Serialize(writer);
            }

            return base.TryWrite(key, value, writer);
        }
    }
}
