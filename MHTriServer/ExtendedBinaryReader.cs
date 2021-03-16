using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace MHTriServer
{
    public enum Endianness
    {
        Little,
        Big,
    }

    public class ExtendedBinaryReader : BinaryReader
    {
        private readonly Endianness m_Endianness;
        private readonly Encoding m_Encoding;

        public ExtendedBinaryReader(Stream input) : this(input, new ASCIIEncoding(), false, Endianness.Little) {}

        public ExtendedBinaryReader(Stream input, Encoding encoding) : this(input, encoding, false, Endianness.Little) {}

        public ExtendedBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : 
            this(input, encoding, leaveOpen, Endianness.Little) {}

        public ExtendedBinaryReader(Stream input, Endianness endianness) : 
            this(input, new ASCIIEncoding(), false, endianness) {}

        public ExtendedBinaryReader(Stream input, Encoding encoding, Endianness endianness) :
            this(input, encoding, false, endianness) { }

        public ExtendedBinaryReader(Stream input, Encoding encoding, bool leaveOpen,
            Endianness endianness) : base(input, encoding, leaveOpen)
        {
            m_Endianness = endianness;
            m_Encoding = encoding;
        }

        public override short ReadInt16() => ReadInt16(m_Endianness);

        public override int ReadInt32() => ReadInt32(m_Endianness);

        public override long ReadInt64() => ReadInt64(m_Endianness);

        public override ushort ReadUInt16() => ReadUInt16(m_Endianness);

        public override uint ReadUInt32() => ReadUInt32(m_Endianness);

        public override ulong ReadUInt64() => ReadUInt64(m_Endianness);

        public short ReadInt16(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(sizeof(Int16)))
            : BinaryPrimitives.ReadInt16BigEndian(ReadBytes(sizeof(Int16)));

        public int ReadInt32(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadInt32LittleEndian(ReadBytes(sizeof(Int32)))
            : BinaryPrimitives.ReadInt32BigEndian(ReadBytes(sizeof(Int32)));

        public long ReadInt64(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(sizeof(Int64)))
            : BinaryPrimitives.ReadInt64BigEndian(ReadBytes(sizeof(Int64)));

        public ushort ReadUInt16(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(sizeof(UInt16)))
            : BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(sizeof(UInt16)));

        public uint ReadUInt32(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadUInt32LittleEndian(ReadBytes(sizeof(UInt32)))
            : BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(sizeof(UInt32)));

        public ulong ReadUInt64(Endianness endianness) => endianness == Endianness.Little
            ? BinaryPrimitives.ReadUInt64LittleEndian(ReadBytes(sizeof(UInt64)))
            : BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(sizeof(UInt64)));

        public override string ReadString()
        {
            var length = ReadUInt16(m_Endianness);
            var chars = ReadBytes(length);
            return m_Encoding.GetString(chars);
        }

        // TODO: Better name
        public byte[] ReadShortBytes()
        {
            var length = ReadUInt16();
            return ReadBytes(length);
        }

        // TODO: better name
        public byte[] ReadByteBytes()
        {
            var length = ReadByte();
            return ReadBytes(length);
        }
    }

    public class ExtendedBinaryWriter : BinaryWriter
    {
        private readonly Endianness m_Endianness = Endianness.Little;
        private readonly byte[] m_buffer = new byte[8];
        private readonly Encoding m_Encoding;

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public ExtendedBinaryWriter(Stream input) : this(input, new ASCIIEncoding(), false, Endianness.Little) { }

        public ExtendedBinaryWriter(Stream input, Encoding encoding) : this(input, encoding, false, Endianness.Little) { }

        public ExtendedBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) :
            this(input, encoding, leaveOpen, Endianness.Little) {}

        public ExtendedBinaryWriter(Stream input, Endianness endianness) : 
            this(input, new ASCIIEncoding(), false, endianness) { }

        public ExtendedBinaryWriter(Stream input, Encoding encoding, Endianness endianness) :
            this(input,encoding, false, endianness) { }

        public ExtendedBinaryWriter(Stream input, Encoding encoding, bool leaveOpen,
            Endianness endianness) : base(input, encoding, leaveOpen)
        {
            m_Endianness = endianness;
            m_Encoding = encoding;
        }

        public override void Write(ushort value) => WriteUInt16(m_Endianness, value);
        public override void Write(uint value) => WriteUInt32(m_Endianness, value);
        public override void Write(ulong value) => WriteUInt64(m_Endianness, value);
        public override void Write(short value) => WriteInt16(m_Endianness, value);
        public override void Write(int value) => WriteInt32(m_Endianness, value);
        public override void Write(long value) => WriteInt64(m_Endianness, value);

        //
        public void WriteUInt16(Endianness endianness, ushort value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt16BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(ushort));
        }

        public void WriteUInt32(Endianness endianness, uint value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(uint));
        }

        public void WriteUInt64(Endianness endianness, ulong value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(ulong));
        }

        public void WriteInt16(Endianness endianness, short value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt16LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteInt16BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(short));
        }

        public void WriteInt32(Endianness endianness, int value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt32LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(int));
        }

        public void WriteInt64(Endianness endianness, long value)
        {
            if (endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt64LittleEndian(m_buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(m_buffer, value);
            }

            base.Write(m_buffer, 0, sizeof(long));
        }

        // TODO: Better name
        public void WriteShortBytes(byte[] values)
        {
            var length = (ushort)Math.Min(values.Length, ushort.MaxValue);
            Write(length);
            Write(values, 0, length);
        }

        // TODO: better name
        public void WriteByteBytes(byte[] values)
        {
            var length = (byte)Math.Min(values.Length, byte.MaxValue);
            Write(length);
            Write(values, 0, length);
        }

        public override void Write(string value)
        {
            WriteUInt16(m_Endianness, (ushort)Math.Min(value.Length, ushort.MaxValue));
            if (value.Length > 0)
            {
                var bytes = m_Encoding.GetBytes(value);
                Write(bytes);
            }
        }
    }
}
