using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Collections
{
    public class ByteArraySegment : ICollection<byte>
    {
        private readonly byte[] array;
        private readonly int start;
        private readonly int count;

        public ByteArraySegment(byte[] array, int start, int count)
        {
            this.array = array;
            this.start = start;
            this.count = count;
        }

        public void Add(byte item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(byte item)
        {
            return array.Contains(item);
        }

        public void CopyTo(byte[] target, int index)
        {
            Buffer.BlockCopy(array, start, target, index, count);
        }

        public bool Remove(byte item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public byte this[int index]
        {
            get
            {
                return array[index];
            }
            set
            {
                if (index > array.Length)
                    throw new ArgumentOutOfRangeException("value");

                array[index] = value;
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return new ByteArraySegmentEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        class ByteArraySegmentEnumerator : IEnumerator<byte>
        {
            private byte current;
            private int pos;

            private readonly ByteArraySegment _segment;

            public ByteArraySegmentEnumerator(ByteArraySegment segment)
            {
                _segment = segment;
                pos = segment.start;
            }

            public bool MoveNext()
            {
                if (pos >= _segment.Count)
                    return false;

                current = _segment.array[++pos];

                return true;
            }

            public void Reset()
            {
                pos = _segment.start;
            }

            public byte Current
            {
                get
                {
                    return current;
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

        }
    }
}
