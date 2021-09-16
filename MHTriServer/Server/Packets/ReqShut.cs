using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqShut : Packet
    {
        public const uint PACKET_ID = 0x60100100;

        // Possible values: 1 or 2
        public byte UnknownField { get; set; }

        public ReqShut(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqShut(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);
            UnknownField = reader.ReadByte();
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqShut(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
