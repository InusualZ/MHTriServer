using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcCollectionLog : Packet
    {
        public const uint PACKET_ID = 0x60501000;

        public CollectionLog Data { get; set; }

        public NtcCollectionLog(CollectionLog data) : base(PACKET_ID) => Data = data;

        public NtcCollectionLog(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            Data.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Data = CompoundList.Deserialize<CollectionLog>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession) =>
            handler.HandleNtcCollectionLog(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + ":\n" + Data.ToString();
        }

        public class CollectionLog : CompoundList 
        {
            private const byte ERROR_CODE_FIELD = 0x01;
            private const byte FIELD_2 = 0x02;
            private const byte TIMEOUT_FIELD = 0x03;

            public uint ErrorCode { get => Get<uint>(ERROR_CODE_FIELD); set => Set(ERROR_CODE_FIELD, value); }

            public uint UnknownField2 { get => Get<uint>(FIELD_2); set => Set(FIELD_2, value); }

            public uint Timeout { get => Get<uint>(TIMEOUT_FIELD); set => Set(TIMEOUT_FIELD, value); }
        }
    }
}
