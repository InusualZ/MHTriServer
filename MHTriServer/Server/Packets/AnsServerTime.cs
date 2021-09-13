using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsServerTime : Packet
    {
        public const uint PACKET_ID = 0x60020200;

        // Day last 1490
        public uint GameTime { get; private set; }

        public uint UnknownField2 { get; private set; }

        public AnsServerTime(uint unknownField, uint unknownField2) : base(PACKET_ID) => (GameTime, UnknownField2) = (unknownField, unknownField2);

        public AnsServerTime(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(GameTime);
            writer.Write(UnknownField2);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);

            GameTime = reader.ReadUInt32();
            UnknownField2 = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tGameTime {GameTime}\n\tUnknownField2 {UnknownField2}";
        }
    }
}
