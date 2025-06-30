using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Objetos.PowerUps.Jump;
using TGC.TP.FIFO.Objetos.PowerUps.Speed;

namespace TGC.TP.FIFO.Fisica;

public struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
{
    private readonly CollidableProperty<MaterialProperties> CollidableMaterials;
    private readonly Dictionary<CollidableReference, ICollisionable> CollidableReferences;

    public NarrowPhaseCallbacks(CollidableProperty<MaterialProperties> collidableMaterials, Dictionary<CollidableReference, ICollisionable> collidableReferences)
    {
        CollidableMaterials = collidableMaterials;
        CollidableReferences = collidableReferences;
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
        int workerIndex,
        CollidablePair collidablePair,
        ref TManifold contacts,
        out PairMaterialProperties pairMaterial)
        where TManifold : unmanaged, IContactManifold<TManifold>
    {
        var collidableMaterialA = CollidableMaterials[collidablePair.A];
        var collidableMaterialB = CollidableMaterials[collidablePair.B];

        pairMaterial.FrictionCoefficient = collidableMaterialA.FrictionCoefficient * collidableMaterialB.FrictionCoefficient;
        pairMaterial.MaximumRecoveryVelocity = MathF.Max(collidableMaterialA.MaximumRecoveryVelocity, collidableMaterialB.MaximumRecoveryVelocity);
        pairMaterial.SpringSettings = pairMaterial.MaximumRecoveryVelocity == collidableMaterialA.MaximumRecoveryVelocity ? collidableMaterialA.SpringSettings : collidableMaterialB.SpringSettings;

        if (contacts.Count <= 0)
        {
            return true;
        }

        var collidableReferenceA = CollidableReferences[collidablePair.A];
        var collidableReferenceB = CollidableReferences[collidablePair.B];

        if (collidableReferenceA is null || collidableReferenceB is null)
        {
            return true;
        }

        if (collidableReferenceA is PlayerBall || collidableReferenceB is PlayerBall)
        {
            var convexContact = contacts as ConvexContactManifold?;

            var playerBall = collidableReferenceA is PlayerBall ? collidableReferenceA as PlayerBall : collidableReferenceB as PlayerBall;
            var contactWithPlayerBall = collidableReferenceA is PlayerBall ? collidableReferenceB : collidableReferenceA;

            contactWithPlayerBall.NotifyCollition(playerBall, convexContact?.Normal, playerBall.GetLinearSpeed());
            playerBall.NotifyCollition(contactWithPlayerBall, convexContact?.Normal, 0f);
        }

        var ignorePhyisicsReactionToCollition = collidableReferenceA is Checkpoint || collidableReferenceB is Checkpoint ||
                                                collidableReferenceA is SpeedPowerUp || collidableReferenceB is SpeedPowerUp ||
                                                collidableReferenceA is JumpPowerUp || collidableReferenceB is JumpPowerUp;

        return !ignorePhyisicsReactionToCollition;
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