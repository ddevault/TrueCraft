using System;
using System.Threading;
using System.Collections.Concurrent;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// Abstract base class for renderers of meshes.
    /// </summary>
    /// <typeparam name="T">The object to render into a mesh.</typeparam>
    public abstract class Renderer<T> : IDisposable
    {
        private readonly object _syncLock =
            new object();

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<RendererEventArgs<T>> MeshCompleted;

        private volatile bool _isRunning;
        private Thread _rendererThread;
        private ConcurrentQueue<T> _items, _priorityItems;
        private volatile bool _isDisposed;

        /// <summary>
        /// Gets whether this renderer is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);
                return _isRunning;
            }
        }

        /// <summary>
        /// Gets whether this renderer is disposed of.
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Renderer()
        {
            lock (_syncLock)
            {
                _isRunning = false;
                _rendererThread = new Thread(DoRendering);
                _items = new ConcurrentQueue<T>(); _priorityItems = new ConcurrentQueue<T>();
                _isDisposed = false;
            }
        }

        /// <summary>
        /// Starts this renderer.
        /// </summary>
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (_isRunning) return;
            lock (_syncLock)
            {
                _isRunning = true;
                _rendererThread.Start(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void DoRendering(object obj)
        {
            while (_isRunning)
            {
                var item = default(T);
                var result = default(Mesh);

                lock (_syncLock)
                {
                    if (_priorityItems.TryDequeue(out item) && TryRender(item, out result))
                    {
                        var args = new RendererEventArgs<T>(item, result, true);
                        if (MeshCompleted != null)
                            MeshCompleted(this, args);
                    }
                    else if (_items.TryDequeue(out item) && TryRender(item, out result))
                    {
                        var args = new RendererEventArgs<T>(item, result, false);
                        if (MeshCompleted != null)
                            MeshCompleted(this, args);
                    }
                }

                if (item == null) // We don't have any work, so sleep for a bit.
                    Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected abstract bool TryRender(T item, out Mesh result);

        /// <summary>
        /// Stops this renderer.
        /// </summary>
        public void Stop()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!_isRunning) return;
            lock (_syncLock)
            {
                _isRunning = false;
                _rendererThread.Join();
            }
        }

        /// <summary>
        /// Enqueues an item to this renderer for rendering.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="hasPriority"></param>
        public void Enqueue(T item, bool hasPriority = false)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!_isRunning) return;
            lock (_syncLock)
            {
                if (hasPriority)
                    _priorityItems.Enqueue(item);
                else
                    _items.Enqueue(item);
            }
        }

        /// <summary>
        /// Disposes of this renderer.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this renderer.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                lock (_syncLock)
                {
                    _rendererThread = null;
                    _items = null; _priorityItems = null;
                    _isDisposed = true;
                }
            }
            else
                throw new NotSupportedException(); // We should 'encourage' developers to dispose of renderers properly.
        }

        /// <summary>
        /// Finalizes this renderer.
        /// </summary>
        ~Renderer()
        {
            Dispose(false);
        }
    }
}
