using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Globales;

namespace TGC.TP.FIFO.Modelos.Primitivas;

public class BoxPrimitive
{
    private VertexBuffer Vertices { get; set; }
    private IndexBuffer Indices { get; set; }

    public BoxPrimitive(XnaVector3 size)
    {
        CreateVertexBuffer(size);
        CreateIndexBuffer();
    }

    private void CreateVertexBuffer(XnaVector3 size)
    {
        var x = size.X / 2;
        var y = size.Y / 2;
        var z = size.Z / 2;

        var positions = new XnaVector3[]
        {
            // Back face
            new XnaVector3(x, -y, z),
            new XnaVector3(-x, -y, z),
            new XnaVector3(x, y, z),
            new XnaVector3(-x, y, z),

            // Front face
            new XnaVector3(x, y, -z),
            new XnaVector3(-x, y, -z),
            new XnaVector3(x, -y, -z),
            new XnaVector3(-x, -y, -z),

            // Top face
            new XnaVector3(x, y, z),
            new XnaVector3(-x, y, z),
            new XnaVector3(x, y, -z),
            new XnaVector3(-x, y, -z),

            // Bottom face
            new XnaVector3(x, -y, -z),
            new XnaVector3(x, -y, z),
            new XnaVector3(-x, -y, z),
            new XnaVector3(-x, -y, -z),

            // Left face
            new XnaVector3(-x, -y, z),
            new XnaVector3(-x, y, z),
            new XnaVector3(-x, y, -z),
            new XnaVector3(-x, -y, -z),

            // Right face
            new XnaVector3(x, -y, -z),
            new XnaVector3(x, y, -z),
            new XnaVector3(x, y, z),
            new XnaVector3(x, -y, z),
        };

        var textureCoordinates = new XnaVector2[]
        {
             // Back face
             XnaVector2.Zero,
             XnaVector2.UnitX,
             XnaVector2.UnitY,
             XnaVector2.One,

             // Front face
             XnaVector2.Zero,
             XnaVector2.UnitX,
             XnaVector2.UnitY,
             XnaVector2.One,

             // Top face
             XnaVector2.UnitX,
             XnaVector2.One,
             XnaVector2.Zero,
             XnaVector2.UnitY,

             // Bottom face
             XnaVector2.Zero,
             XnaVector2.UnitX,
             XnaVector2.One,
             XnaVector2.UnitY,

             // Left face
             XnaVector2.Zero,
             XnaVector2.UnitY,
             XnaVector2.One,
             XnaVector2.UnitX,

             // Right face
             XnaVector2.Zero,
             XnaVector2.UnitY,
             XnaVector2.One,
             XnaVector2.UnitX,
        };

        var normals = new XnaVector3[]
        {
            // Back face
            XnaVector3.Backward,
            XnaVector3.Backward,
            XnaVector3.Backward,
            XnaVector3.Backward,

            // Front face
            XnaVector3.Forward,
            XnaVector3.Forward,
            XnaVector3.Forward,
            XnaVector3.Forward,

            // Top face
            XnaVector3.Up,
            XnaVector3.Up,
            XnaVector3.Up,
            XnaVector3.Up,

            // Bottom face
            XnaVector3.Down,
            XnaVector3.Down,
            XnaVector3.Down,
            XnaVector3.Down,

            // Left face
            XnaVector3.Left,
            XnaVector3.Left,
            XnaVector3.Left,
            XnaVector3.Left,

            // Right face
            XnaVector3.Right,
            XnaVector3.Right,
            XnaVector3.Right,
            XnaVector3.Right,
        };

        var vertices = new VertexPositionNormalTexture[positions.Length];

        for (int index = 0; index < vertices.Length; index++)
        {
            vertices[index] = new VertexPositionNormalTexture(positions[index], normals[index], textureCoordinates[index]);
        }

        Vertices = new VertexBuffer(GameGlobals.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.None);
        Vertices.SetData(vertices);
    }

    private void CreateIndexBuffer()
    {
        var indices = new ushort[]
        {
            // Back face
            1, 2, 0,
            1, 3, 2,

            // Front face
            5, 6, 4,
            5, 7, 6,

            // Top face
            9, 10, 8,
            9, 11, 10,

            // Bottom face
            12, 15, 13,
            13, 15, 14,

            // Left face
            17, 16, 19,
            17, 19, 18,

            // Right face
            20, 23, 21,
            21, 23, 22,
        };

        Indices = new IndexBuffer(GameGlobals.GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
        Indices.SetData(indices);
    }

    public void Draw(Effect effect)
    {
        var graphicsDevice = effect.GraphicsDevice;

        graphicsDevice.SetVertexBuffer(Vertices);
        graphicsDevice.Indices = Indices;

        foreach (var effectPass in effect.CurrentTechnique.Passes)
        {
            effectPass.Apply();
            var primitiveCount = Indices.IndexCount / 3;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);
        }
    }
}