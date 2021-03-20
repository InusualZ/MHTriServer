﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class ReqAuthenticationToken : Packet
    {
        public const uint PACKET_ID = 0x62600100;

        public string Token { get; private set; }

        public ReqAuthenticationToken(string token) : base(PACKET_ID) => Token = token;

        public ReqAuthenticationToken(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Token);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size >= 2);
            Token = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tToken {Token}";
        }
    }
}