using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcCircleMatchStart : Packet
    {
        public const uint PACKET_ID = 0x65121000;

        // TODO: Guessed name
        public class HunterData
        {
            // When the client read this byte, it substract one to it
            public byte UnknownField1 { get; set; }

            // Max size: 8 including null character
            public string UnknownField2 { get; set; }

            // The client read this as a byte array of max size of 1
            public byte UnknownField3 { get; set; }

            public ushort UnknownField4 { get; set; }

            public int CalculateSize()
            {
                var size = 0;

                size += sizeof(byte); // UnknownField 1

                var unknownField3Length = Encoding.ASCII.GetByteCount(UnknownField2);
                Debug.Assert(unknownField3Length < 8);
                size += sizeof(ushort) + unknownField3Length; // UnknownField 2 = length (ushort) + str bytes

                size += sizeof(ushort) + sizeof(byte); // UnknownField 3

                size += sizeof(ushort); // UnknownField 4

                return size;
            }
        }

        public List<HunterData> HuntersData { get; private set; }

        public byte UnknownField2 { get; private set; }

        public uint UnknownField3 { get; private set; }

        public uint UnknownField4 { get; private set; }

        public uint UnknownField5 { get; private set; }

        public uint UnknownField6 { get; private set; }

        public int PatInterface0XD630 { get; private set; }

        private bool SerializeExtraFields = false;

        public NtcCircleMatchStart(List<HunterData> huntersData, int unknownField7) : base(PACKET_ID) => (HuntersData, SerializeExtraFields, PatInterface0XD630) = (huntersData, false, unknownField7);

        public NtcCircleMatchStart(List<HunterData> huntersData, byte unknownField2, uint unknownField3, uint unknownField4, uint unknownField5, uint unknownField6, int patInterface0XD630) : base(PACKET_ID)
            => (HuntersData, SerializeExtraFields, UnknownField2, UnknownField3, UnknownField4, UnknownField5, UnknownField6, PatInterface0XD630) 
            = (huntersData, true, unknownField2, unknownField3, unknownField4, unknownField5, unknownField6, patInterface0XD630);

        public NtcCircleMatchStart(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            Debug.Assert(HuntersData.Count <= 4);

            var calculatedBytesCount = HuntersData.Sum(h => h.CalculateSize()) + sizeof(uint)/* List Count*/;
            if (SerializeExtraFields)
            {
                calculatedBytesCount += sizeof(byte); // UnknownField2
                calculatedBytesCount += sizeof(uint); // UnknownField3
                calculatedBytesCount += sizeof(uint); // UnknownField4
                calculatedBytesCount += sizeof(uint); // UnknownField5
                calculatedBytesCount += sizeof(uint); // UnknownField6
            }

            writer.Write((ushort)calculatedBytesCount);

            writer.Write((uint)HuntersData.Count);

            foreach (var hunter in HuntersData)
            {
                writer.Write(hunter.UnknownField1);
                writer.Write(hunter.UnknownField2);

                writer.Write((ushort)1);
                writer.Write(hunter.UnknownField3);

                writer.Write(hunter.UnknownField4);
            }

            if (SerializeExtraFields)
            {
                writer.Write(UnknownField2);
                writer.Write(UnknownField3);
                writer.Write(UnknownField4);
                writer.Write(UnknownField5);
                writer.Write(UnknownField6);
            }

            writer.Write(PatInterface0XD630);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
            // TODO: Implement me
        }
    }
}
