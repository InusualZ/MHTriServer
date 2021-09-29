using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace MHTriServer.Utils
{

    /// <summary>
    /// Big-endian binary writer
    /// </summary>
    public class BEBinaryWriter : BinaryWriter
    {
        private readonly byte[] m_buffer = new byte[sizeof(ulong)];
        private readonly Encoding m_Encoding;

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public BEBinaryWriter(Stream input) : this(input, Encoding.ASCII, false) { }

        public BEBinaryWriter(Stream input, Encoding encoding) : this(input, encoding, false) { }

        public BEBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
            m_Encoding = encoding;
        }

        public override void Write(ushort value) => WriteUInt16(value);
        public override void Write(uint value) => WriteUInt32(value);
        public override void Write(ulong value) => WriteUInt64(value);
        public override void Write(short value) => WriteInt16(value);
        public override void Write(int value) => WriteInt32(value);
        public override void Write(long value) => WriteInt64(value);
        public override void Write(float value) => WriteSingle(value);
        public override void Write(double value) => WriteDouble(value);

        public override void Write(decimal value) => throw new NotImplementedException();

        public void WriteUInt16(ushort value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(ushort));
        }

        public void WriteUInt32(uint value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(uint));
        }

        public void WriteUInt64(ulong value)
        {
            BinaryPrimitives.WriteUInt64BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(ulong));
        }

        public void WriteInt16(short value)
        {
            BinaryPrimitives.WriteInt16BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(short));
        }

        public void WriteInt32(int value)
        {
            BinaryPrimitives.WriteInt32BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(int));
        }

        public void WriteInt64(long value)
        {
            BinaryPrimitives.WriteInt64BigEndian(m_buffer, value);
            base.Write(m_buffer, 0, sizeof(long));
        }

        public void WriteSingle(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            base.Write(bytes, 0, bytes.Length);
        }

        public void WriteDouble(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);

            base.Write(bytes, 0, bytes.Length);
        }

        // TODO: Better name
        public void WriteShortBytes(byte[] values)
        {
            var length = (ushort)Math.Min(values.Length, ushort.MaxValue);
            Write(length);
            if (length > 0)
            {
                Write(values, 0, length);
            }
        }

        // TODO: better name
        public void WriteByteBytes(byte[] values)
        {
            var length = (byte)Math.Min(values.Length, byte.MaxValue);
            Write(length);
            if (length > 0)
            {
                Write(values, 0, length);
            }
        }

        public override void Write(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteUInt16(0);
                return;
            }

            WriteUInt16((ushort)Math.Min(value.Length, ushort.MaxValue - 1 /* null char */));
            var bytes = m_Encoding.GetBytes(value);
            Write(bytes);
        }
    }
}
