using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Texturas;

namespace TGC.MonoGame.TP.Objetos;

public class FloorWallRamp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;

    private readonly StaticHandle boundingVolume;
    private readonly BoxPrimitive model;

    private RampWallTextureType RampWallTextureType;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.FloorRamp;

    public bool CanPlayerBallJumpOnIt { get; private set; }

    private const float Height = 1.25f;

    public FloorWallRamp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
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

        this.RampWallTextureType = rampWallTextureType;
        this.CanPlayerBallJumpOnIt = canPlayerBallJumpOnIt;

        model = this.modelManager.CreateBox(graphicsDevice, Height, width, length);
        boundingVolume = this.physicsManager.AddStaticBox(width, Height, length, position, rotation, this);

        world = XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume)) * XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume));
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
            case RampWallTextureType.Stones1:
                texture = textureManager.Stones1Texture;
                break;
            case RampWallTextureType.Stones2:
                texture = textureManager.Stones1Texture;
                break;
            case RampWallTextureType.Stones3:
                texture = textureManager.Stones1Texture;
                break;
            case RampWallTextureType.Grass:
                texture = textureManager.GrassTexture;
                break;
            case RampWallTextureType.Ground:
                texture = textureManager.GroundTexture;
                break;
        }

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["ModelTexture"].SetValue(texture);
        effect.Parameters["Tiling"].SetValue(new Vector2(10f, 10f));

        model.Draw(effect);
    }

    public void NotifyCollition(IColisionable with)
    {
    }
}