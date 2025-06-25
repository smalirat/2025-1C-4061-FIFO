using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Luz;

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

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = effectManager.BlinnPhongShader;
        Texture2D texture = null;
        BlinnPhongMaterial material;

        switch (RampWallTextureType)
        {
            case RampWallTextureType.Dirt:
                texture = textureManager.DirtTexture;
                material = MaterialPresets.Tierra;
                break;
            case RampWallTextureType.Stones:
                texture = textureManager.StonesTexture;
                material = MaterialPresets.Piedra;
                break;
            default:
                texture = textureManager.DirtTexture;
                material = MaterialPresets.Tierra;
                break;
        }

        effect.Parameters["WorldViewProjection"]?.SetValue(world * view * projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["ambientColor"]?.SetValue(material.AmbientColor);
        effect.Parameters["diffuseColor"]?.SetValue(material.DiffuseColor);
        effect.Parameters["specularColor"]?.SetValue(material.SpecularColor);
        effect.Parameters["KAmbient"]?.SetValue(material.KAmbient);
        effect.Parameters["KDiffuse"]?.SetValue(material.KDiffuse);
        effect.Parameters["KSpecular"]?.SetValue(material.KSpecular);
        effect.Parameters["shininess"]?.SetValue(material.Shininess);

        effect.Parameters["lightPosition"]?.SetValue(lightPosition);
        effect.Parameters["eyePosition"]?.SetValue(eyePosition);

        effect.Parameters["baseTexture"]?.SetValue(texture);

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