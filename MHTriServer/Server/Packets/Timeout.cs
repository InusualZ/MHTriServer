using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class Timeout : Packet
    {
        public const uint PACKET_ID = 0x60501000;

        public class TimeoutData : CompoundList {} // TODO: Add properties for getting data

        public TimeoutData Data { get; set; }

        public Timeout(TimeoutData data) : base(PACKET_ID) => Data = data;

        public Timeout(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Data = CompoundList.Deserialize<TimeoutData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + ":\n" + Data.ToString();
        }
    }
}
