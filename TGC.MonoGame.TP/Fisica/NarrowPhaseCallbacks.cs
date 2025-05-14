using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using System.Runtime.CompilerServices;

namespace TGC.MonoGame.TP.Fisica;

public class NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    private readonly CollidableProperty<MaterialProperties> materialProperties;
    private SpringSettings ContactSpringiness = new(30, 1);
    private float MaximumRecoveryVelocity = 2f;

    public NarrowPhaseCallbacks(CollidableProperty<MaterialProperties> materialProperties)
    {
        this.materialProperties = materialProperties;
    }

    public void Initialize(Simulation simulation)
    {
        // Nada que hacer por ahora
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold<TManifold>(
        int workerIndex, CollidablePair pair, ref TManifold manifold,
        out PairMaterialProperties pairMaterial)
        where TManifold : unmanaged, IContactManifold<TManifold>
    {
        materialProperties.TryGetProperty(pair.A, out var materialA);
        materialProperties.TryGetProperty(pair.B, out var materialB);

        // Promedio de fricción
        pairMaterial.FrictionCoefficient = 0.5f * (materialA.Friction + materialB.Friction);

        // Rebote simulado con MaximumRecoveryVelocity escalado por "restitución"
        var restitution = 0.5f * (materialA.Restitution + materialB.Restitution);
        pairMaterial.MaximumRecoveryVelocity = MaximumRecoveryVelocity * restitution;

        pairMaterial.SpringSettings = ContactSpringiness;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB,
        ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
        // Nada por ahora
    }
}