using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcShut : Packet
    {
        public const uint PACKET_ID = 0x60101000;

        // Probably an error code
        public byte UnknownField1 { get; set; }

        public string Message { get; set; }

        public NtcShut(byte unknownField1, string message) : base(PACKET_ID)
        {
            UnknownField1 = unknownField1;

            Debug.Assert(!string.IsNullOrEmpty(message) && message.Length < 0x200);
            Message = message;
        }

        public NtcShut(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField1);
            writer.Write(Message);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField1 = reader.ReadByte();
            Message = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}\n\tMessage: {Message}";
        }
    }
}
