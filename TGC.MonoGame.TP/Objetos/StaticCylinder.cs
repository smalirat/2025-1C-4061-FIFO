using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class StaticCylinder : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly GraphicsDevice graphicsDevice;
    private const float ModelHeight = 2f;
    private const float ModelRadius = 2f;
    private readonly float height;
    private readonly float radius;
    private readonly Color color;
    private readonly XnaQuaternion rotation;
    private readonly XnaVector3 position;
    private float XScale => radius / ModelRadius;
    private float YScale => height / ModelHeight;
    private float ZScale => radius / ModelRadius;

    private readonly CylinderPrimitive primitive;
    private readonly StaticHandle boundingVolume;

    public BodyType BodyType => BodyType.Other;
    public bool CanPlayerBallJumpOnIt => false;

    public StaticCylinder(
        ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float height,
        float radius,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.graphicsDevice = graphicsDevice;
        this.position = position;
        this.rotation = rotation;
        this.height = height;
        this.radius = radius;
        this.color = color;

        primitive = new CylinderPrimitive(graphicsDevice, height, radius, tessellation: 32, color);

        boundingVolume = physicsManager.AddStaticCylinder(radius, height, position, rotation, this);
    }

    public void Draw(Matrix view, Matrix projection)
    {

        var world = Matrix.CreateScale(XScale, YScale, ZScale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);

        var effect = effectManager.BasicShader;

        effect.Parameters["World"].SetValue(world);
        effect.Parameters["View"].SetValue(view);
        effect.Parameters["Projection"].SetValue(projection);
        effect.Parameters["DiffuseColor"].SetValue(color.ToVector3());

        primitive.Draw(effectManager.BasicShader);
    }

    public void NotifyCollition(IColisionable with)
    {

    }
}
