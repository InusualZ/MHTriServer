﻿using System.Diagnostics;
using MHTriServer.Server.Packets.Properties;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class NtcLayerBinary : Packet
    {
        public const uint PACKET_ID = 0x64701000;

        public string CapcomID { get; private set; }

        public NtcBinaryCompoundData UnknownField2 { get; private set; }

        public ushort UnknownField3 { get; private set; }

        public CompoundExtendedList UnknownField4 { get; private set; }

        public NtcLayerBinary(string unknownField1, NtcBinaryCompoundData unknownField2, ushort unknownField3, CompoundExtendedList unknownField4) : base(PACKET_ID)
            => (CapcomID, UnknownField2, UnknownField3, UnknownField4) = (unknownField1, unknownField2, unknownField3, unknownField4);

        public NtcLayerBinary(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            // TODO: Struct is not correct
            base.Serialize(writer);

            writer.Write(CapcomID);
            UnknownField2.Serialize(writer);

            writer.Write(UnknownField3);
            UnknownField4.Serialize(writer);
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            UnknownField2 = CompoundList.Deserialize<NtcBinaryCompoundData>(reader);
            UnknownField3 = reader.ReadUInt16();
            UnknownField4 = CompoundExtendedList.Deserialize<CompoundExtendedList>(reader);
        }

        public override void Handle(PacketHandler handler, NetworkSession networkSession)
            => handler.HandleNtcLayerBinary(networkSession, this);

        public override string ToString()
        {
            return base.ToString() + $"\n\tUnknownField1 {CapcomID}\n\tUnknownField2 {UnknownField2}\n\tUnknownField3 {UnknownField3}" +
                $"\n\tUnknownField4\n{UnknownField4}";
        }
    }
}
