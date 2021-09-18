using System.Diagnostics;

namespace MHTriServer.Server.Packets.Properties
{
    public class ChargeInfo : CompoundList
    {
        private const byte TICKET_VALIDTIY_1_FIELD = 0x01;
        private const byte TICKET_VALIDTIY_2_FIELD = 0x02;
        private const byte FIELD_5 = 0x05;
        private const byte ONLINE_SUPPORT_CODE_FIELD = 0x07;

        public uint TicketValidity1 { get => Get<uint>(TICKET_VALIDTIY_1_FIELD); set => Set(TICKET_VALIDTIY_1_FIELD, value); }

        public uint TicketValidity2 { get => Get<uint>(TICKET_VALIDTIY_2_FIELD); set => Set(TICKET_VALIDTIY_2_FIELD, value); }

        public byte[] UnknownField5
        {
            get => Get<byte[]>(FIELD_5);
            set
            {
                Debug.Assert(value.Length <= 0x2c);
                Set(FIELD_5, value);
            }
        }

        public string OnlineSupportCode 
        { 
            get => Get<string>(ONLINE_SUPPORT_CODE_FIELD);
            set
            {
                Debug.Assert(value.Length < 0x20);
                Set(ONLINE_SUPPORT_CODE_FIELD, value);
            }
        }
    }
}
