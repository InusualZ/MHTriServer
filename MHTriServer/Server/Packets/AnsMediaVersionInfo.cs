using System.Diagnostics;

namespace MHTriServer.Server.Packets
{
    public class AnsMediaVersionInfo : Packet
    {
        private const byte UNKNOWN_FIELD_2 = 2;
        public const uint PACKET_ID = 0x62410200;

        public string MediaVersion { get; private set; }

        public string PatchMessage { get; private set; }

        public string UnknownField { get; private set; }

        public AnsMediaVersionInfo(string mediaVersion, string patchMessage, string unknownField) : base(PACKET_ID)
            => (MediaVersion, PatchMessage, UnknownField) = (mediaVersion, patchMessage, unknownField);

        public AnsMediaVersionInfo(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(ExtendedBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(MediaVersion);
            writer.Write(PatchMessage);

            // This is how the client does it. It's wasteful I know.
            var compoundData = new CompoundList();
            compoundData.Set(UNKNOWN_FIELD_2, UnknownField);
            compoundData.Serialize(writer);
        }

        public override void Deserialize(ExtendedBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);
            MediaVersion = reader.ReadString();
            PatchMessage = reader.ReadString();

            // This is how the client does it. It's wasteful I know.
            var compoundData = CompoundList.Deserialize<CompoundList>(reader);
            UnknownField = compoundData.Get<string>(UNKNOWN_FIELD_2);
        }

        public override string ToString()
        {
            return base.ToString() + $"\n\tMediaVersion {MediaVersion}\n\tPatchMessage {PatchMessage}\n\tUnknownField {UnknownField}";
        }
    }
}
