using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsUserListHead : Packet
    {
        public const uint PACKET_ID = 0x61100200;

        public uint UnknownField { get; private set; }

        public uint UserListSize { get; private set; }

        public AnsUserListHead(uint unknownField, uint userListSize) : base(PACKET_ID) => (UnknownField, UserListSize) = (unknownField, userListSize);

        public AnsUserListHead(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            writer.Write(UserListSize);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Debug.Assert(Size == 8);

            UnknownField = reader.ReadUInt32();
            UserListSize = reader.ReadUInt32();
        }
    }
}
