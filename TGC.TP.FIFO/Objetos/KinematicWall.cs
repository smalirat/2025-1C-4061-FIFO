using BepuPhysics;
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
    private readonly PhysicsManager physicsManager;

    private readonly BodyHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.FloorRamp;

    private float tiempo;

    public bool CanPlayerBallJumpOnIt => false;

    private const float Depth = 1.25f;
    private const float Mass = 1f;
    private const float Friction = 1f;

    private float speed;

    public KinematicWall(PhysicsManager physicsManager,
        XnaVector3 position,
        float width,
        float height,
        float speed)
    {
        this.physicsManager = physicsManager;
        this.speed = speed;

        var rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f);

        model = ModelManager.CreateBox(Depth, width, height);
        boundingVolume = this.physicsManager.AddKinematicBox(width, Depth, height, Mass, Friction, position, rotation, this);

        world = XnaMatrix.CreateTranslation(position) * XnaMatrix.CreateFromQuaternion(rotation);
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

    public void Update(float deltaTime, TargetCamera camera)
    {
        physicsManager.Awake(boundingVolume);

        tiempo += deltaTime;
        float x = MathF.Sin(tiempo) * speed;

        var previousPosition = physicsManager.GetPosition(boundingVolume);

        physicsManager.SetPosition(boundingVolume, new BepuVector3(x, previousPosition.Y, previousPosition.Z));

        // Actualizo matriz mundo
        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
    }

    public void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= GameState.MinBallSpeedForSounds)
        {
            AudioManager.PlayWallHitSound(GameState.BallType);
        }
    }

    public void NotifyCollition(IColisionable with) { }
}