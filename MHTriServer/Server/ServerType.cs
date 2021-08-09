using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server
{
    public struct ServerType
    {
        private const int BIG_BINARY_BLOB_SIZE = 0x140c;
        private const int SERVER_TYPE_NAME_SIZE = 24;
        private const int SERVER_TYPE_DESC_SIZE = 168;
        private const int SERVER_TYPE_STRUCT_SIZE = SERVER_TYPE_NAME_SIZE + SERVER_TYPE_DESC_SIZE;
        private const int SERVER_TYPE_MINMAX_STRUCT_SIZE = sizeof(ushort) + sizeof(ushort);
        private const int MAX_SERVER_TYPE = 4;
        private const int MAX_SEEKING_DESC_LENGTH = 0x0d; // Unconfirmed, probably is wrong
        private const int SEEKING_STRUCT_SIZE = 52;

        public static readonly ServerType OPEN = new ServerType("Open", "Hunters of all Ranks\ncan gather here.", 0, ushort.MaxValue);
        public static readonly ServerType ROOKIE = new ServerType("Rookie", "Only hunters HR 30\nor lower may enter", 0, 30);
        public static readonly ServerType EXPERT = new ServerType("Expert", "Only hunters HR 31\nor higher may enter.", 31, ushort.MaxValue);
        public static readonly ServerType RECRUITING = new ServerType("Recruiting", "Hunters in search of\nhunting companions\ncan gather here.", 0, ushort.MaxValue);

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ushort MinRank { get; private set;  }

        public ushort MaxRank { get; private set; }

        public ServerType(string name, string desciption, ushort minRank, ushort maxRank)
        {
            Name = name;
            Description = desciption;
            MinRank = minRank;
            MaxRank = maxRank;
        }

        public static byte[] GenerateBinaryData() => GenerateBinaryData(OPEN, ROOKIE, EXPERT, RECRUITING);

        public static byte[] GenerateBinaryData(params ServerType[] serverTypes)
        {
            Debug.Assert(serverTypes.Length <= MAX_SERVER_TYPE);

            // TODO: Move the generation of this big binary blob, elsewhere
            var asciiEncoder = Encoding.ASCII;
            var blob = new byte[BIG_BINARY_BLOB_SIZE];
            for (var index = 0; index < serverTypes.Length; ++index)
            {
                var serverType = serverTypes[index];
                asciiEncoder.GetBytes(serverType.Name, new Span<byte>(blob, 0 + (index * SERVER_TYPE_STRUCT_SIZE), SERVER_TYPE_NAME_SIZE - 1));
                asciiEncoder.GetBytes(serverType.Description, new Span<byte>(blob, 24 + (index * SERVER_TYPE_STRUCT_SIZE), SERVER_TYPE_DESC_SIZE - 1));

                BinaryPrimitives.WriteUInt16BigEndian(new Span<byte>(blob, 0x13AC + (index * SERVER_TYPE_MINMAX_STRUCT_SIZE), sizeof(ushort)), serverType.MinRank);
                BinaryPrimitives.WriteUInt16BigEndian(new Span<byte>(blob, 0x13AE + (index * SERVER_TYPE_MINMAX_STRUCT_SIZE), sizeof(ushort)), serverType.MaxRank);
            }

            // Confirmed! It control how long last the day/night cycle
            // Need more RE in order to know how exactly it work
            // 2 uint array
            // Offset: 0x304
            // Used: @8042d91c
            BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(blob, 0x304 + (0 * sizeof(uint)), sizeof(uint)), 0);
            BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(blob, 0x304 + (1 * sizeof(uint)), sizeof(uint)), 0);

            // Seeking City Strings
            const ushort SEEKING_LIST_COUNT = 8 * 4; // Confirmed max amount
            for (var i = 0; i < SEEKING_LIST_COUNT; ++i)
            {
                asciiEncoder.GetBytes($"Everyone{i}", new Span<byte>(blob, 0x30C + (i * SEEKING_STRUCT_SIZE), MAX_SEEKING_DESC_LENGTH - 1));

                blob[0x33D + (i * SEEKING_STRUCT_SIZE)] = 0x01; // Probably a boolean
                blob[0x33E + (i * SEEKING_STRUCT_SIZE)] = 0xFF; // Probably a flag field
            }

            return blob;
        }
    }
}
