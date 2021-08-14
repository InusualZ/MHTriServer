using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqUserStatusSet : Packet
    {
        public const uint PACKET_ID = 0x66400100;

        public List<UnkByteIntStruct> UserStatus { get; private set; }

        public ReqUserStatusSet(List<UnkByteIntStruct> userStatus) : base(PACKET_ID) => (UserStatus) = (userStatus);

        public ReqUserStatusSet(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            UnkByteIntStruct.SerializeArray(UserStatus, writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UserStatus = UnkByteIntStruct.DeserializeArray(reader);
        }

        public override string ToString()
        {
            var str = $":\n\tUserStatus({UserStatus.Count})";
            for (var i = 0; i < UserStatus.Count; ++i)
            {
                var data = UserStatus[i];
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
