using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerMediationLock : Packet
    {
        public const uint PACKET_ID = 0x64800200;

        public byte UnknownField1 { get; private set; }

        public AnsLayerMediationLock(byte unknownField1) : base(PACKET_ID) => (UnknownField1) = (unknownField1);

        public AnsLayerMediationLock(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(UnknownField1);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            UnknownField1 = reader.ReadByte();
        }
    }
}
