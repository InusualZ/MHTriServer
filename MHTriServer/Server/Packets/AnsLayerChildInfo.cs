using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerChildInfo : Packet
    {
        public const uint PACKET_ID = 0x64230200;

        public ushort UnknownField { get; private set; }

        public LayerData UnknownField2 { get; private set; }

        // public byte[] UnknownField3 { get; private set; }

        public AnsLayerChildInfo(ushort unknownField, LayerData layerData) : base(PACKET_ID)
            => (UnknownField, UnknownField2) = (unknownField, layerData);

        public AnsLayerChildInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(UnknownField);
            UnknownField2.Serialize(writer);

            // FIXME: Data would be left unwritten
            writer.Write((byte)0);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField = reader.ReadUInt16();
            UnknownField2 = CompoundList.Deserialize<LayerData>(reader);
            // FIXME: Data would be left unread
        }

        public override string ToString()
        {
            return base.ToString() + $":\n\tUnknownField {UnknownField}\n\tUnknownField2({UnknownField2})";
        }
    }
}
