namespace MHTriServer.Server
{
    public class LayerDownData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02; // Weird field
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_5 = 0x05;
        private const byte FIELD_9 = 0x09;
        private const byte FIELD_10 = 0x0a;
        private const byte FIELD_12 = 0x0c;
        private const byte FIELD_23 = 0x17;


        public uint UnknownField1 { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public byte[] UnknownField2 {
            get => Get<byte[]>(FIELD_2);
            set {
                Set(FIELD_2, value);
            }
        }

        public string UnknownField3 { get => Get<string>(FIELD_3); set => Set(FIELD_3, value); }

        public ushort UnknownField5 { get => Get<ushort>(FIELD_5); set => Set(FIELD_5, value); }

        public uint UnknownField9 { get => Get<uint>(FIELD_9); set => Set(FIELD_9, value); }

        public uint UnknownField10 { get => Get<uint>(FIELD_10); set => Set(FIELD_10, value); }

        public uint UnknownField12 { get => Get<uint>(FIELD_12); set => Set(FIELD_12, value); }

        public byte[] UnknownField23 {
            get => Get<byte[]>(FIELD_23);
            set {
                Set(FIELD_23, value);
            }
        }


    }
}
