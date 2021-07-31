using System;
using System.Text;

namespace MHTriServer.Server
{
    public struct ServerType
    {
        private const int SERVER_TYPE_STRUCT_SIZE = 0xc0;
        private const int SERVER_TYPE_NAME_SIZE = 24;
        private const int SERVER_TYPE_DESC_SIZE = 168;

        public static readonly ServerType OPEN = new ServerType("Open", "Hunters of all Ranks\ncan gather here.");
        public static readonly ServerType ROOKIE = new ServerType("Rookie", "Only hunters HR 30\nor lower may enter");
        public static readonly ServerType EXPERT = new ServerType("Expert", "Only hunters HR 31\nor higher may enter.");
        public static readonly ServerType RECRUITING = new ServerType("Recruiting", "Hunters in search of\nhunting companions\ncan gather here.");

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ServerType(string name, string desciption)
        {
            Name = name;
            Description = desciption;
        }

        public static byte[] GenerateBinaryData() => GenerateBinaryData(OPEN, ROOKIE, EXPERT, RECRUITING);

        public static byte[] GenerateBinaryData(params ServerType[] serverTypes)
        {
            var asciiEncoder = Encoding.ASCII;
            var blob = new byte[SERVER_TYPE_STRUCT_SIZE * serverTypes.Length];
            var currentOffset = 0;

            foreach (var serverType in serverTypes)
            {
                asciiEncoder.GetBytes(serverType.Name, new Span<byte>(blob, currentOffset, SERVER_TYPE_NAME_SIZE));
                currentOffset += SERVER_TYPE_NAME_SIZE;

                asciiEncoder.GetBytes(serverType.Description, new Span<byte>(blob, currentOffset, SERVER_TYPE_DESC_SIZE));
                currentOffset += SERVER_TYPE_DESC_SIZE;
            }

            return blob;
        }
    }
}
