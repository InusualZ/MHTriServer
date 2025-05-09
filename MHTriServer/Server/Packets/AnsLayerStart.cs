﻿using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerStart : Packet
    {
        public const uint PACKET_ID = 0x64010200;

        public LayerData Data { get; set; }

        public AnsLayerStart(LayerData data) : base(PACKET_ID) => Data = data;

        public AnsLayerStart(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            CompoundList.Deserialize<LayerData>(reader);
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tLayerData {Data}";
        }
    }
}
