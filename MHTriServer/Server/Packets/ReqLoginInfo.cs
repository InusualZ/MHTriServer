using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    /*
     * It seems this packet is only sent when the client is connected to the LMP Server
     * And the first packet that is sent is the NtcLogin with `0x01` as first parameter
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
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
    }
}
