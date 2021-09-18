using System.Collections.Generic;

namespace MHTriServer.Server.Packets.Properties
{
    public class CircleListData
    {
        public CircleData ChildData { get; set; }

        public List<UnkByteIntStruct> UnknownField2 { get; set; }
    }
}
