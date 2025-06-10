using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos;

public class StaticBox : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;
    private readonly AudioManager audioManager;

    private readonly StaticHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.Box;
    public bool CanPlayerBallJumpOnIt => true;

    public StaticBox(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float sideLength)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;
        this.audioManager = audioManager;

        model = this.modelManager.CreateBox(graphicsDevice, sideLength, sideLength, sideLength);
        boundingVolume = this.physicsManager.AddStaticBox(sideLength, sideLength, sideLength, position, rotation, this);

        world = XnaMatrix.CreateFromQuaternion(rotation) * XnaMatrix.CreateTranslation(position);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.BasicTextureShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.Black.ToVector3());
        effect.Parameters["ModelTexture"].SetValue(textureManager.WoodBox4Texture);
        effect.Parameters["UVScale"].SetValue(1f);

        model.Draw(effect);
    }
    public void NotifyCollition(IColisionable with)
    {
        if (with.BodyType == BodyType.PlayerBall)
        {
            audioManager.PlayWallHitSound(GameState.BallType);
        }
    }

    public void Reset()
    {
    }
}
