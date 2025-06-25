using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
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

        effect.Parameters["baseTexture"]?.SetValue(textureManager.WoodBox2Texture);

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