using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserSearchSet : Packet
    {
        public const uint PACKET_ID = 0x66300100;

        public class UserSearchData
        {
            public byte UnknownField { get; set; }

            public bool IsField2Set{ get; set; }

            public uint UnknownField2 { get; set; }
        } 

        public List<UserSearchData> UnknownField { get; private set; }

        public ReqUserSearchSet(List<UserSearchData> unknownField) : base(PACKET_ID) => (UnknownField) = (unknownField);

        public ReqUserSearchSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((byte)UnknownField.Count);
            foreach (var data in UnknownField)
            {
                writer.Write(data.UnknownField);
                writer.Write(data.IsField2Set);
                if (data.IsField2Set)
                {
                    writer.Write(data.UnknownField2);
                }
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            var count = reader.ReadByte();
            UnknownField = new List<UserSearchData>(count);
            for (var i = 0; i < count; ++i)
            {
                var data = new UserSearchData();
                data.UnknownField = reader.ReadByte();
                data.IsField2Set = reader.ReadBoolean();
                if (data.IsField2Set)
                {
                    data.UnknownField2 = reader.ReadUInt32();
                }

                UnknownField.Add(data);
            }
        }

        public override string ToString()
        {
            var str = $":\n\tUnknownField({UnknownField.Count})";
            for(var i = 0; i < UnknownField.Count; ++i)
            {
                var data = UnknownField[i];
                str += $"\n\t  [{i}] =>\n\t    UnknownField {data.UnknownField}";
                if (data.IsField2Set)
                {
                    str += $"\n\t    UnknownField2 {data.UnknownField2}";
                }
            }
            return base.ToString() + str;
        }
    }
}
