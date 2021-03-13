using System;
using System.Collections.Generic;
using System.Text;

namespace MHTriServer.Server
{
    public class ConnectionData : CompoundList
    {
        private const byte ONLINE_SUPPORT_CODE_FIELD = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x05;
        private const byte FIELD_4 = 0x05;
        private const byte FIELD_5 = 0x05;
        private const byte FIELD_6 = 0x06;
        private const byte UNKNOWN_UNIQUE_ID_FIELD = 0x07;
        private const byte RECEIVE_BUFFER_MAX_LENGTH = 0x08;
        private const byte FIELD_9 = 0x09;
        private const byte LANGUAGE_CODE_FIELD = 0x0a;

        public string OnlineSupportCode { get => Get<string>(ONLINE_SUPPORT_CODE_FIELD); set => Set(ONLINE_SUPPORT_CODE_FIELD, value); }

        public string UnknownUniqueID { get => Get<string>(UNKNOWN_UNIQUE_ID_FIELD); set => Set(UNKNOWN_UNIQUE_ID_FIELD, value); }

        public byte[] UnknownField2 { get => Get<byte[]>(FIELD_2); set => Set(FIELD_2, value); }

        public uint UnknownField3 { get => Get<uint>(FIELD_3); set => Set(FIELD_3, value); }

        // Seems to be always 2
        public uint UnknownField4 { get => Get<uint>(FIELD_4); set => Set(FIELD_4, value); }

        public uint UnknownField5 { get => Get<uint>(FIELD_5); set => Set(FIELD_5, value); }

        public uint UnknownField6 { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        // This is a guess, but I'm almost certain about 
        public uint ReceiveBufferMaxLength { get => Get<uint>(RECEIVE_BUFFER_MAX_LENGTH); set => Set(RECEIVE_BUFFER_MAX_LENGTH, value); }

        // Comes from a config file
        public uint UnknownField9 { get => Get<uint>(FIELD_9); set => Set(FIELD_9, value); }

        public uint LanguageCode { get => Get<uint>(LANGUAGE_CODE_FIELD); set => Set(LANGUAGE_CODE_FIELD, value); }
    }
}
