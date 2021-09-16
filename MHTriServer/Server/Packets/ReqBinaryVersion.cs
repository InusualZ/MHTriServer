using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqBinaryVersion : Packet
    {
        public const uint PACKET_ID = 0x63010100;

        public byte BinaryType { get; private set; }

        public ReqBinaryVersion(byte unknownField) : base(PACKET_ID) => BinaryType = unknownField;

        public ReqBinaryVersion(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(BinaryType);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);

            BinaryType = reader.ReadByte();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqBinaryVersion(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {BinaryType}";
        }
    }
}
