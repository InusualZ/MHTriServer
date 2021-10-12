using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerDown : Packet
    {
        public const uint PACKET_ID = 0x64140100;

        public ushort LayerIndex { get; private set; }

        public LayerData Layer { get; private set; }

        public ReqLayerDown(LayerData slot) : base(PACKET_ID) => Layer = slot;

        public ReqLayerDown(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(LayerIndex);
            Layer.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            LayerIndex = reader.ReadUInt16();
            Layer = CompoundList.Deserialize<LayerData>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerDown(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tLayerIndex {LayerIndex}\n\tLayer\n{Layer}";
        }
    }
}
