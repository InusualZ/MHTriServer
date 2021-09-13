using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerDown : Packet
    {
        public const uint PACKET_ID = 0x64140100;

        public ushort UnknownField { get; private set; }

        public LayerDownData Slot { get; private set; }

        public ReqLayerDown(LayerDownData slot) : base(PACKET_ID) => Slot = slot;

        public ReqLayerDown(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            Slot.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt16();
            Slot = CompoundList.Deserialize<LayerDownData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tSlot\n{Slot}";
        }
    }
}
