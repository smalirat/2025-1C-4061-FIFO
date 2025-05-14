using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class Pelota
{
    public XnaVector3 Position => world.Translation.ToBepuVector3();

    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly BodyHandle boundingVolume;
    private readonly SpherePrimitive model;

    private XnaMatrix world;

    private readonly float radius;
    private readonly float friction;

    // Impulso fijo al tocar una tecla
    private const float ImpulseForce = 1f;

    private Color color;

    public Pelota(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition,
        float radius,
        float mass,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.radius = radius;
        this.color = color;

        model = this.modelManager.CreateSphere(graphicsDevice, radius * 2f);
        boundingVolume = this.physicsManager.AddDynamicSphere(radius, mass, initialPosition);
        world = XnaMatrix.CreateTranslation(initialPosition);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var keyPressed = false;
        var impulseDirection = BepuVector3.Zero;

        // La pelota siempre esta activa en el mundo física
        this.physicsManager.Awake(boundingVolume);

        if (keyboardState.IsKeyDown(Keys.W))
        {
            keyPressed = true;
            impulseDirection -= camera.ForwardXZ.ToBepuVector3();
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            keyPressed = true;
            impulseDirection += camera.ForwardXZ.ToBepuVector3();
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            keyPressed = true;
            impulseDirection -= camera.RightXZ.ToBepuVector3();
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            keyPressed = true;
            impulseDirection += camera.RightXZ.ToBepuVector3();
        }

        if (keyPressed)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                impulseDirection, // Ya normalizado
                impulseOffset: Vector3.Zero, // Centro de masa
                ImpulseForce,
                deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.Space))
        {
            physicsManager.ApplyImpulse(boundingVolume,
                Vector3.Up,
                impulseOffset: Vector3.Zero,
                25f,
                deltaTime);
        }

        Debug.WriteLine($"Velocidad Angular PELOTA {physicsManager.GetAngularVelocity(boundingVolume)}");

        // Actualizo matriz mundo
        world = XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = effectManager.PelotaShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["DiffuseColor"]?.SetValue(this.color.ToVector3());

        model.Draw(effect);
    }
}