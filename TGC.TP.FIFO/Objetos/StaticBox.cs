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

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = effectManager.BlinnPhongShader;

        effect.Parameters["WorldViewProjection"]?.SetValue(world * view * projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["ambientColor"]?.SetValue(new Vector3(0.25f, 0.18f, 0.1f));
        effect.Parameters["diffuseColor"]?.SetValue(new Vector3(0.6f, 0.4f, 0.2f));
        effect.Parameters["specularColor"]?.SetValue(new Vector3(0.2f, 0.2f, 0.1f));
        effect.Parameters["KAmbient"]?.SetValue(0.4f);
        effect.Parameters["KDiffuse"]?.SetValue(0.7f);
        effect.Parameters["KSpecular"]?.SetValue(0.2f);
        effect.Parameters["shininess"]?.SetValue(12.0f);

        effect.Parameters["lightPosition"]?.SetValue(lightPosition);
        effect.Parameters["eyePosition"]?.SetValue(eyePosition);

        // Usá la textura de madera correspondiente
        effect.Parameters["baseTexture"]?.SetValue(textureManager.WoodBox1Texture);

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
