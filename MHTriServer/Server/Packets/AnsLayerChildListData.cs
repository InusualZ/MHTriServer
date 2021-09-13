using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsLayerChildListData : Packet
    {
        public const uint PACKET_ID = 0x64250200;

        public List<LayerChildData> Data { get; private set; }

        public AnsLayerChildListData(List<LayerChildData> data) : base(PACKET_ID) => Data = data;

        public AnsLayerChildListData(uint id, ushort size, ushort counter) : base(id, size, counter) { }

        public override void Serialize(BEBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write((uint)0); // This field is read, but not used.
            writer.Write((uint) Data.Count);

            foreach(var data in Data)
            {
                data.ChildData.Serialize(writer);
                UnkByteIntStruct.SerializeArray(data.UnknownField2, writer);
            }
        }

        public override void Deserialize(BEBinaryReader reader)
        {
            Debug.Assert(ID == PACKET_ID);

            _ = reader.ReadUInt32();
            var listCount = reader.ReadUInt32();

            Data = new List<LayerChildData>((int)listCount);
            for (var i = 0; i < listCount; ++i)
            {
                Data.Add(new LayerChildData()
                {
                    ChildData = CompoundList.Deserialize<LayerData>(reader),
                    UnknownField2 = UnkByteIntStruct.DeserializeArray(reader)
                });
            }
        }
    }
}
