using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TGC.TP.FIFO.Modelos.Primitivas;

public class CylinderPrimitive : GeometricPrimitive
{
    // Constructor simplificado con valores por defecto
    public CylinderPrimitive(GraphicsDevice graphicsDevice, float height = 1f, float radius = 0.5f, int tessellation = 16)
        : this(graphicsDevice, height, radius, tessellation, Color.Red)
    {
    }

    public CylinderPrimitive(GraphicsDevice graphicsDevice, float height, float radius, int tessellation, Color color)
    {
        if (tessellation < 3)
            throw new ArgumentOutOfRangeException(nameof(tessellation), "Tessellation must be >= 3");

        float halfHeight = height / 2;

        // Lados del cilindro
        for (int i = 0; i <= tessellation; i++)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            XnaVector3 normal = new(dx, 0, dz);
            XnaVector3 side1 = normal * radius;

            AddVertex(new XnaVector3(dx * radius, halfHeight, dz * radius), color, normal);
            AddVertex(new XnaVector3(dx * radius, -halfHeight, dz * radius), color, normal);

            if (i > 0)
            {
                int baseIndex = CurrentVertex - 2;
                AddIndex(baseIndex);
                AddIndex(baseIndex + 1);
                AddIndex(baseIndex - 2);

                AddIndex(baseIndex + 1);
                AddIndex(baseIndex - 1);
                AddIndex(baseIndex - 2);
            }
        }

        // Tapa superior
        XnaVector3 topCenter = new(0, halfHeight, 0);
        for (int i = 0; i <= tessellation; i++)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            XnaVector3 normal = XnaVector3.Up;
            AddVertex(new XnaVector3(dx * radius, halfHeight, dz * radius), color, normal);

            if (i > 1)
            {
                AddIndex(CurrentVertex - 1);
                AddIndex(CurrentVertex - 2);
                AddIndex(CurrentVertex - 3);
            }
        }

        // Tapa inferior
        int start = CurrentVertex;
        for (int i = 0; i <= tessellation; i++)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);

            XnaVector3 normal = XnaVector3.Down;
            AddVertex(new XnaVector3(dx * radius, -halfHeight, dz * radius), color, normal);

            if (i > 1)
            {
                AddIndex(CurrentVertex - 3);
                AddIndex(CurrentVertex - 2);
                AddIndex(CurrentVertex - 1);
            }
        }

        InitializePrimitive(graphicsDevice);
    }


}
