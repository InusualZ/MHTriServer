using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    class AnsAnnounce : Packet
    {
        public const uint PACKET_ID = 0x62300200;

        public string AnnouncementMessage { get; set; }

        public AnsAnnounce(string message) : base(PACKET_ID) => AnnouncementMessage = message;

        public AnsAnnounce(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(AnnouncementMessage);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            AnnouncementMessage = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tAnnouncement {AnnouncementMessage}";
        }
    }
}
