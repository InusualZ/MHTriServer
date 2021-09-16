using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqVulgarityLow : Packet
    {
        public const uint PACKET_ID = 0x62570100;

        public uint InfoType { get; private set; }

        public uint Version { get; private set; }

        public uint CurrentLength { get; private set; }

        public uint ExpectedLength { get; private set; }

        public ReqVulgarityLow(uint vulgarityInfoType, uint termsVersion, uint expectedLength, uint extraLength) : base(PACKET_ID) 
            => (InfoType, Version, CurrentLength, ExpectedLength) = (vulgarityInfoType, termsVersion, expectedLength, extraLength);

        public ReqVulgarityLow(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(InfoType);
            writer.Write(Version);
            writer.Write(CurrentLength);
            writer.Write(ExpectedLength);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 16);
            InfoType = reader.ReadUInt32();
            Version = reader.ReadUInt32();
            CurrentLength = reader.ReadUInt32();
            ExpectedLength = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqVulgarityLow(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tInfoType {InfoType}\n\tVersion {Version}\n\tCurrentLength {CurrentLength}\n\tExpectedLength {ExpectedLength}";
        }
    }
}
