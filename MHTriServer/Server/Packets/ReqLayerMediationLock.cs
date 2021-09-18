using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class ReqLayerMediationLock : Packet
    {
        public const uint PACKET_ID = 0x64800100;

        public byte UnknownField1 { get; private set; }

        public MediationData LockData { get; private set; }

        public ReqLayerMediationLock(byte unknownField1, MediationData lockData) : base(PACKET_ID) => (UnknownField1, LockData) = (unknownField1, lockData);

        public ReqLayerMediationLock(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(UnknownField1);
            LockData.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            UnknownField1 = reader.ReadByte();
            LockData = CompoundList.Deserialize<MediationData>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleReqLayerMediationLock(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField1 {UnknownField1}\n\tLockData\n{LockData}";
        }
    }
}
