
using System.Diagnostics;

namespace MHTriServer.Server
{
    public class CircleData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_4 = 0x04;
        private const byte FIELD_5 = 0x05;
        private const byte FIELD_6 = 0x06;
        private const byte FIELD_7 = 0x07;
        private const byte FIELD_8 = 0x08;
        private const byte FIELD_9 = 0x09;
        private const byte FIELD_10 = 0x0a;
        private const byte FIELD_11 = 0x0b;
        private const byte FIELD_12 = 0x0c;
        private const byte FIELD_13 = 0x0d;
        private const byte FIELD_14 = 0x0e;
        private const byte FIELD_15 = 0x0f;
        private const byte FIELD_16 = 0x10;

        public uint UnknownField1 { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public string UnknownField2
        {
            get => Get<string>(FIELD_2);
            set
            {
                Debug.Assert(value.Length < 0x40);
                Set(FIELD_2, value);
            }
        }

        public byte HasPassword { get => Get<byte>(FIELD_3); set => Set(FIELD_3, value); }

        public string UnknownField4
        {
            get => Get<string>(FIELD_4);
            set
            {
                //Debug.Assert(value.Length < 0x200);
                Set(FIELD_4, value);
            }
        }

        public byte[] Password
        {
            get => Get<byte[]>(FIELD_5); set
            {
                Debug.Assert(value.Length < 0x100);
                Set(FIELD_5, value);
            }
        }

        public string Remarks
        {
            get => Get<string>(FIELD_6);
            set
            {
                Debug.Assert(value.Length < 0x200);
                Set(FIELD_6, value);
            }
        }

        public uint UnknownField7 { get => Get<uint>(FIELD_7); set => Set(FIELD_7, value); }

        public uint UnknownField8 { get => Get<uint>(FIELD_8); set => Set(FIELD_8, value); }

        public uint UnknownField9 { get => Get<uint>(FIELD_9); set => Set(FIELD_9, value); }

        public uint UnknownField10 { get => Get<uint>(FIELD_10); set => Set(FIELD_10, value); }

        public uint UnknownField11 { get => Get<uint>(FIELD_11); set => Set(FIELD_11, value); }

        public uint UnknownField12 { get => Get<uint>(FIELD_12); set => Set(FIELD_12, value); }

        public string UnknownField13
        {
            get => Get<string>(FIELD_13);
            set
            {
                Debug.Assert(value.Length < 0x8);
                Set(FIELD_13, value);
            }
        }

        public byte UnknownField14 { get => Get<byte>(FIELD_14); set => Set(FIELD_14, value); }

        public byte UnknownField15 { get => Get<byte>(FIELD_15); set => Set(FIELD_15, value); }

        public byte UnknownField16 { get => Get<byte>(FIELD_16); set => Set(FIELD_16, value); }
    }
}
