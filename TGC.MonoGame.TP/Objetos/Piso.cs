using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Modelos.Primitivas;

namespace TGC.MonoGame.TP.Objetos;

public class Piso
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly StaticHandle boundingVolume;
    private readonly QuadPrimitive model;

    private XnaMatrix world;

    private const float Height = 0.1f;
    private Color color;

    public Piso(ModelManager modelManager, 
        EffectManager effectManager, 
        PhysicsManager physicsManager, 
        GraphicsDevice graphicsDevice, 
        XnaVector3 position, 
        XnaQuaternion rotation, 
        float width, 
        float length,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.color = color;

        model = this.modelManager.CreateQuad(graphicsDevice);
        boundingVolume = this.physicsManager.AddStaticBox(width, Height, length, position, rotation);

        // Matriz de mundo
        world = XnaMatrix.CreateScale(width / 2f, 1f, length / 2f) *
                XnaMatrix.CreateFromQuaternion(rotation) *
                XnaMatrix.CreateTranslation(position.X, position.Y, position.Z);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.BasicShader;

        effect.Parameters["World"].SetValue(world);
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        effect.Parameters["DiffuseColor"]?.SetValue(this.color.ToVector3());

        model.Draw(effect);
    }
}