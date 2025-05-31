using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;
using TGC.MonoGame.TP.Audio;



namespace TGC.MonoGame.TP.Objetos;

public class JumpPowerUp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;


    private readonly StaticHandle boundingVolume;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private const float ModelHeight = 3.72f;
    private const float ModelWidth = 0.78f;
    private const float ModelLength = 5.66f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.JumpPowerUp;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public float JumpMultiplier { get; private set; }
    public bool CanPlayerBallJumpOnIt => false;

    public JumpPowerUp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
        float jumpMultiplier,
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

        this.JumpMultiplier = jumpMultiplier;

        boundingVolume = this.physicsManager.AddStaticBox(width, height, depth, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.ArrowModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, deltaTime * 0.5f);
        rotation = Quaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollition(IColisionable with)
    {
        audioManager.PlayJumpPowerUpSound();
    }
}
