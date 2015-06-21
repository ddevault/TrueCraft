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
        private readonly BlockingCollection<SocketAsyncEventArgs> _argsPool;

        private readonly int _maxPoolSize;

        private BufferManager _bufferManager;

        public SocketAsyncEventArgsPool(int poolSize, int maxSize, int bufferSize)
        {
            _maxPoolSize = maxSize;
            _argsPool = new BlockingCollection<SocketAsyncEventArgs>(new ConcurrentQueue<SocketAsyncEventArgs>());
            _bufferManager = new BufferManager(bufferSize);

            Init(poolSize);
        }

        private void Init(int size)
        {
            for (int i = 0; i < size; i++)
            {
                _argsPool.Add(CreateEventArgs());
            }
        }

        public SocketAsyncEventArgs Get()
        {
            SocketAsyncEventArgs args;
            if (!_argsPool.TryTake(out args))
            {
                args = CreateEventArgs();
            }

            if (_argsPool.Count > _maxPoolSize)
            {
                Trim(_argsPool.Count - _maxPoolSize);
            }

            return args;
        }

        public void Add(SocketAsyncEventArgs args)
        {
            if (!_argsPool.IsAddingCompleted)
                _argsPool.Add(args);
        }

        protected SocketAsyncEventArgs CreateEventArgs()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            _bufferManager.SetBuffer(args);

            return args;
        }

        public void Trim(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SocketAsyncEventArgs args;

                if (_argsPool.TryTake(out args))
                {
                    _bufferManager.ClearBuffer(args);
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
                _argsPool.CompleteAdding();

                while (_argsPool.Count > 0)
                {
                    SocketAsyncEventArgs arg = _argsPool.Take();

                    _bufferManager.ClearBuffer(arg);
                    arg.Dispose();
                }
            }

            _bufferManager = null;
        }

        ~SocketAsyncEventArgsPool()
        {
            Dispose(false);
        }
    }
}
