using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class DynamicTree : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private const float ModelHeight = 14.33f;
    private const float ModelRadius = 3.95f;

    private readonly BodyHandle boundingVolume;

    private readonly float height;
    private readonly float radius;

    private Color color;
    private Quaternion rotation;
    private Vector3 position;

    private float XScale => radius / ModelRadius;
    private float YScale => height / ModelHeight;
    private float ZScale => radius / ModelRadius;

    public BodyType BodyType => BodyType.Tree;
    public bool CanPlayerBallJumpOnIt => false;

    public DynamicTree(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float radius,
        float height,
        float friction,
        float mass,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.radius = radius;
        this.height = height;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        boundingVolume = this.physicsManager.AddDynamicCylinder(height, radius, mass, friction, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.NormalTreeModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        // Actualizo matriz mundo
        position = physicsManager.GetPosition(boundingVolume);
        rotation = physicsManager.GetOrientation(boundingVolume);
    }

    public void NotifyCollition(IColisionable with)
    {

    }
}
