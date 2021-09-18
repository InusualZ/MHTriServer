using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcCollectionLog : Packet
    {
        public const uint PACKET_ID = 0x60501000;

        public class TimeoutData : CompoundList {} // TODO: Add properties for getting data

        public TimeoutData Data { get; set; }

        public NtcCollectionLog(TimeoutData data) : base(PACKET_ID) => Data = data;

        public NtcCollectionLog(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
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
