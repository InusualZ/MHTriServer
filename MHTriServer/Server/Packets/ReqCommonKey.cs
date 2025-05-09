﻿using System.Diagnostics;

using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqCommonKey : Packet
    {
        public const uint PACKET_ID = 0x60700100;

        public ReqCommonKey() : base(PACKET_ID) { }

        public ReqCommonKey(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqCommonKey(networkSession, this);

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 0);
        }
    }
}
