using System.Diagnostics;

namespace MHTriServer.Server.Packets.Properties
{
    public class MessageData : CompoundList
    {
        private const byte COLOR_FIELD = 0x01;
        private const byte TIME_FIELD = 0x02;
        private const byte SENDER_ID = 0x03;
        private const byte SENDER_NAME = 0x04;

        public uint Color { get => Get<uint>(COLOR_FIELD); set => Set(COLOR_FIELD, value); }

        public uint Time { get => Get<uint>(TIME_FIELD); set => Set(TIME_FIELD, value); }

        public string SenderID 
        { 
            get => Get<string>(SENDER_ID); 
            set
            {
                Debug.Assert(value.Length < 8);
                Set(SENDER_ID, value);
            }
        }

        public string SenderName
        {
            get => Get<string>(SENDER_NAME);
            set
            {
                Debug.Assert(value.Length < 0x20);
                Set(SENDER_NAME, value);
            }
        }
    }
}
