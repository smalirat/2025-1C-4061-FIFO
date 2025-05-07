using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;
using BepuVector3 = System.Numerics.Vector3;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaQuaternion = Microsoft.Xna.Framework.Quaternion;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Modelos;

public class Piso
{
    private readonly EffectManager effectManager;

    private readonly QuadPrimitive quadPrimitive;
    private readonly XnaMatrix World;

    private readonly Random random;
    private XnaVector3 Color;

    public Piso(EffectManager effectManager, Simulation simulation, GraphicsDevice graphicsDevice, XnaVector3 position, XnaQuaternion rotation, float width, float length)
    {
        random = new Random();
        Color = new XnaVector3(
            random.Next(256) / 255f,
            random.Next(256) / 255f,
            random.Next(256) / 255f
        );

        this.effectManager = effectManager;

        quadPrimitive = new QuadPrimitive(graphicsDevice);

        // Matriz de mundo
        World = XnaMatrix.CreateScale(width / 2f, 1f, length / 2f) *
                XnaMatrix.CreateFromQuaternion(rotation) *
                XnaMatrix.CreateTranslation(position.X, position.Y - 0.05f, position.Z);

        // Modelo fisico
        var boxShape = new Box(width, 0.1f, length);

        var staticDescription = new StaticDescription(
            new BepuVector3(position.X, position.Y - 0.05f, position.Z), // centro del piso físico
            rotation.ToBepuQuaternion(),
            simulation.Shapes.Add(boxShape)
        );

        // El objeto es estatico
        simulation.Statics.Add(staticDescription);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.BasicShader;

        effect.Parameters["World"].SetValue(World);
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        effect.Parameters["DiffuseColor"]?.SetValue(Color);

        quadPrimitive.Draw(effect);
    }
}