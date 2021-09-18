using System.Diagnostics;

namespace MHTriServer.Server.Packets.Properties
{
    public class FmpData : CompoundList
    {
        private const byte FIELD_1 = 0x01; // Used
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_7 = 0x07; // Used
        private const byte FIELD_8 = 0x08; // Used
        private const byte FIELD_9 = 0x09; // Used
        private const byte FIELD_10 = 0x0a; // Used
        // private const byte FIELD_11 = 0x0b;
        private const byte FIELD_12 = 0x0c; // Used

        public uint Index { get => Get<uint>(FIELD_1); set => Set(FIELD_1, value); }

        public string ServerAddress {
            get => Get<string>(FIELD_2);
            set {
                Debug.Assert(value.Length < 0x100);
                Set(FIELD_2, value);
            }
        }

        public ushort ServerPort { get => Get<ushort>(FIELD_3); set => Set(FIELD_3, value); }

        public ulong ServerTypeIndex { get => Get<ulong>(FIELD_7); set => Set(FIELD_7, value); }

        public uint CurrentPopulation { get => Get<uint>(FIELD_8); set => Set(FIELD_8, value); }

        public uint MaxPopulation { get => Get<uint>(FIELD_9); set => Set(FIELD_9, value); }

        public string ServerName {
            get => Get<string>(FIELD_10);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(FIELD_10, value);
            }
        }

        public uint UnknownField12 { get => Get<uint>(FIELD_12); set => Set(FIELD_12, value); }


        public static FmpData Server(uint index, uint currentPopulation, uint maxPopulation, ulong ServerTypeIndex, 
            string serverName, uint unknownField12)
        {
            return new FmpData() { 
                Index = index,
                CurrentPopulation = currentPopulation,
                MaxPopulation = maxPopulation,
                ServerTypeIndex = ServerTypeIndex,
                ServerName = serverName, 
                UnknownField12 = unknownField12
            };
        }

        public static FmpData Address(string address, ushort port)
        {
            return new FmpData()
            {
                ServerAddress = address,
                ServerPort = port,
            };
        }
    }
}
