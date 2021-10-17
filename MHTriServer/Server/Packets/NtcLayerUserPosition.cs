using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerUserPosition : Packet
    {
        public const uint PACKET_ID = 0x64711000;

        public string CapcomID { get; private set;  }

        public ushort UnknownField1 { get; private set; }

        public CompoundExtendedList UnknownField2 { get; private set; }

        public NtcLayerUserPosition(string capcomId, ushort unknownField1, CompoundExtendedList unknownField2) : base(PACKET_ID)
            => (CapcomID, UnknownField1, UnknownField2) = (capcomId, unknownField1, unknownField2);

        public NtcLayerUserPosition(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            // TODO: Struct is not correct
            base.Serialize(writer);
            writer.Write(CapcomID);
            writer.Write(UnknownField1);
            UnknownField2.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadUInt16();
            UnknownField2 = CompoundExtendedList.Deserialize<CompoundExtendedList>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleNtcLayerUserPosition(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField1 {UnknownField1}\n\tUnknownField2\n{UnknownField2}";
        }
    }
}
