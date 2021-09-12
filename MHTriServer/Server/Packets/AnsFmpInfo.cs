
using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsFmpInfo : Packet
    {
        public const uint PACKET_ID = 0x61340200;
        public const uint PACKET_ID_FMP = 0x63140200;

        public FmpData Slot { get; private set; }

        public AnsFmpInfo(FmpData slot) : base(PACKET_ID) => Slot = slot;

        public AnsFmpInfo(FmpData slot, bool isServerFmp) : base(PACKET_ID_FMP) => Slot = slot;

        public AnsFmpInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            Slot.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Slot = CompoundList.Deserialize<FmpData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tSlot\n{Slot}";
        }
    }
}
