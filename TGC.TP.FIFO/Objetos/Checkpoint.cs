using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Utilidades;
using TGC.TP.FIFO.Audio;



namespace TGC.TP.FIFO.Objetos;

public class Checkpoint : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;

    private readonly StaticHandle boundingVolume;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    public int Id { get; private set; }

    private const float ModelHeight = 1f;
    private const float ModelWidth = 1f;
    private const float ModelLength = 1f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.Checkpoint;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public bool CanPlayerBallJumpOnIt => false;

    private static int _lastActivatedCheckpointId = 0;

    public Checkpoint(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        AudioManager audioManager,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
        Color color,
        int id)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.audioManager = audioManager;


        this.width = width;
        this.height = height;
        this.depth = depth;

        this.color = color;
        this.rotation = rotation;
        this.position = position;
        this.Id = id;

        boundingVolume = this.physicsManager.AddStaticBox(width * 2, height * 4, depth * 2, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.FlagModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: XnaMatrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: XnaMatrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.5f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollition(IColisionable with)
    {
        if (with.BodyType == BodyType.PlayerBall)
        {
            if (Id > _lastActivatedCheckpointId)
            {
                _lastActivatedCheckpointId = Id;
                audioManager.PlayCheckpointSound();
            }
        }
    }

    public XnaVector3 GetPlayerBallRespawnPosition()
    {
        return Position + new XnaVector3(0f, 10f, 0f);
    }

    public static int GetLastActivatedCheckpointId()
    {
        return _lastActivatedCheckpointId;
    }
}