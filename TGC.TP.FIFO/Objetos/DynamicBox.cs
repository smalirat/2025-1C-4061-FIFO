using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos;

public class DynamicBox : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;

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

    public DynamicBox(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition,
        XnaQuaternion initialRotation,
        float sideLength,
        float friction,
        float mass)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;

        this.InitialPosition = initialPosition;
        this.InitialRotation = initialRotation;
        this.SideLength = sideLength;
        this.Friction = friction;
        this.Mass = mass;

        model = this.modelManager.CreateBox(graphicsDevice, sideLength, sideLength, sideLength);
        boundingVolume = this.physicsManager.AddDynamicBox(sideLength, sideLength, sideLength, mass, friction, initialPosition, initialRotation, this);

        world = XnaMatrix.CreateFromQuaternion(initialRotation) * XnaMatrix.CreateTranslation(initialPosition);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.BasicTextureShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.Black.ToVector3());
        effect.Parameters["ModelTexture"].SetValue(textureManager.WoodBox3Texture);
        effect.Parameters["UVScale"].SetValue(1f);

        model.Draw(effect);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        // Actualizo matriz mundo
        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
    }

    public void NotifyCollition(IColisionable with)
    {
    }

    public void Reset()
    {
        this.physicsManager.RemoveBoundingVolume(boundingVolume);
        this.boundingVolume = this.physicsManager.AddDynamicBox(SideLength, SideLength, SideLength, Mass, Friction, InitialPosition, InitialRotation, this);
    }
}