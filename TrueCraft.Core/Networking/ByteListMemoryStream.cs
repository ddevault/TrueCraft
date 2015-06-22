using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Networking
{
    public class ByteListMemoryStream : Stream
    {
        private long position;
        private readonly List<byte> buffer;

        public ByteListMemoryStream() : this(new List<byte>())
        {
        }

        public ByteListMemoryStream(List<byte> buffer, int offset = 0)
        {
            position = offset;
            this.buffer = buffer;
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                position = offset;
            else if (origin == SeekOrigin.Current)
                position += offset;
            else //End
                position = (buffer.Count - 1) - offset;

            return position;
        }

        public override void SetLength(long value)
        {
            buffer.RemoveRange((int)value, buffer.Count - (int)value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset)
                throw new ArgumentOutOfRangeException("offset");

            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count");

            byte[] buf = this.buffer.Skip((int)position).Take(count).ToArray();
            
            Buffer.BlockCopy(buf, 0, buffer, offset, buf.Length);

            position += Math.Min(count, buf.Length);
            
            return Math.Min(count, buf.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset)
                throw new ArgumentOutOfRangeException("offset");

            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count");

            this.buffer.AddRange(buffer.Skip(offset).Take(count));
            position += count;
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
                return buffer.Count;
            }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}
