using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class MediationData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;

        public string UnknownField1
        {
            get => Get<string>(FIELD_1);
            set
            {
                Debug.Assert(value.Length < 8);
                Set(FIELD_1, value);
            }
        }


        public byte UnknownField2
        {
            get => Get<byte>(FIELD_2);
            set
            {
                Set(FIELD_2, value);
            }
        }

        public byte UnknownField3
        {
            get => Get<byte>(FIELD_3);
            set
            {
                Set(FIELD_3, value);
            }
        }
    }
}
