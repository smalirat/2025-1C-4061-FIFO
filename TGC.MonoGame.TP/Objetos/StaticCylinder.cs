using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class StaticCylinder : IColisionable
{
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly float height;
    private readonly float radius;
    private readonly Color color;
    private readonly XnaQuaternion rotation;
    private readonly XnaVector3 position;

    private readonly CylinderPrimitive primitive;

    private readonly StaticHandle boundingVolume;

    public BodyType BodyType => BodyType.Other;
    public bool CanPlayerBallJumpOnIt => false;

    public StaticCylinder(
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float height,
        float radius,
        Color color)
    {
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
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
        primitive.Draw(effectManager.BasicShader);
    }

    public void NotifyCollition(IColisionable with)
    {

    }
}
