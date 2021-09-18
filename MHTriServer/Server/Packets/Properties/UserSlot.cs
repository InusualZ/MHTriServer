using System.Diagnostics;

namespace MHTriServer.Server.Packets.Properties
{
    public class UserSlot : CompoundList
    {
        private const byte SLOT_INDEX_FIELD = 0x01;
        private const byte SAVE_ID_FIELD = 0x02;
        private const byte CHARACTER_NAME_FIELD = 0x03; // Fill
        private const byte FIELD_4 = 0x04; // Fill
        private const byte FIELD_5 = 0x05; // Fill
        private const byte FIELD_6 = 0x06; // Fill
        private const byte FIELD_7 = 0x07; // Fill
        private const byte FIELD_8 = 0x08;

        public uint SlotIndex { get => Get<uint>(SLOT_INDEX_FIELD); set => Set(SLOT_INDEX_FIELD, value); }

        public string SaveID 
        { 
            get => Get<string>(SAVE_ID_FIELD); 
            set {
                Debug.Assert(value.Length < 8);
                Set(SAVE_ID_FIELD, value);
            } 
        }

        public string CharacterName 
        { 
            get => Get<string>(CHARACTER_NAME_FIELD);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(CHARACTER_NAME_FIELD, value);
            } 
        }

        public uint UnknownField4 { get => Get<uint>(FIELD_4); set => Set(FIELD_4, value); }

        public uint UnknownField5 { get => Get<uint>(FIELD_5); set => Set(FIELD_5, value); }

        public uint UnknownField6 { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        public uint UnknownField7 { get => Get<uint>(FIELD_7); set => Set(FIELD_7, value); }

        public string UnknownField8 
        { 
            get => Get<string>(FIELD_8);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(FIELD_8, value);
            }
        }

        public static UserSlot NoData(uint slotIndex)
        {
            return new UserSlot() { 
                SlotIndex = slotIndex,
                SaveID = "******"
            };
        }
    }
}
