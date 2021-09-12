using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserSearchSet : Packet
    {
        public const uint PACKET_ID = 0x66300100;

        public List<UnkByteIntStruct> UnknownField { get; private set; }

        public ReqUserSearchSet(List<UnkByteIntStruct> unknownField) : base(PACKET_ID) => (UnknownField) = (unknownField);

        public ReqUserSearchSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            UnkByteIntStruct.SerializeArray(UnknownField, writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField({UnknownField.Count})";
            for(var i = 0; i < UnknownField.Count; ++i)
            {
                var data = UnknownField[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField}";
                if (data.ContainUnknownField3)
                {
                    str += $"\n\t    UnknownField2 {data.UnknownField3}";
                }
            }
            return base.ToString() + str;
        }
    }
}
