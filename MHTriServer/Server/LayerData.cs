
using System.Diagnostics;

namespace MHTriServer.Server
{
    public class LayerData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02; // Weird field
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_5 = 0x05;
        private const byte FIELD_6 = 0x06;
        private const byte FIELD_7 = 0x07;
        private const byte FIELD_8 = 0x08;
        private const byte FIELD_9 = 0x09;
        private const byte FIELD_10 = 0x0a;
        private const byte FIELD_11 = 0x0b;
        private const byte FIELD_12 = 0x0c;
        private const byte FIELD_13 = 0x0d;
        private const byte FIELD_16 = 0x10;
        private const byte FIELD_17 = 0x11;
        private const byte FIELD_18 = 0x12;
        private const byte FIELD_21 = 0x15;
        private const byte FIELD_22 = 0x16;
        private const byte FIELD_23 = 0x17;


        public uint UnknownField1 { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public string Name {
            get => Get<string>(FIELD_3);
            set {
                Debug.Assert(value.Length < 0x40);
                Set(FIELD_3, value);
            }
        }

        // Any value that you put here, when the client receive it. It would substract one
        public ushort UnknownField5 { get => Get<ushort>(FIELD_5); set => Set(FIELD_5, value); }

        public uint CurrentPopulation { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        public uint UnknownField7 { get => Get<uint>(FIELD_7); set => Set(FIELD_7, value); }

        public uint UnknownField8 { get => Get<uint>(FIELD_8); set => Set(FIELD_8, value); }

        public uint MaxPopulation { get => Get<uint>(FIELD_9); set => Set(FIELD_9, value); }

        public uint UnknownField10 { get => Get<uint>(FIELD_10); set => Set(FIELD_10, value); }

        public uint UnknownField11 { get => Get<uint>(FIELD_11); set => Set(FIELD_11, value); }

        public uint UnknownField12 { get => Get<uint>(FIELD_12); set => Set(FIELD_12, value); }

        public ushort UnknownField13 { get => Get<ushort>(FIELD_13); set => Set(FIELD_13, value); }

        public byte UnknownField16 { get => Get<byte>(FIELD_16); set => Set(FIELD_16, value); }

        public uint UnknownField17 { get => Get<uint>(FIELD_17); set => Set(FIELD_17, value); }

        public byte UnknownField18 { get => Get<byte>(FIELD_18); set => Set(FIELD_18, value); }

        // Any value that you put here, when the client receive it. It would substract one
        public byte UnknownField21 { get => Get<byte>(FIELD_21); set => Set(FIELD_21, value); }

        public string UnknownField22 {
            get => Get<string>(FIELD_22);
            set {
                Debug.Assert(value.Length < 0xc0);
                Set(FIELD_22, value);
            }
        }

        public byte[] UnknownField23 {
            get => Get<byte[]>(FIELD_23);
            set {
                Debug.Assert(value.Length < 0x100);
                Set(FIELD_23, value);
            }
        }

    }
}
