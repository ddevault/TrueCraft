using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TrueCraft.Core.Networking
{
    public class BufferManager
    {
        private readonly object _bufferLocker = new object();

        private readonly List<byte[]> _buffers;

        private readonly int _bufferSize;

        private readonly Stack<int> _availableBuffers;

        public BufferManager(int bufferSize)
        {
            _bufferSize = bufferSize;
            _buffers = new List<byte[]>();
            _availableBuffers = new Stack<int>();
        }

        public void SetBuffer(SocketAsyncEventArgs args)
        {
            if (_availableBuffers.Count > 0)
            {
                int index = _availableBuffers.Pop();

                byte[] buffer;
                lock (_bufferLocker)
                {
                    buffer = _buffers[index];
                }

                args.SetBuffer(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = new byte[_bufferSize];

                lock (_bufferLocker)
                {
                    _buffers.Add(buffer);
                }

                args.SetBuffer(buffer, 0, buffer.Length);
            }
        }

        public void ClearBuffer(SocketAsyncEventArgs args)
        {
            int index;
            lock (_bufferLocker)
            {
                index = _buffers.IndexOf(args.Buffer);
            }

            if (index >= 0)
                _availableBuffers.Push(index);

            args.SetBuffer(null, 0, 0);
        }
    }
}
