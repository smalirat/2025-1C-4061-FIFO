using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using System;
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

public class KinematicWall : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;
    private readonly AudioManager audioManager;

    private readonly BodyHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.FloorRamp;

    private float tiempo;

    public bool CanPlayerBallJumpOnIt { get; private set; }

    private const float Height = 1.25f;

    private float movementMultiplier;

    public KinematicWall(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        float width,
        float length,
        float mass,
        float friction,
        bool canPlayerBallJumpOnIt,
        float movementMultiplier)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;
        this.audioManager = audioManager;

        CanPlayerBallJumpOnIt = canPlayerBallJumpOnIt;
        this.movementMultiplier = movementMultiplier;

        var rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f);

        model = this.modelManager.CreateBox(graphicsDevice, Height, width, length);
        boundingVolume = this.physicsManager.AddKinematicBox(width, Height, length, mass, friction, position, rotation, this);

        world = XnaMatrix.CreateTranslation(position) * XnaMatrix.CreateFromQuaternion(rotation);
        this.movementMultiplier = movementMultiplier;
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = effectManager.BlinnPhongShader;
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

        effect.Parameters["baseTexture"]?.SetValue(textureManager.WoodBox1Texture);

        // Solo establecer la textura de normales si estamos usando NormalMapping
        // if (effect.CurrentTechnique.Name == "NormalMapping")
        // {
        //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture);
        // }

        model.Draw(effect);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        physicsManager.Awake(boundingVolume);

        tiempo += deltaTime;
        float x = MathF.Sin(tiempo) * movementMultiplier;

        var previousPosition = physicsManager.GetPosition(boundingVolume);

        physicsManager.SetPosition(boundingVolume, new BepuVector3(x, previousPosition.Y, previousPosition.Z));

        // Actualizo matriz mundo
        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
    }

    public void NotifyCollition(IColisionable with)
    { }

    public void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= GameState.MinBallSpeedForSounds)
        {
            audioManager.PlayWallHitSound(GameState.BallType);
        }
    }

    public void Reset()
    {
    }
}