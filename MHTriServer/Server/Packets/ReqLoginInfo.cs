using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLoginInfo : Packet
    {
        public const uint PACKET_ID = 0x61010100;

        public CompoundList Data { get; set; }

        public ReqLoginInfo(CompoundList data) : base(PACKET_ID) => (Data) = (data);

        public ReqLoginInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Data = CompoundList.Deserialize<CompoundList>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tData\n{Data}";
        }
    }
}
