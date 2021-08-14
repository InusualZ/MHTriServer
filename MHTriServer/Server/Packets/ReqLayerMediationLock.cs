using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerMediationLock : Packet
    {
        public const uint PACKET_ID = 0x64800100;

        public byte UnknownField1 { get; private set; }

        public MediationData LockData { get; private set; }

        public ReqLayerMediationLock(byte unknownField1, MediationData lockData) : base(PACKET_ID) => (UnknownField1, LockData) = (unknownField1, lockData);

        public ReqLayerMediationLock(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(UnknownField1);
            LockData.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            UnknownField1 = reader.ReadByte();
            LockData = CompoundList.Deserialize<MediationData>(reader);
        }
    }
}
