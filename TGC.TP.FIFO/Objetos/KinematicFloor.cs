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

public class KinematicFloor : IColisionable
{
    private readonly PhysicsManager physicsManager;

    private readonly BodyHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.FloorRamp;

    private XnaVector3 direction;

    private float tiempo;

    public bool CanPlayerBallJumpOnIt => true;

    private const float Depth = 1.25f;
    private const float Width = 15f;
    private const float Mass = 1f;
    private const float Friction = 0.2f;

    public KinematicFloor(PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaVector3 direction)
    {
        this.physicsManager = physicsManager;
        this.direction = direction;

        model = ModelManager.CreateBox(graphicsDevice, Depth, Width, Width);
        boundingVolume = this.physicsManager.AddKinematicBox(Width, Depth, Width, Mass, Friction, position, XnaQuaternion.Identity, this);

        world = XnaMatrix.CreateTranslation(position);
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

        var previousPosition = physicsManager.GetPosition(boundingVolume);

        // Avanzar el tiempo
        tiempo += deltaTime * 1f;

        // Movimiento oscilante sobre la dirección indicada
        float offset = MathF.Sin(tiempo) * 0.2f;
        XnaVector3 desplazamiento = direction * offset;

        XnaVector3 nuevaPosicion = new XnaVector3(
            previousPosition.X + desplazamiento.X,
            previousPosition.Y + desplazamiento.Y,
            previousPosition.Z + desplazamiento.Z
        );

        physicsManager.SetPosition(boundingVolume, new BepuVector3(nuevaPosicion.X, nuevaPosicion.Y, nuevaPosicion.Z));

        // Actualizar la matriz mundo
        world = XnaMatrix.CreateTranslation(nuevaPosicion);
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