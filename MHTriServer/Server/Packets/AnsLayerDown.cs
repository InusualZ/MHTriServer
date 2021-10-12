using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerDown : Packet
    {
        public const uint PACKET_ID = 0x64140200;

        public ushort LayerIndex { get; private set; }

        public AnsLayerDown(ushort layerIndex) : base(PACKET_ID) => LayerIndex = layerIndex;

        public AnsLayerDown(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(LayerIndex);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 2);

            LayerIndex = reader.ReadUInt16();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tLayerIndex {LayerIndex}";
        }
    }
}
