using System.Collections.Generic;

namespace MHTriServer.Server.Packets.Properties
{
    public class LayerUserListData
    {
        public LayerUserData ChildData { get; set; }

        public List<UnkByteIntStruct> UnknownField2 { get; set; }
    }
}
