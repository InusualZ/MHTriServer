using System.Collections.Generic;
using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsBlackList : Packet
    {
        public const uint PACKET_ID = 0x66620200;

        private List<FriendData> m_friends;

        public IReadOnlyList<FriendData> Friends => m_friends;

        public AnsBlackList(List<FriendData> friends) : base(PACKET_ID) => m_friends = friends;

        public AnsBlackList(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((uint)0); // This field is read, but not used.
            writer.Write((uint)m_friends.Count);

            foreach (var slot in m_friends)
            {
                slot.Serialize(writer);
            }
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            _ = reader.ReadUInt32();

            var friendListCount = reader.ReadUInt32();
            m_friends = new List<FriendData>((int)friendListCount);
            for (var i = 0; i < friendListCount; ++i)
            {
                m_friends.Add(CompoundList.Deserialize<FriendData>(reader));
            }
        }
    }
}
