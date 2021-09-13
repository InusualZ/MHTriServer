using System.Collections.Generic;
using System.Diagnostics;
using MHTriServer.Utils;

namespace MHTriServer.Server.Packets
{
    public class AnsCircleListLayer : Packet
    {
        public const uint PACKET_ID = 0x65270200;

        public List<CircleListData> Data { get; private set; }

        public AnsCircleListLayer(List<CircleListData> data) : base(PACKET_ID) => Data = data;

        public AnsCircleListLayer(uint id, ushort size, ushort counter) : base(id, size, counter) { }

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

            Data = new List<CircleListData>((int)listCount);
            for (var i = 0; i < listCount; ++i)
            {
                Data.Add(new CircleListData()
                {
                    ChildData = CompoundList.Deserialize<CircleData>(reader),
                    UnknownField2 = UnkByteIntStruct.DeserializeArray(reader)
                });
            }
        }
    }
}
