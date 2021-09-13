using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsBinaryData : Packet
    {
        public const uint PACKET_ID = 0x63030200;

        public uint Version { get; private set; }

        public uint Offset { get; private set; }

        public uint DataSize { get; private set; }

        public byte[] Data { get; private set; }

        public AnsBinaryData(uint version, uint offset, byte[] data) : base(PACKET_ID) 
            => (Version, Offset, DataSize, Data) = (version, offset, (uint)data.Length, data);

        public AnsBinaryData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
            writer.Write(Offset);
            writer.Write(DataSize);
            writer.WriteShortBytes(Data);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            Version = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            DataSize = reader.ReadUInt32();
            Data = reader.ReadShortBytes();
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tVersion {Version}\n\tOffset {Offset}\n\tSize {DataSize}" +
                $"\n\tData {Packet.Hexstring(Data, ' ')}";
        }
    }
}
