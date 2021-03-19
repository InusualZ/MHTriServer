using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class NtcLogin : Packet
    {
        public const uint PACKET_ID = 0x60211000;

        public ServerLoginType LoginType { get; set; }

        public NtcLogin(ServerLoginType loginType) : base(PACKET_ID) => LoginType = loginType;

        public NtcLogin(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((byte)LoginType);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);
            LoginType = (ServerLoginType)reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tLoginType {LoginType}";
        }
    }
}
