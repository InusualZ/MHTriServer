using System.Diagnostics;

namespace MHTriServer.Server
{
    public class FriendData : CompoundList
    {
        private const byte INDEX_FIELD = 0x01;
        private const byte ID_FIELD = 0x02;
        private const byte NAME_FIELD = 0x03;

        public uint Index { get => Get<uint>(INDEX_FIELD); set => Set(INDEX_FIELD, value); }

        public string ID {
            get => Get<string>(ID_FIELD);
            set {
                Debug.Assert(value.Length < 8);
                Set(ID_FIELD, value);
            }
        }

        public string Name {
            get => Get<string>(NAME_FIELD);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(NAME_FIELD, value);
            }
        }

        public FriendData() { }

        public FriendData(uint index, string uniqueId, string name) 
            => (Index, ID, Name) = (index, uniqueId, name);
    }
}
