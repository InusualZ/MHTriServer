using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqCircleLeave : Packet
    {
        public const uint PACKET_ID = 0x65040100;

        public uint UnknownField1 { get; private set; }

        public ReqCircleLeave(uint unknownField1) : base(PACKET_ID)
            => (UnknownField1) = (unknownField1);

        public ReqCircleLeave(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 4);

            UnknownField1 = reader.ReadUInt32();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqCircleLeave(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}";
        }
    }
}
