﻿using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerIn : Packet
    {
        public const uint PACKET_ID = 0x64141000;

        public string CapcomID { get; private set; }

        public LayerUserData UserData { get; private set; }

        public NtcLayerIn(string capcomID, LayerUserData userData) : base(PACKET_ID) => (CapcomID, UserData) = (capcomID, userData);

        public NtcLayerIn(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(CapcomID);
            UserData.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            CapcomID = reader.ReadString();
            UserData = CompoundList.Deserialize<LayerUserData>(reader);
        }
    }
}
