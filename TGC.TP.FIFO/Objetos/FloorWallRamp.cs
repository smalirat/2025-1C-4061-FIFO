using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos;

public class FloorWallRamp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;
    private readonly AudioManager audioManager;

    private readonly StaticHandle boundingVolume;
    private readonly BoxPrimitive model;

    private RampWallTextureType RampWallTextureType;

    private XnaMatrix world;

    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public BodyType BodyType => BodyType.FloorRamp;

    public bool CanPlayerBallJumpOnIt { get; private set; }

    private const float Height = 1.25f;

    public FloorWallRamp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float length,
        bool canPlayerBallJumpOnIt,
        RampWallTextureType rampWallTextureType)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;
        this.audioManager = audioManager;

        RampWallTextureType = rampWallTextureType;
        CanPlayerBallJumpOnIt = canPlayerBallJumpOnIt;

        model = this.modelManager.CreateBox(graphicsDevice, Height, width, length);
        boundingVolume = this.physicsManager.AddStaticBox(width, Height, length, position, rotation, this);

        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) * XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
        this.audioManager = audioManager;
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.TextureTilingShader;
        Texture2D texture = null;

        switch (RampWallTextureType)
        {
            case RampWallTextureType.Dirt:
                texture = textureManager.DirtTexture;
                break;

            case RampWallTextureType.Stones:
                texture = textureManager.StonesTexture;
                break;
        }

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["ModelTexture"].SetValue(texture);
        effect.Parameters["Tiling"].SetValue(new XnaVector2(10f, 10f));

        model.Draw(effect);
    }

    public void NotifyCollition(IColisionable with)
    {
        if (with.BodyType == BodyType.PlayerBall && !CanPlayerBallJumpOnIt)
        {
            audioManager.PlayWallHitSound(GameState.BallType);
        }
    }

    public void Reset()
    {
    }
}