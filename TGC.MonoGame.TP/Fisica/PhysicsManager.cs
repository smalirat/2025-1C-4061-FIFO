using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using System;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Fisica
{
    public class PhysicsManager
    {
        private Simulation Simulation;
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

        private CollidableProperty<MaterialProperties> MaterialProperties;

        public void Initialize()
        {
            BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

            MaterialProperties = new CollidableProperty<MaterialProperties>();
            var callbacks = new NarrowPhaseCallbacks(MaterialProperties);

            Simulation = Simulation.Create(
                BufferPool,
                callbacks,
                new PoseIntegratorCallbacks(new BepuVector3(0, -60, 0)),
                new SolveDescription(8, 4));

            MaterialProperties.Initialize(Simulation);
        }

        public void Update(float deltaTime)
        {
            float safeDeltaTime = Math.Max(deltaTime, 1f / 240f);
            Simulation.Timestep(safeDeltaTime, ThreadDispatcher);
        }

        public BodyHandle AddDynamicSphere(float radius, float mass, float friction, float dampingRatio, float springFrequency, float maximumRecoveryVelocity, XnaVector3 initialPosition)
        {
            var sphereShape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(sphereShape);

            var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.2f);

            var bodyDescription = BodyDescription.CreateDynamic(
                initialPosition.ToBepuVector3(),
                sphereShape.ComputeInertia(mass),
                collidableDescription,
                new BodyActivityDescription(0.1f));

            var handle = Simulation.Bodies.Add(bodyDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties {
                FrictionCoefficient = friction,
                MaximumRecoveryVelocity = maximumRecoveryVelocity,
                SpringSettings = new SpringSettings(springFrequency, dampingRatio)
            };

            return handle;
        }

        public BodyHandle AddDynamicBox(float width, float height, float length, float mass, float friction, XnaVector3 initialPosition, XnaQuaternion initialRotation)
        {
            var boxShape = new Box(width, height, length);
            var shapeIndex = Simulation.Shapes.Add(boxShape);

            var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.2f);

            var bodyDescription = BodyDescription.CreateDynamic(
                new RigidPose(initialPosition.ToBepuVector3(), initialRotation.ToBepuQuaternion()),
                boxShape.ComputeInertia(mass),
                collidableDescription,
                new BodyActivityDescription(0.1f));

            var handle = Simulation.Bodies.Add(bodyDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties
            {
                FrictionCoefficient = friction,
                MaximumRecoveryVelocity = float.MaxValue, // Default
                SpringSettings = new SpringSettings(30, 1) // Default
            };

            return handle;
        }

        public BodyHandle AddDynamicCylinder(float length, float radius, float mass, float friction, XnaVector3 initialPosition, XnaQuaternion initialRotation)
        {
            var cylinderShape = new Cylinder(length, radius);
            var shapeIndex = Simulation.Shapes.Add(cylinderShape);

            var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.2f);

            var bodyDescription = BodyDescription.CreateDynamic(
                new RigidPose(initialPosition.ToBepuVector3(), initialRotation.ToBepuQuaternion()),
                cylinderShape.ComputeInertia(mass),
                collidableDescription,
                new BodyActivityDescription(0.1f));

            var handle = Simulation.Bodies.Add(bodyDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties
            {
                FrictionCoefficient = friction,
                MaximumRecoveryVelocity = float.MaxValue, // Default
                SpringSettings = new SpringSettings(30, 1) // Default
            };

            return handle;
        }

        public StaticHandle AddStaticSphere(float radius, XnaVector3 initialPosition)
        {
            var sphereShape = new Sphere(radius);
            var shapeIndex = Simulation.Shapes.Add(sphereShape);

            var staticDescription = new StaticDescription(
                initialPosition.ToBepuVector3(),
                BepuQuaternion.Identity,
                shapeIndex);

            var handle = Simulation.Statics.Add(staticDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties
            {
                FrictionCoefficient = 1f, // Default
                MaximumRecoveryVelocity = float.MaxValue, // Default
                SpringSettings = new SpringSettings(30, 1) // Default
            };

            return handle;
        }

        public StaticHandle AddStaticBox(float width, float height, float length, XnaVector3 initialPosition, XnaQuaternion initialRotation)
        {
            var boxShape = new Box(width, height, length);
            var shapeIndex = Simulation.Shapes.Add(boxShape);

            var staticDescription = new StaticDescription(
                initialPosition.ToBepuVector3(),
                initialRotation.ToBepuQuaternion(),
                shapeIndex);

            var handle = Simulation.Statics.Add(staticDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties
            {
                FrictionCoefficient = 1f,
                MaximumRecoveryVelocity = float.MaxValue,
                SpringSettings = new SpringSettings(30, 1)
            };

            return handle;
        }

        public StaticHandle AddStaticCylinder(float length, float radius, XnaVector3 initialPosition, XnaQuaternion initialRotation)
        {
            var cylinderShape = new Cylinder(radius, length);
            var shapeIndex = Simulation.Shapes.Add(cylinderShape);

            var staticDescription = new StaticDescription(
                initialPosition.ToBepuVector3(),
                initialRotation.ToBepuQuaternion(),
                shapeIndex);

            var handle = Simulation.Statics.Add(staticDescription);

            MaterialProperties.Allocate(handle) = new MaterialProperties
            {
                FrictionCoefficient = 1f, // Default
                MaximumRecoveryVelocity = float.MaxValue, // Default
                SpringSettings = new SpringSettings(30, 1) // Default
            };

            return handle;
        }

        public Vector3 GetPosition(BodyHandle bodyHandle)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            return bodyRef.Pose.Position.ToXnaVector3();
        }

        public Quaternion GetOrientation(BodyHandle bodyHandle)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            return bodyRef.Pose.Orientation.ToXnaQuaternion();
        }

        public void Awake(BodyHandle bodyHandle)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            bodyRef.Awake = true;
        }

        public Vector3 GetLinearVelocity(BodyHandle bodyHandle)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            return bodyRef.Velocity.Linear.ToXnaVector3();
        }

        public Vector3 GetAngularVelocity(BodyHandle bodyHandle)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            return bodyRef.Velocity.Angular.ToXnaVector3();
        }

        public void ApplyImpulse(BodyHandle bodyHandle, XnaVector3 impulseDirection, XnaVector3 impulseOffset, float impulseForce, float deltaTime)
        {
            var bodyRef = Simulation.Bodies.GetBodyReference(bodyHandle);
            var impulse = impulseDirection.ToBepuVector3() * impulseForce * deltaTime * bodyRef.LocalInertia.InverseMass;

            bodyRef.ApplyImpulse(impulse, bodyRef.Pose.Position + impulseOffset.ToBepuVector3());
        }
    }
}
