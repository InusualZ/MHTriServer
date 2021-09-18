using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLoginInfo : Packet
    {
        public const uint PACKET_ID = 0x61010100;

        public ChargeInfo Data { get; set; }

        public ReqLoginInfo(ChargeInfo data) : base(PACKET_ID) => (Data) = (data);

        public ReqLoginInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Data = CompoundList.Deserialize<ChargeInfo>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLoginInfo(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tData\n{Data}";
        }
    }
}
