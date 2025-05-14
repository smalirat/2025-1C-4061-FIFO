using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using System;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Fisica;

public class PhysicsManager
{
    private Simulation Simulation;
    public BufferPool BufferPool { get; private set; }
    public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

    public void Initialize()
    {
        BufferPool = new BufferPool();

        var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

        Simulation = Simulation.Create(
            BufferPool,
            new NarrowPhaseCallbacks(new SpringSettings(30, 1), maximumRecoveryVelocity: 2f, frictionCoefficient: 0f),
            new PoseIntegratorCallbacks(new BepuVector3(0, -60, 0)),
            new SolveDescription(8, 4));
    }

    public void Update(float deltaTime)
    {
        float safeDeltaTime = Math.Max(deltaTime, 1f / 240f);

        Simulation.Timestep(safeDeltaTime, ThreadDispatcher);
    }

    public BodyHandle AddDynamicSphere(float radius, float mass, XnaVector3 initialPosition)
    {
        var sphereShape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(sphereShape);

        var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.2f);

        var bodyDescription = BodyDescription.CreateDynamic(
            initialPosition.ToBepuVector3(),
            inertia: sphereShape.ComputeInertia(mass),
            collidableDescription,
            new BodyActivityDescription(sleepThreshold: 0.1f)
        );

        return Simulation.Bodies.Add(bodyDescription);
    }

    public StaticHandle AddStaticBox(float width, float height, float length, XnaVector3 initialPosition, XnaQuaternion initialRotation)
    {
        var boxShape = new Box(width, height, length);

        var staticDescription = new StaticDescription(
            initialPosition.ToBepuVector3(),
            initialRotation.ToBepuQuaternion(),
            Simulation.Shapes.Add(boxShape)
        );

        // El objeto es estatico
        return Simulation.Statics.Add(staticDescription);
    }

    public XnaVector3 GetPosition(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Pose.Position.ToXnaVector3();
    }

    public XnaQuaternion GetOrientation(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Pose.Orientation.ToXnaQuaternion();
    }

    public void Awake(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        bodyRef.Awake = true;
    }

    public XnaVector3 GetLinearVelocity(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Velocity.Linear.ToXnaVector3();
    }

    public XnaVector3 GetAngularVelocity(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Velocity.Angular.ToXnaVector3();
    }

    public void ApplyImpulse(BodyHandle bodyHandle, XnaVector3 impulseDirection, XnaVector3 impulseOffset, float impulseForce, float deltaTime)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);

        // A mayor masa menor impulso, mas cuesta mover la pelota
        var impulse = impulseDirection.ToBepuVector3() * impulseForce * deltaTime * bodyRef.LocalInertia.InverseMass;

        bodyRef.ApplyImpulse(impulse, bodyRef.Pose.Position + impulseOffset.ToBepuVector3());
    }
}