using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos;

public class KinematicFloor : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;

    private readonly BodyHandle boundingVolume;
    private readonly BoxPrimitive model;

    private XnaMatrix world;

    public BodyType BodyType => BodyType.FloorRamp;

    private XnaVector3 direction;

    private float tiempo;

    public bool CanPlayerBallJumpOnIt { get; private set; }

    private const float Height = 1.25f;

    public KinematicFloor(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaVector3 direction,
        float width,
        float length,
        float mass,
        float friction,
        bool canPlayerBallJumpOnIt)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;

        this.direction = direction;
        CanPlayerBallJumpOnIt = canPlayerBallJumpOnIt;

        model = this.modelManager.CreateBox(graphicsDevice, Height, width, length);
        boundingVolume = this.physicsManager.AddKinematicBox(width, Height, length, mass, friction, position, XnaQuaternion.Identity, this);

        world = XnaMatrix.CreateTranslation(position);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.BasicTextureShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.Black.ToVector3());
        effect.Parameters["ModelTexture"].SetValue(textureManager.WoodBox2Texture);
        effect.Parameters["UVScale"].SetValue(1f);

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


    public void NotifyCollition(IColisionable with)
    {
    }
}