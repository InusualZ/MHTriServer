using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    /*
     * It seems this packet is only sent when the client is connected to the OPN Server
     * And the first packet that is sent is the NtcLogin with `0x03` as first parameter
     * TODO: Find more meaningful name, this name was given because of the context in which the
     * packet get sent.
     */
    public class AnsLoginSuccess : Packet
    {
        public const uint PACKET_ID = 0x62410100;

        public AnsLoginSuccess() : base(PACKET_ID) { }

        public AnsLoginSuccess(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
        }
    }
}
