using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TrueCraft.Core.Networking
{
    public class SocketAsyncEventArgsPool : IDisposable
    {
        private readonly BlockingCollection<SocketAsyncEventArgs> argsPool;

        private readonly int maxPoolSize;

        private BufferManager bufferManager;

        public SocketAsyncEventArgsPool(int poolSize, int maxSize, int bufferSize)
        {
            maxPoolSize = maxSize;
            argsPool = new BlockingCollection<SocketAsyncEventArgs>(new ConcurrentQueue<SocketAsyncEventArgs>());
            bufferManager = new BufferManager(bufferSize);

            Init(poolSize);
        }

        private void Init(int size)
        {
            for (int i = 0; i < size; i++)
            {
                argsPool.Add(CreateEventArgs());
            }
        }

        public SocketAsyncEventArgs Get()
        {
            SocketAsyncEventArgs args;
            if (!argsPool.TryTake(out args))
            {
                args = CreateEventArgs();
            }

            if (argsPool.Count > maxPoolSize)
            {
                Trim(argsPool.Count - maxPoolSize);
            }

            return args;
        }

        public void Add(SocketAsyncEventArgs args)
        {
            if (!argsPool.IsAddingCompleted)
                argsPool.Add(args);
        }

        protected SocketAsyncEventArgs CreateEventArgs()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            bufferManager.SetBuffer(args);

            return args;
        }

        public void Trim(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SocketAsyncEventArgs args;

                if (argsPool.TryTake(out args))
                {
                    bufferManager.ClearBuffer(args);
                    args.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                argsPool.CompleteAdding();

                while (argsPool.Count > 0)
                {
                    SocketAsyncEventArgs arg = argsPool.Take();

                    bufferManager.ClearBuffer(arg);
                    arg.Dispose();
                }
            }

            bufferManager = null;
        }

        ~SocketAsyncEventArgsPool()
        {
            Dispose(false);
        }
    }
}
