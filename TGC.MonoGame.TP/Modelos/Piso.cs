using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using BepuQuaternion = System.Numerics.Quaternion;
using BepuVector3 = System.Numerics.Vector3;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Modelos;

public class Piso
{
    private readonly EffectManager effectManager;

    private readonly QuadPrimitive quadPrimitive;
    private readonly XnaMatrix World;

    public Piso(EffectManager effectManager, Simulation simulation, GraphicsDevice graphicsDevice, XnaVector3 position, float width, float length)
    {
        this.effectManager = effectManager;

        quadPrimitive = new QuadPrimitive(graphicsDevice);

        // Matriz de mundo
        World = XnaMatrix.CreateScale(width / 2f, 1f, length / 2f) * XnaMatrix.CreateTranslation(position);

        // Modelo fisico
        var boxShape = new Box(width, 0.1f, length);

        var staticDescription = new StaticDescription(
            new BepuVector3(position.X, position.Y - 0.05f, position.Z), // centro del piso físico
            BepuQuaternion.Identity,
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
        effect.Parameters["DiffuseColor"]?.SetValue(new XnaVector3(0, 0, 0)); // verde claro

        quadPrimitive.Draw(effect);
    }
}