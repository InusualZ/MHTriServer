namespace MHTriServer.Server
{
    public class ChargeInfo : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_5 = 0x05;
        private const byte ONLINE_SUPPORT_CODE_FIELD = 0x07;

        public uint UnknownField1 { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public uint UnknownField2 { get => Get<uint>(FIELD_2); set => Set(FIELD_2, value); }

        public byte[] UnknownField5 { get => Get<byte[]>(FIELD_5); set => Set(FIELD_5, value); }

        public string OnlineSupportCode { get => Get<string>(ONLINE_SUPPORT_CODE_FIELD); set => Set(ONLINE_SUPPORT_CODE_FIELD, value); }
    }
}
