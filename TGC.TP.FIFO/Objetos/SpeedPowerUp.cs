using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.Objetos;

public class SpeedPowerUp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly StaticHandle boundingVolume;

    private float radius;
    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private const float ModelHeight = 859.56f;
    private const float ModelWidth = 492.08f;
    private const float ModelLength = 115.72f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.SpeedPowerUp;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);


    public float SpeedMultiplier { get; private set; }
    public bool CanPlayerBallJumpOnIt => false;

    public SpeedPowerUp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
        float speedMultiplier,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.width = width;
        this.height = height;
        this.depth = depth;

        this.color = color;
        this.rotation = rotation;
        this.position = position;

        SpeedMultiplier = speedMultiplier;

        boundingVolume = this.physicsManager.AddStaticBox(radius, radius, radius, position, rotation, this);
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.8f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.LigthingModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: XnaMatrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: XnaMatrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void NotifyCollition(IColisionable with)
    {

    }
}
