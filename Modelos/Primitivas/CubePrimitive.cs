using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Modelos.Primitivas;

/// <summary>
///     Geometric primitive class for drawing cubes.
/// </summary>
public class CubePrimitive : GeometricPrimitive
{
    public CubePrimitive(GraphicsDevice graphicsDevice) : this(graphicsDevice, 1, Color.White, Color.White,
        Color.White, Color.White, Color.White, Color.White)
    {
    }

    public CubePrimitive(GraphicsDevice graphicsDevice, float size, Color color) : this(graphicsDevice, size, color,
        color, color, color, color, color)
    {
    }

    /// <summary>
    ///     Constructs a new cube primitive, with the specified size.
    /// </summary>
    public CubePrimitive(GraphicsDevice graphicsDevice, float size, Color color1, Color color2, Color color3,
        Color color4, Color color5, Color color6)
    {
        // A cube has six faces, each one pointing in a different direction.
        XnaVector3[] normals =
        {
            // front normal
            XnaVector3.UnitZ,
            // back normal
            -XnaVector3.UnitZ,
            // right normal
            XnaVector3.UnitX,
            // left normal
            -XnaVector3.UnitX,
            // top normal
            XnaVector3.UnitY,
            // bottom normal
            -XnaVector3.UnitY
        };

        Color[] colors =
        {
            color1, color2, color3, color4, color5, color6
        };

        var i = 0;
        // Create each face in turn.
        foreach (var normal in normals)
        {
            // Get two vectors perpendicular to the face normal and to each other.
            var side1 = new XnaVector3(normal.Y, normal.Z, normal.X);
            var side2 = XnaVector3.Cross(normal, side1);

            // Six indices (two triangles) per face.
            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 1);
            AddIndex(CurrentVertex + 2);

            AddIndex(CurrentVertex + 0);
            AddIndex(CurrentVertex + 2);
            AddIndex(CurrentVertex + 3);

            // Four vertices per face.
            AddVertex((normal - side1 - side2) * size / 2, colors[i], normal);
            AddVertex((normal - side1 + side2) * size / 2, colors[i], normal);
            AddVertex((normal + side1 + side2) * size / 2, colors[i], normal);
            AddVertex((normal + side1 - side2) * size / 2, colors[i], normal);

            i++;
        }

        InitializePrimitive(graphicsDevice);
    }
}
