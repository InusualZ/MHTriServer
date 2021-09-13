using System.Diagnostics;
using System.Text;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    class AnsCommonKey : Packet
    {
        public const uint PACKET_ID = 0x60700200;

        public byte[] Key { get; set; }

        public AnsCommonKey(string key) : base(PACKET_ID) => Key = Encoding.ASCII.GetBytes(key);

        public AnsCommonKey(byte[] key) : base(PACKET_ID) => Key = key;

        public AnsCommonKey(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.WriteShortBytes(Key);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Key = reader.ReadShortBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tKey {Key}";
        }
    }
}
