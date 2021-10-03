
namespace MHTriServer.Server.Packets.Properties
{
    public class NtcBinaryCompoundData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;

        /// <summary>
        /// Somekind of time related field
        /// </summary>
        public uint UnknownField1 { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public string CapcomID { get => Get<string>(FIELD_2); set => Set(FIELD_2, value); }

        public string Name { get => Get<string>(FIELD_3); set => Set(FIELD_3, value); }
    }
}
