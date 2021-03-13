using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHTriServer.Server
{
    public class LayerUserData : CompoundList
    {
        private const byte FIELD_1 = 0x01;
        private const byte FIELD_2 = 0x02;
        private const byte FIELD_3 = 0x03;
        private const byte FIELD_6 = 0x06;
        private const byte FIELD_7 = 0x07;

        public string UnknownField {
            get => Get<string>(FIELD_1);
            set {
                Debug.Assert(value.Length < 8);
                Set(FIELD_1, value);
            }
        }


        public string UnknownField2 {
            get => Get<string>(FIELD_2);
            set {
                Debug.Assert(value.Length < 0x20);
                Set(FIELD_2, value);
            }
        }

        public uint UnknownField6 {
            get => Get<uint>(FIELD_6);
            set => Set(FIELD_6, value);
        }

        public byte[] UnknownField7 {
            get => Get<byte[]>(FIELD_7);
            set {
                Debug.Assert(value.Length < 0x100);
                Set(FIELD_7, value);
            }
        }
    }
}
