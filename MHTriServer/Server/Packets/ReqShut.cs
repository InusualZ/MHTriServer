using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    /*
     * This packet is sent by the client. Don't know his purpose.
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
    public class ReqShut : Packet
    {
        public const uint PACKET_ID = 0x60100100;

        // Possible values: 1 or 2
        public byte UnknownField { get; set; }

        public ReqShut(byte unknownField) : base(PACKET_ID) => UnknownField = unknownField;

        public ReqShut(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 1);
            UnknownField = reader.ReadByte();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}";
        }
    }
}
