using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerCreateFoot : Packet
    {
        public const uint PACKET_ID = 0x64130100;

        public ushort CityIndex { get; private set; }

        public bool Cancelled { get; private set; }

        public ReqLayerCreateFoot(ushort cityIndex, bool cancelled) : base(PACKET_ID)
            => (CityIndex, Cancelled) = (cityIndex, cancelled);

        public ReqLayerCreateFoot(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CityIndex);
            writer.Write(Cancelled);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 3);

            CityIndex = reader.ReadUInt16();
            Cancelled = reader.ReadBoolean();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerCreateFoot(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tCityIndex {CityIndex}\n\tCancelled {Cancelled}";
        }
    }
}
