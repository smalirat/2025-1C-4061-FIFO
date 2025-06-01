using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Modelos.Primitivas;

public abstract class GeometricPrimitive : IDisposable
{
    // During the process of constructing a primitive model, vertex and index data is stored on the CPU in these managed lists.
    public List<VertexPositionColorNormal> Vertices { get; } = new List<VertexPositionColorNormal>();

    public List<ushort> Indices { get; } = new List<ushort>();

    // Once all the geometry has been specified, the InitializePrimitive method copies the vertex and index data into these buffers,
    // which store it on the GPU ready for efficient rendering.
    private VertexBuffer VertexBuffer { get; set; }

    private IndexBuffer IndexBuffer { get; set; }

    /// <summary>
    ///     Adds a new vertex to the primitive model. This should only be called during the initialization process, before
    ///     InitializePrimitive.
    /// </summary>
    protected void AddVertex(XnaVector3 position, Color color, XnaVector3 normal)
    {
        Vertices.Add(new VertexPositionColorNormal(position, color, normal));
    }

    /// <summary>
    ///     Adds a new index to the primitive model. This should only be called during the initialization process, before
    ///     InitializePrimitive.
    /// </summary>
    protected void AddIndex(int index)
    {
        if (index > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(index));

        Indices.Add((ushort)index);
    }

    /// <summary>
    ///     Queries the index of the current vertex. This starts at zero, and increments every time AddVertex is called.
    /// </summary>
    protected int CurrentVertex => Vertices.Count;

    /// <summary>
    ///     Once all the geometry has been specified by calling AddVertex and AddIndex, this method copies the vertex and index
    ///     data into GPU format buffers, ready for efficient rendering.
    /// </summary>
    protected void InitializePrimitive(GraphicsDevice graphicsDevice)
    {
        // Create a vertex declaration, describing the format of our vertex data.

        // Create a vertex buffer, and copy our vertex data into it.
        VertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorNormal), Vertices.Count, BufferUsage.None);
        VertexBuffer.SetData(Vertices.ToArray());

        // Create an index buffer, and copy our index data into it.
        IndexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), Indices.Count, BufferUsage.None);
        IndexBuffer.SetData(Indices.ToArray());
    }

    /// <summary>
    ///     Finalizer.
    /// </summary>
    ~GeometricPrimitive()
    {
        Dispose(false);
    }

    /// <summary>
    ///     Frees resources used by this object.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Frees resources used by this object.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        VertexBuffer?.Dispose();
        IndexBuffer?.Dispose();
    }

    /// <summary>
    ///     Draws the primitive model, using the specified effect. Unlike the other Draw overload where you just specify the
    ///     world/view/projection matrices and color, this method does not set any render states, so you must make sure all
    ///     states are set to sensible values before you call it.
    /// </summary>
    /// <param name="effect">Used to set and query effects, and to choose techniques.</param>
    public void Draw(Effect effect)
    {
        var graphicsDevice = effect.GraphicsDevice;

        // Set our vertex declaration, vertex buffer, and index buffer.
        graphicsDevice.SetVertexBuffer(VertexBuffer);

        graphicsDevice.Indices = IndexBuffer;

        foreach (var effectPass in effect.CurrentTechnique.Passes)
        {
            effectPass.Apply();

            var primitiveCount = Indices.Count / 3;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
        }
    }
}
