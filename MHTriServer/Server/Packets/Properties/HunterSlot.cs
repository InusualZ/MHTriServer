using System.Diagnostics;

namespace MHTriServer.Server.Packets.Properties
{
    public class HunterSlot : CompoundList
    {
        public const byte SLOT_INDEX_FIELD = 0x01;
        public const byte SAVE_ID_FIELD = 0x02;
        public const byte HUNTER_NAME_FIELD = 0x03; // Fill
        public const byte FIELD_4 = 0x04; // Fill
        public const byte FIELD_5 = 0x05; // Fill
        public const byte FIELD_6 = 0x06; // Fill
        public const byte RANK_FIELD = 0x07;
        public const byte FIELD_8 = 0x08;

        public uint SlotIndex { get => Get<uint>(SLOT_INDEX_FIELD); set => Set(SLOT_INDEX_FIELD, value); }

        public string SaveID 
        { 
            get => Get<string>(SAVE_ID_FIELD); 
            set {
                Debug.Assert(value.Length < 8);
                Set(SAVE_ID_FIELD, value);
            } 
        }

        public string HunterName 
        { 
            get => Get<string>(HUNTER_NAME_FIELD);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(HUNTER_NAME_FIELD, value);
            } 
        }

        public uint UnknownField4 { get => Get<uint>(FIELD_4); set => Set(FIELD_4, value); }

        public uint UnknownField5 { get => Get<uint>(FIELD_5); set => Set(FIELD_5, value); }

        public uint UnknownField6 { get => Get<uint>(FIELD_6); set => Set(FIELD_6, value); }

        public uint Rank { get => Get<uint>(RANK_FIELD); set => Set(RANK_FIELD, value); }

        public string UnknownField8 
        { 
            get => Get<string>(FIELD_8);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(FIELD_8, value);
            }
        }

        public static HunterSlot NoData(uint slotIndex)
        {
            return new HunterSlot() { 
                SlotIndex = slotIndex,
                SaveID = "******"
            };
        }
    }
}
