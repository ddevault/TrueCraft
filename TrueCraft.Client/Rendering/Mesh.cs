using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Linq;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// Represents an indexed collection of data that can be rendered.
    /// </summary>
    public class Mesh : IDisposable
    {
        public static int VerticiesRendered { get; set; }
        public static int IndiciesRendered { get; set; }

        public static void ResetStats()
        {
            VerticiesRendered = 0;
            IndiciesRendered = 0;
        }

        /// <summary>
        /// The maximum number of submeshes stored in a single mesh.
        /// </summary>
        public const int SubmeshLimit = 16;

        // Used for synchronous access to the graphics device.
        private static readonly object _syncLock =
            new object();

        private TrueCraftGame _game;
        private GraphicsDevice _graphicsDevice;
        private int _submeshes = 0;
        private bool _isReady = false;
        protected VertexBuffer _vertices; // ChunkMesh uses these but external classes shouldn't, so I've made them protected.
        protected IndexBuffer[] _indices;

        private bool _recalculateBounds; // Whether this mesh should recalculate its bounding box when changed.

        /// <summary>
        /// Gets or sets the vertices in this mesh.
        /// </summary>
        public VertexPositionNormalColorTexture[] Vertices
        {
            set
            {
                if (_vertices != null)
                    _vertices.Dispose();

                _game.Invoke(() =>
                {
                    _vertices = new VertexBuffer(_graphicsDevice, VertexPositionNormalColorTexture.VertexDeclaration,
                        (value.Length + 1), BufferUsage.WriteOnly);
                    _vertices.SetData(value);
                    _isReady = true;
                });

                if (_recalculateBounds)
                    BoundingBox = RecalculateBounds(value);
            }
        }

        public bool IsReady
        {
            get
            {
                return _isReady;
            }
        }

        public int Submeshes
        {
            get
            {
                return _submeshes;
            }
        }

        /// <summary>
        /// Gets the bounding box for this mesh.
        /// </summary>
        public BoundingBox BoundingBox { get; private set; }

        /// <summary>
        /// Gets whether this mesh is disposed of.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates a new mesh.
        /// </summary>
        public Mesh(TrueCraftGame game, int submeshes = 1, bool recalculateBounds = true)
        {
            if ((submeshes < 0) || (submeshes >= Mesh.SubmeshLimit))
                throw new ArgumentOutOfRangeException();

            _game = game;
            _graphicsDevice = game.GraphicsDevice;
            _indices = new IndexBuffer[submeshes];
            _recalculateBounds = recalculateBounds;
        }

        /// <summary>
        /// Creates a new mesh.
        /// </summary>
        public Mesh(TrueCraftGame game, VertexPositionNormalColorTexture[] vertices,
                int submeshes = 1, bool recalculateBounds = true) : this(game, submeshes, recalculateBounds)
        {
            Vertices = vertices;
        }

        /// <summary>
        /// Creates a new mesh.
        /// </summary>
        public Mesh(TrueCraftGame game, VertexPositionNormalColorTexture[] vertices,
                int[] indices, bool recalculateBounds = true) : this(game, 1, recalculateBounds)
        {
            Vertices = vertices;
            SetSubmesh(0, indices);
        }

        /// <summary>
        /// Sets a submesh in this mesh.
        /// </summary>
        public void SetSubmesh(int index, int[] indices)
        {
            if ((index < 0) || (index > _indices.Length))
                throw new ArgumentOutOfRangeException();

            lock (_syncLock)
            {
                if (_indices[index] != null)
                    _indices[index].Dispose();

                _game.Invoke(() =>
                {
                    _indices[index] = new IndexBuffer(_graphicsDevice, typeof(int),
                        (indices.Length + 1), BufferUsage.WriteOnly);
                    _indices[index].SetData(indices);
                    if (index + 1 > _submeshes)
                        _submeshes = index + 1;
                });
            }
        }

        /// <summary>
        /// Draws this mesh using the specified effect.
        /// </summary>
        /// <param name="effect">The effect to use.</param>
        public void Draw(Effect effect)
        {
            if (effect == null)
                throw new ArgumentException();

            for (int i = 0; i < _indices.Length; i++)
                Draw(effect, i);
        }

        /// <summary>
        /// Draws a submesh in this mesh using the specified effect.
        /// </summary>
        /// <param name="effect">The effect to use.</param>
        /// <param name="index">The submesh index.</param>
        public void Draw(Effect effect, int index)
        {
            if (effect == null)
                throw new ArgumentException();

            if ((index < 0) || (index > _indices.Length))
                throw new ArgumentOutOfRangeException();

            if (_vertices == null || _vertices.IsDisposed || _indices[index] == null || _indices[index].IsDisposed || _indices[index].IndexCount < 3)
                return; // Invalid state for rendering, just return.

            effect.GraphicsDevice.SetVertexBuffer(_vertices);
            effect.GraphicsDevice.Indices = _indices[index];
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0, 0, _indices[index].IndexCount, 0, _indices[index].IndexCount / 3);
            }
            VerticiesRendered += _vertices.VertexCount;
            IndiciesRendered += _indices[index].IndexCount;
        }

        /// <summary>
        /// Returns the total vertices in this mesh.
        /// </summary>
        public int GetTotalVertices()
        {
            if (_vertices == null)
                return 0;

            lock (_syncLock)
                return _vertices.VertexCount;
        }

        /// <summary>
        /// Returns the total indices for all the submeshes in this mesh.
        /// </summary>
        public int GetTotalIndices()
        {
            lock (_syncLock)
            {
                int sum = 0;
                foreach (var element in _indices)
                    sum += (element != null) ? element.IndexCount : 0;
                return sum;
            }
        }

        /// <summary>
        /// Recalculates the bounding box for this mesh.
        /// </summary>
        /// <param name="vertices">The vertices in this mesh.</param>
        /// <returns></returns>
        protected virtual BoundingBox RecalculateBounds(VertexPositionNormalColorTexture[] vertices)
        {
            return new BoundingBox(
                vertices.Select(v => v.Position).OrderBy(v => v.Length()).First(),
                vertices.Select(v => v.Position).OrderByDescending(v => v.Length()).First());
        }

        /// <summary>
        /// Disposes of this mesh.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this mesh.
        /// </summary>
        /// <param name="disposing">Whether Dispose() called the method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _graphicsDevice = null; // Lose the reference to our graphics device.

                if (_vertices != null)
                    _vertices.Dispose();
                foreach (var element in _indices)
                {
                    if (element != null)
                        element.Dispose();
                }
            }
        }

        /// <summary>
        /// Finalizes this mesh.
        /// </summary>
        ~Mesh()
        {
            Dispose(false);
        }
    }
}
