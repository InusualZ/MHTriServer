using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace MHTriServer.Utils
{
    /// <summary>
    /// Big-endian binary reader
    /// </summary>
    public class BEBinaryReader : BinaryReader
    {
        private readonly Encoding m_Encoding;
        private readonly byte[] m_buffer = new byte[sizeof(ulong)];

        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public BEBinaryReader(Stream input) : this(input, new ASCIIEncoding(), false) {}

        public BEBinaryReader(Stream input, Encoding encoding) : this(input, encoding, false) {}

        public BEBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
            m_Encoding = encoding;
        }

        public override ushort ReadUInt16()
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(ushort));
            Read(span);
            return BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public override short ReadInt16()
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(short));
            Read(span);
            return BinaryPrimitives.ReadInt16BigEndian(span);
        }


        public override uint ReadUInt32()
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(uint));
            Read(span);
            return BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public override int ReadInt32() 
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(int));
            Read(span);
            return BinaryPrimitives.ReadInt32BigEndian(span);
        }


        public override long ReadInt64() 
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(long));
            Read(span);
            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public override ulong ReadUInt64() 
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(ulong));
            Read(span);
            return BinaryPrimitives.ReadUInt64BigEndian(span);
        }


        public override float ReadSingle()
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(float));
            Read(span);
            span.Reverse();

            return BitConverter.ToSingle(span);
        }

        public override double ReadDouble()
        {
            var span = new Span<byte>(m_buffer, 0, sizeof(double));
            Read(span);
            span.Reverse();

            return BitConverter.ToDouble(span);
        }


        public override decimal ReadDecimal() => throw new NotImplementedException();

        public override string ReadString()
        {
            var length = ReadUInt16();
            if (length > 0)
            {
                var chars = ReadBytes(length);
                return m_Encoding.GetString(chars);
            }
            return string.Empty;
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
}
