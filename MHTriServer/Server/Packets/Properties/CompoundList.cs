using MHTriServer.Utils;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MHTriServer.Server.Packets.Properties
{
    public class CompoundList
    {
        protected enum ElementType
        {
            None = 0,
            UInt8 = 1,
            UInt16 = 2,
            UInt32 = 3,
            UInt64 = 4,
            String = 5,
            Binary = 6
        }

        private readonly Dictionary<byte, object> m_Data;

        public int Count => m_Data.Count;

        public CompoundList()
        {
            m_Data = new Dictionary<byte, object>();
        }

        public void Clear() => m_Data.Clear();

        public bool ContainsKey(byte key) => m_Data.ContainsKey(key);

        public IEnumerator<KeyValuePair<byte, object>> GetEnumerator() => m_Data.GetEnumerator();

        public bool Remove(byte key) => m_Data.Remove(key);

        public T Get<T>(byte key, T value = default) => (T) m_Data.GetValueOrDefault(key, value);

        public void Set(byte key, byte value) => m_Data[key] = value;
        public void Set(byte key, ushort value) => m_Data[key] = value;
        public void Set(byte key, uint value) => m_Data[key] = value;
        public void Set(byte key, ulong value) => m_Data[key] = value;
        public void Set(byte key, string value) => m_Data[key] = value;
        public void Set(byte key, byte[] value) => m_Data[key] = value;

        /// <summary>
        /// You must override TryRead and TryWrite
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void Set(byte key, object value) => m_Data[key] = value;

        public bool TryGetValue<T>(byte key, [MaybeNullWhen(false)] out T value)
        {
            value = default;
            if (m_Data.TryGetValue(key, out var obj)) {
                if (obj is T val)
                {
                    value = val;
                    return true;
                }
            }

            return false;
        }


        public void Serialize(BEBinaryWriter writer)
        {
            writer.Write((byte)m_Data.Count);
            foreach (var (id, value) in m_Data)
            {
                writer.Write(id);
                if (!TryWrite(id, value, writer))
                {
                    writer.Write((byte)ElementType.None);
                    writer.Write((byte)0x00);
                }
            }
        }

        public void Deserialize(BEBinaryReader reader)
        {
            var listSize = (int)reader.ReadByte();
            for (var i  = 0; i < listSize; ++i)
            {
                var key = reader.ReadByte();
                var type = reader.ReadByte();
                if (!TryRead(key, type, reader, out var value))
                {
                    value = reader.ReadByte();
                }

                m_Data[key] = value;
            }
        }

        protected virtual bool TryRead(byte key, byte type, BEBinaryReader reader, out object value)
        {
            value = default;
            switch ((ElementType)type)
            {
                case ElementType.UInt8:
                    value = reader.ReadByte();
                    break;
                case ElementType.UInt16:
                    value = reader.ReadUInt16();
                    break;
                case ElementType.UInt32:
                    value = reader.ReadUInt32();
                    break;
                case ElementType.UInt64:
                    value = reader.ReadUInt64();
                    break;
                case ElementType.String:
                    value = reader.ReadString();
                    break;
                case ElementType.Binary:
                    value = reader.ReadShortBytes();
                    break;

                case ElementType.None:
                default:
                    return false;
            }

            return true;
        }

        protected virtual bool TryWrite(byte key, object value, BEBinaryWriter writer)
        {
            switch (value)
            {
                case byte b:
                    writer.Write((byte)ElementType.UInt8);
                    writer.Write(b);
                    break;
                case ushort s:
                    writer.Write((byte)ElementType.UInt16);
                    writer.Write(s);
                    break;
                case uint i:
                    writer.Write((byte)ElementType.UInt32);
                    writer.Write(i);
                    break;
                case ulong l:
                    writer.Write((byte)ElementType.UInt64);
                    writer.Write(l);
                    break;
                case string str:
                    writer.Write((byte)ElementType.String);
                    writer.Write(str);
                    break;
                case byte[] binary:
                    writer.Write((byte)ElementType.Binary);
                    writer.WriteShortBytes(binary);
                    break;

                default:
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach(var (key, value) in m_Data)
            {
                builder.Append($"\t  {{{(int)key}}}: ");
                switch (value)
                {
                    case byte b:
                        builder.AppendLine($"0x{b:X2}");
                        break;
                    case ushort s:
                        builder.AppendLine($"{s}");
                        break;
                    case uint i:
                        builder.AppendLine($"{i}");
                        break;
                    case ulong l:
                        builder.AppendLine($"{l}");
                        break;
                    case string str:
                        builder.AppendLine($"\"{str}\"");
                        break;
                    case byte[] binary:
                        builder.AppendLine($"b'{Packet.Hexstring(binary, ' ')}'");
                        break;

                    default:
                        builder.AppendLine($"{value.GetType().Name}");
                        break;
                }
            }
            return builder.ToString();
        }

        public static T Deserialize<T>(BEBinaryReader reader) where T : CompoundList, new()
        {
            var compoindList = new T();
            compoindList.Deserialize(reader);
            return compoindList;
        }
    }
}
