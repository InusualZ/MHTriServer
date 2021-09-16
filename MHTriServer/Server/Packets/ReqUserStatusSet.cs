using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqUserStatusSet : Packet
    {
        public const uint PACKET_ID = 0x66400100;

        public CompoundList UserStatus { get; private set; }

        public ReqUserStatusSet(CompoundList userStatus) : base(PACKET_ID) => (UserStatus) = (userStatus);

        public ReqUserStatusSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            UserStatus.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UserStatus = CompoundList.Deserialize<CompoundList>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqUserStatusSet(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUserStatus\n{UserStatus}";
        }
    }
}
