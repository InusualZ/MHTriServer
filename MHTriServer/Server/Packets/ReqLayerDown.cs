using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerDown : Packet
    {
        public const uint PACKET_ID = 0x64140100;

        public ushort Index { get; private set; }

        public LayerDownData Slot { get; private set; }

        public ReqLayerDown(LayerDownData slot) : base(PACKET_ID) => Slot = slot;

        public ReqLayerDown(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Index);
            Slot.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Index = reader.ReadUInt16();
            Slot = CompoundList.Deserialize<LayerDownData>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerDown(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tIndex {Index}\n\tSlot\n{Slot}";
        }
    }
}
