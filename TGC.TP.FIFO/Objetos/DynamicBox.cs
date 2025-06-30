using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos;

public class DynamicBox : IColisionable
{
    private readonly PhysicsManager physicsManager;

    private BodyHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.Box;

    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);
    private XnaVector3 InitialPosition;
    private XnaQuaternion InitialRotation;
    private float SideLength;
    private float Mass;
    private float Friction;

    public bool CanPlayerBallJumpOnIt => false;

    public DynamicBox(PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition,
        XnaQuaternion initialRotation,
        float sideLength,
        float friction,
        float mass)
    {
        this.physicsManager = physicsManager;

        this.InitialPosition = initialPosition;
        this.InitialRotation = initialRotation;
        this.SideLength = sideLength;
        this.Friction = friction;
        this.Mass = mass;

        model = ModelManager.CreateBox(graphicsDevice, sideLength, sideLength, sideLength);
        boundingVolume = this.physicsManager.AddDynamicBox(sideLength, sideLength, sideLength, mass, friction, initialPosition, initialRotation, this);

        world = XnaMatrix.CreateFromQuaternion(initialRotation) * XnaMatrix.CreateTranslation(initialPosition);
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
        //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture); //por ahora no tiene textura normal
        // }

        model.Draw(effect);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
    }

    public void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= GameState.MinBallSpeedForSounds)
        {
            AudioManager.PlayWoodBoxHitSound();
        }
    }

    public void Reset()
    {
        this.physicsManager.RemoveBoundingVolume(boundingVolume);
        this.boundingVolume = this.physicsManager.AddDynamicBox(SideLength, SideLength, SideLength, Mass, Friction, InitialPosition, InitialRotation, this);
    }

    public void NotifyCollition(IColisionable with) { }
}