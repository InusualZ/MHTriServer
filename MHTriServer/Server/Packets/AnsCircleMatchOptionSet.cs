﻿using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleMatchOptionSet : Packet
    {
        public const uint PACKET_ID = 0x65100200;

        public AnsCircleMatchOptionSet() : base(PACKET_ID) { }

        public AnsCircleMatchOptionSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
        }
    }
}
