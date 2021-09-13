using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    /*
     * It seems this packet is only sent when the client is connected to the OPN Server
     * And the first packet that is sent is the NtcLogin with `0x03` as first parameter
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
    public class ReqMediaVersionInfo : Packet
    {
        public const uint PACKET_ID = 0x62410100;

        public ReqMediaVersionInfo() : base(PACKET_ID) { }

        public ReqMediaVersionInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
        }
    }
}
