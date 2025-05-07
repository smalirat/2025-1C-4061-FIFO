using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;
using BepuVector3 = System.Numerics.Vector3;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP.Modelos;

public class Pelota
{
    private readonly EffectManager effectManager;
    private readonly Simulation simulation;

    public BodyHandle BodyHandle;
    private readonly SpherePrimitive SpherePrimitive;

    public XnaMatrix World { get; private set; }

    public XnaVector3 Position => World.Translation.ToBepuVector3();

    private readonly float mass;
    private readonly float diameter;
    private readonly float radius;

    // Impulso fijo al tocar una tecla
    private const float ImpulsoFijo = 300f;

    public Pelota(EffectManager effectManager, Simulation simulation, GraphicsDevice graphicsDevice, XnaVector3 initialPosition, float diameter = 6f, float mass = 1f)
    {
        this.effectManager = effectManager;
        this.simulation = simulation;
        this.mass = mass;
        this.diameter = diameter;
        this.radius = this.diameter / 2f;

        SpherePrimitive = new SpherePrimitive(graphicsDevice, diameter);
        World = XnaMatrix.CreateTranslation(initialPosition);

        var sphereShape = new Sphere(radius); // Bounding volume
        var shapeIndex = simulation.Shapes.Add(sphereShape);

        var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.1f);

        var bodyDescription = BodyDescription.CreateDynamic(
            initialPosition.ToBepuVector3(),
            inertia: sphereShape.ComputeInertia(mass),
            collidableDescription,
            new BodyActivityDescription(sleepThreshold: 0.1f)
        );

        BodyHandle = simulation.Bodies.Add(bodyDescription);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var bodyRef = simulation.Bodies.GetBodyReference(BodyHandle);
        bodyRef.Awake = true; // La pelota siempre esta activa en el mundo física

        // Movimiento de la pelota
        var direccion = BepuVector3.Zero;
        var offset = BepuVector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            direccion += camera.ForwardXZ.ToBepuVector3();
            offset = new BepuVector3(0, 0, -radius); // Aplico impulso en el borde frontal
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            direccion -= camera.ForwardXZ.ToBepuVector3();
            offset = new BepuVector3(0, 0, radius); // Aplico impulso en el borde trasero
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            direccion += camera.RightXZ.ToBepuVector3();
            offset = new BepuVector3(-radius, 0, 0); // Aplico impulso en el borde izquierdo
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            direccion -= camera.RightXZ.ToBepuVector3();
            offset = new BepuVector3(radius, 0, 0); // Aplico impulso en el borde derecho
        }

        // Si se oprimio una tecla
        if (direccion != BepuVector3.Zero)
        {
            // A mayor masa menor impulso, mas cuesta mover la pelota
            var impulso = direccion * ImpulsoFijo * deltaTime / mass;

            // Aplico el impulso en algun borde de la pelota
            var puntoDeAplicacion = bodyRef.Pose.Position + offset;

            bodyRef.ApplyImpulse(impulso * deltaTime, puntoDeAplicacion);
        }

        // Actualizo matriz mundo
        World = XnaMatrix.CreateFromQuaternion(bodyRef.Pose.Orientation.ToXnaQuaternion()) *
                XnaMatrix.CreateTranslation(bodyRef.Pose.Position.ToXnaVector3());
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = this.effectManager.PelotaShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(World);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.MediumVioletRed.ToVector3());

        SpherePrimitive.Draw(effect);
    }
}