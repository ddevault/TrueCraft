using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Networking
{
    public class ByteListMemoryStream : Stream
    {
        private long _position;
        private readonly List<byte> _buffer;

        public ByteListMemoryStream() : this(new List<byte>())
        {
        }

        public ByteListMemoryStream(List<byte> buffer, int offset = 0)
        {
            _position = offset;
            _buffer = buffer;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                _position = offset;
            else if (origin == SeekOrigin.Current)
                _position += offset;
            else //End
                _position = (_buffer.Count - 1) - offset;

            return _position;
        }

        public override void SetLength(long value)
        {
            _buffer.RemoveRange((int)value, _buffer.Count - (int)value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset)
                throw new ArgumentOutOfRangeException("offset");

            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count");

            byte[] buf = _buffer.Skip((int)_position).Take(count).ToArray();
            
            Buffer.BlockCopy(buf, 0, buffer, offset, buf.Length);

            _position += Math.Min(count, buf.Length);
            
            return Math.Min(count, buf.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset)
                throw new ArgumentOutOfRangeException("offset");

            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count");

            _buffer.AddRange(buffer.Skip(offset).Take(count));
            _position += count;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return _buffer.Count;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
    }
}
