using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TrueCraft.Core.Networking
{
    public class BufferManager
    {
        private readonly object bufferLocker = new object();

        private readonly List<byte[]> buffers;

        private readonly int bufferSize;

        private readonly Stack<int> availableBuffers;

        public BufferManager(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffers = new List<byte[]>();
            availableBuffers = new Stack<int>();
        }

        public void SetBuffer(SocketAsyncEventArgs args)
        {
            if (availableBuffers.Count > 0)
            {
                int index = availableBuffers.Pop();

                byte[] buffer;
                lock (bufferLocker)
                {
                    buffer = buffers[index];
                }

                args.SetBuffer(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = new byte[bufferSize];

                lock (bufferLocker)
                {
                    buffers.Add(buffer);
                }

                args.SetBuffer(buffer, 0, buffer.Length);
            }
        }

        public void ClearBuffer(SocketAsyncEventArgs args)
        {
            int index;
            lock (bufferLocker)
            {
                index = buffers.IndexOf(args.Buffer);
            }

            if (index >= 0)
                availableBuffers.Push(index);

            args.SetBuffer(null, 0, 0);
        }
    }
}
