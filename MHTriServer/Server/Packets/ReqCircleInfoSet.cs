﻿using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqCircleInfoSet : Packet
    {
        public const uint PACKET_ID = 0x65200100;

        public uint CircleIndex { get; private set; }

        public CircleData UnknownField1 { get; private set; }

        public List<UnkByteIntStruct> UnknownField2 { get; private set; }

        public ReqCircleInfoSet(uint circleIndex, CircleData unknownField1, List<UnkByteIntStruct> unknownField2) : base(PACKET_ID)
            => (CircleIndex, UnknownField1, UnknownField2) = (circleIndex, unknownField1, unknownField2);

        public ReqCircleInfoSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(CircleIndex);
            UnknownField1.Serialize(writer);
            UnkByteIntStruct.SerializeArray(UnknownField2, writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            CircleIndex = reader.ReadUInt32();
            UnknownField1 =  CompoundList.Deserialize<CircleData>(reader);
            UnknownField2 = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqCircleInfoSet(networkSession, this);

        public override string ToString()
        {
            var str = $":\n\tCircleIndex {CircleIndex}\n\tUnknownField1 {UnknownField1}\n\tUnknownField2({UnknownField2.Count})";
            for (var i = 0; i < UnknownField2.Count; ++i)
            {
                var data = UnknownField2[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField}";
                if (data.ContainUnknownField3)
                {
                    str += $"\n\t    UnknownField3 {data.UnknownField3}";
                }
            }

            return base.ToString() + str;
        }
    }
}
