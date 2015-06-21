using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Collections
{
    public class ByteArraySegment : ICollection<byte>
    {
        private readonly byte[] _array;
        private readonly int _start;
        private readonly int _count;

        public ByteArraySegment(byte[] array, int start, int count)
        {
            _array = array;
            _start = start;
            _count = count;
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
            return _array.Contains(item);
        }

        public void CopyTo(byte[] target, int index)
        {
            Buffer.BlockCopy(_array, _start, target, index, _count);
        }

        public bool Remove(byte item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return _count;
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
                return _array[index];
            }
            set
            {
                if (index > _array.Length)
                    throw new ArgumentOutOfRangeException("value");

                _array[index] = value;
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
            private byte _current;
            private int _pos;

            private readonly ByteArraySegment _segment;

            public ByteArraySegmentEnumerator(ByteArraySegment segment)
            {
                _segment = segment;
                _pos = segment._start;
            }

            public bool MoveNext()
            {
                if (_pos >= _segment.Count)
                    return false;

                _current = _segment._array[++_pos];

                return true;
            }

            public void Reset()
            {
                _pos = _segment._start;
            }

            public byte Current
            {
                get
                {
                    return _current;
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
