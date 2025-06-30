using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.Fisica;

public static class PhysicsManager
{
    private static Simulation Simulation;
    public static BufferPool BufferPool { get; private set; }
    public static ThreadDispatcher ThreadDispatcher { get; private set; }

    private static CollidableProperty<MaterialProperties> MaterialProperties;
    public static Dictionary<CollidableReference, ICollisionable> CollidableReferences = new();

    public static void Initialize()
    {
        BufferPool = new BufferPool();

        var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new ThreadDispatcher(targetThreadCount);

        MaterialProperties = new CollidableProperty<MaterialProperties>();

        Simulation = Simulation.Create(
            BufferPool,
            new NarrowPhaseCallbacks(MaterialProperties, CollidableReferences),
            new PoseIntegratorCallbacks(
                gravity: new BepuVector3(0, -15, 0),
                linearDamping: 0.03f, // Que tan rapido se disipa la velocidad lineal
                angularDamping: 0.6f // Que tan rapido se disipa la velocidad angular
             ),
            new SolveDescription(8, 4));

        MaterialProperties.Initialize(Simulation);
    }

    public static void Update(float deltaTime)
    {
        float safeDeltaTime = Math.Max(deltaTime, 1f / 240f);
        Simulation.Timestep(safeDeltaTime, ThreadDispatcher);
    }

    public static BodyHandle AddDynamicSphere(float radius, float mass, float friction, float dampingRatio, float springFrequency, float maximumRecoveryVelocity, XnaVector3 initialPosition, ICollisionable collidableReference)
    {
        var sphereShape = new Sphere(radius);
        var shapeIndex = Simulation.Shapes.Add(sphereShape);

        var collidableDescription = new CollidableDescription(shapeIndex, ContinuousDetection.Passive);

        var bodyDescription = BodyDescription.CreateDynamic(
            initialPosition.ToBepuVector3(),
            sphereShape.ComputeInertia(mass),
            collidableDescription,
            new BodyActivityDescription(0.1f));

        var handle = Simulation.Bodies.Add(bodyDescription);

        CollidableReferences[new CollidableReference(CollidableMobility.Dynamic, handle)] = collidableReference;

        MaterialProperties.Allocate(handle) = new MaterialProperties
        {
            FrictionCoefficient = friction,
            MaximumRecoveryVelocity = maximumRecoveryVelocity,
            SpringSettings = new SpringSettings(springFrequency, dampingRatio)
        };

        return handle;
    }

    public static BodyHandle AddDynamicBox(float width, float height, float length, float mass, float friction, XnaVector3 initialPosition, XnaQuaternion initialRotation, ICollisionable collidableReference)
    {
        var boxShape = new Box(width, height, length);
        var shapeIndex = Simulation.Shapes.Add(boxShape);

        var collidableDescription = new CollidableDescription(shapeIndex, ContinuousDetection.Passive);

        var bodyDescription = BodyDescription.CreateDynamic(
            new RigidPose(initialPosition.ToBepuVector3(), initialRotation.ToBepuQuaternion()),
            boxShape.ComputeInertia(mass),
            collidableDescription,
            new BodyActivityDescription(0.1f));

        var handle = Simulation.Bodies.Add(bodyDescription);

        CollidableReferences[new CollidableReference(CollidableMobility.Dynamic, handle)] = collidableReference;

        MaterialProperties.Allocate(handle) = new MaterialProperties
        {
            FrictionCoefficient = friction,
            MaximumRecoveryVelocity = float.MaxValue, // Default
            SpringSettings = new SpringSettings(30, 1) // Default
        };

        return handle;
    }

    public static StaticHandle AddStaticBox(float width, float height, float length, XnaVector3 initialPosition, XnaQuaternion initialRotation, ICollisionable collidableReference)
    {
        var boxShape = new Box(width, height, length);
        var shapeIndex = Simulation.Shapes.Add(boxShape);

        var staticDescription = new StaticDescription(
            initialPosition.ToBepuVector3(),
            initialRotation.ToBepuQuaternion(),
            shapeIndex,
            continuity: ContinuousDetection.Passive);

        var handle = Simulation.Statics.Add(staticDescription);

        CollidableReferences[new CollidableReference(handle)] = collidableReference;

        MaterialProperties.Allocate(handle) = new MaterialProperties
        {
            FrictionCoefficient = 1f,
            MaximumRecoveryVelocity = float.MaxValue,
            SpringSettings = new SpringSettings(30, 1)
        };

        return handle;
    }

    public static BodyHandle AddKinematicBox(float width, float height, float length, float mass, float friction, XnaVector3 initialPosition, XnaQuaternion initialRotation, ICollisionable collidableReference)
    {
        var boxShape = new Box(width, height, length);
        var shapeIndex = Simulation.Shapes.Add(boxShape);

        var collidableDescription = new CollidableDescription(shapeIndex, ContinuousDetection.Passive);

        var bodyDescription = BodyDescription.CreateKinematic(
            new RigidPose(initialPosition.ToBepuVector3(), initialRotation.ToBepuQuaternion()),
            collidableDescription,
            new BodyActivityDescription(0.1f));

        var handle = Simulation.Bodies.Add(bodyDescription);

        CollidableReferences[new CollidableReference(CollidableMobility.Kinematic, handle)] = collidableReference;

        MaterialProperties.Allocate(handle) = new MaterialProperties
        {
            FrictionCoefficient = friction,
            MaximumRecoveryVelocity = 1f, // Default
            SpringSettings = new SpringSettings(30, 1) // Default
        };

        return handle;
    }

    public static float GetLinearSpeed(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Velocity.Linear.Length();
    }

    public static XnaVector3 GetPosition(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Pose.Position.ToXnaVector3();
    }

    public static XnaVector3 GetPosition(StaticHandle staticHandle)
    {
        var bodyRef = Simulation.Statics.GetStaticReference(staticHandle);
        return bodyRef.Pose.Position.ToXnaVector3();
    }

    public static XnaQuaternion GetOrientation(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Pose.Orientation.ToXnaQuaternion();
    }

    public static XnaQuaternion GetOrientation(StaticHandle staticHandle)
    {
        var bodyRef = Simulation.Statics.GetStaticReference(staticHandle);
        return bodyRef.Pose.Orientation.ToXnaQuaternion();
    }

    public static void Awake(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        bodyRef.Awake = true;
    }

    public static XnaVector3 GetLinearVelocity(BodyHandle bodyHandle)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
        return bodyRef.Velocity.Linear.ToXnaVector3();
    }

    public static void ApplyImpulse(BodyHandle bodyHandle, XnaVector3 impulseDirection, float impulseForce, float deltaTime)
    {
        var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);

        var direction = Vector3.Normalize(impulseDirection.ToBepuVector3());
        var impulse = direction * impulseForce * deltaTime;

        var position = bodyRef.Pose.Position;

        bodyRef.ApplyLinearImpulse(impulse.ToBepuVector3());
    }

    public static void SetPosition(BodyHandle bodyHandle, XnaVector3 newPosition)
    {
        var body = Simulation.Bodies.GetBodyReference(bodyHandle);
        body.Pose.Position = newPosition.ToBepuVector3();
        body.Velocity.Linear = BepuVector3.Zero;
    }

    public static void RemoveBoundingVolume(BodyHandle bodyHandle)
    {
        Simulation.Bodies.Remove(bodyHandle);
    }
}