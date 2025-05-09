﻿using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsUserBinarySet : Packet
    {
        public const uint PACKET_ID = 0x66310200;

        public AnsUserBinarySet() : base(PACKET_ID) { }

        public AnsUserBinarySet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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
