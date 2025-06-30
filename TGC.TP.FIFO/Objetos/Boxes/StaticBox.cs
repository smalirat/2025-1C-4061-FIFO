using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Boxes;

public class StaticBox : IColisionable
{
    private readonly PhysicsManager physicsManager;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.Box;
    public bool CanPlayerBallJumpOnIt => true;

    private float MinContactSpeedForSound;

    public StaticBox(PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float sideLength,
        float minContactSpeedForSound = GameState.MinBallSpeedForSounds)
    {
        this.physicsManager = physicsManager;

        MinContactSpeedForSound = minContactSpeedForSound;

        model = ModelManager.CreateBox(graphicsDevice, sideLength, sideLength, sideLength);
        this.physicsManager.AddStaticBox(sideLength, sideLength, sideLength, position, rotation, this);

        world = XnaMatrix.CreateFromQuaternion(rotation) * XnaMatrix.CreateTranslation(position);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = EffectManager.BlinnPhongShader;
        var material = MaterialPresets.Madera;

        effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"

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
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(1.0f, 1.0f));

        effect.Parameters["baseTexture"]?.SetValue(TextureManager.WoodBox1Texture);

        // Solo establecer la textura de normales si estamos usando NormalMapping
        // if (effect.CurrentTechnique.Name == "NormalMapping")
        // {
        //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture);
        // }

        model.Draw(effect);
    }

    public void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= MinContactSpeedForSound)
        {
            AudioManager.PlayWallHitSound(GameState.BallType);
        }
    }

    public void NotifyCollition(IColisionable with) { }
}
