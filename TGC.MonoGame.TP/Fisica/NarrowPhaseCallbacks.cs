using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TGC.MonoGame.TP.Fisica;

public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    private readonly CollidableProperty<MaterialProperties> CollidableMaterials;
    private readonly Dictionary<CollidableReference, IColisionable> CollidableReferences;

    public NarrowPhaseCallbacks(CollidableProperty<MaterialProperties> collidableMaterials, Dictionary<CollidableReference, IColisionable> collidableReferences)
    {
        this.CollidableMaterials = collidableMaterials;
        this.CollidableReferences = collidableReferences;
    }

    public void Initialize(Simulation simulation)
    {
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
        int workerIndex, CollidablePair collidablePair, ref TManifold manifold,
        out PairMaterialProperties pairMaterial)
        where TManifold : unmanaged, IContactManifold<TManifold>
    {
        var collidableMaterialA = CollidableMaterials[collidablePair.A];
        var collidableMaterialB = CollidableMaterials[collidablePair.B];

        pairMaterial.FrictionCoefficient = collidableMaterialA.FrictionCoefficient * collidableMaterialB.FrictionCoefficient;
        pairMaterial.MaximumRecoveryVelocity = MathF.Max(collidableMaterialA.MaximumRecoveryVelocity, collidableMaterialB.MaximumRecoveryVelocity);
        pairMaterial.SpringSettings = pairMaterial.MaximumRecoveryVelocity == collidableMaterialA.MaximumRecoveryVelocity ? collidableMaterialA.SpringSettings : collidableMaterialB.SpringSettings;

        if (manifold.Count > 0)
        {
            if (CollidableReferences.TryGetValue(collidablePair.A, out var collidableReferenceA) && CollidableReferences.TryGetValue(collidablePair.B, out var collidableReferenceB))
            {
                collidableReferenceA.NotifyCollition(collidableReferenceB);
                collidableReferenceB.NotifyCollition(collidableReferenceA);

                if (collidableReferenceA.BodyType == BodyType.Checkpoint || collidableReferenceB.BodyType == BodyType.Checkpoint ||
                    collidableReferenceA.BodyType == BodyType.SpeedPowerUp || collidableReferenceB.BodyType == BodyType.SpeedPowerUp ||
                    collidableReferenceA.BodyType == BodyType.JumpPowerUp || collidableReferenceB.BodyType == BodyType.JumpPowerUp)
                {
                    return false;
                }
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public void Dispose()
    {
    }
}