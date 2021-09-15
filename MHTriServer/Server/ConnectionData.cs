
using System.Diagnostics;

namespace MHTriServer.Server
{
    public class ConnectionData : CompoundList
    {
        private const byte ONLINE_SUPPORT_CODE_FIELD = 0x01;
        private const byte PAT_TICKET_FIELD = 0x02;
        private const byte FIELD_3 = 0x05;
        private const byte FIELD_4 = 0x05;
        private const byte CONVERTED_COUNTRY_CODE_FIELD = 0x05;
        private const byte FIELD_6 = 0x06;
        private const byte MEDIA_VERSION_FIELD = 0x07;
        private const byte RECEIVE_BUFFER_MAX_LENGTH = 0x08;
        private const byte COUNTRY_CODE_FIELD = 0x09;
        private const byte LANGUAGE_CODE_FIELD = 0x0a;

        public string OnlineSupportCode
        {
            get => Get<string>(ONLINE_SUPPORT_CODE_FIELD); 
            set
            {
                Debug.Assert(value.Length < 0x20);
                Set(ONLINE_SUPPORT_CODE_FIELD, value);
            }
        }

        public string MediaVersion { get => Get<string>(MEDIA_VERSION_FIELD); set => Set(MEDIA_VERSION_FIELD, value); }

        public byte[] PatTicket { get => Get<byte[]>(PAT_TICKET_FIELD); set => Set(PAT_TICKET_FIELD, value); }

        public uint UnknownField3 { get => Get<uint>(FIELD_3); set => Set(FIELD_3, value); }

        // Seems to be always 2
        public uint UnknownField4 { get => Get<uint>(FIELD_4); set => Set(FIELD_4, value); }

        public uint ConvertedCountryCode { get => Get<uint>(CONVERTED_COUNTRY_CODE_FIELD); set => Set(CONVERTED_COUNTRY_CODE_FIELD, value); }

        public uint UnknownField6 { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        // This is a guess, but I'm almost certain about 
        public uint ReceiveBufferMaxLength { get => Get<uint>(RECEIVE_BUFFER_MAX_LENGTH); set => Set(RECEIVE_BUFFER_MAX_LENGTH, value); }

        // Comes from a config file
        public uint CountryCode { get => Get<uint>(COUNTRY_CODE_FIELD); set => Set(COUNTRY_CODE_FIELD, value); }

        public uint LanguageCode { get => Get<uint>(LANGUAGE_CODE_FIELD); set => Set(LANGUAGE_CODE_FIELD, value); }
    }
}
