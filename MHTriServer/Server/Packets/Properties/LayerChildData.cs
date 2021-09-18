using System.Collections.Generic;

namespace MHTriServer.Server.Packets.Properties
{
    public class LayerChildData
    {
        public LayerData ChildData { get; set; }

        public List<UnkByteIntStruct> UnknownField2 { get; set; }
    }
}
