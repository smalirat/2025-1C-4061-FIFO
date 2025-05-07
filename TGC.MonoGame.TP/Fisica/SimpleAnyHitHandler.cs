using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using System.Numerics;

namespace TGC.MonoGame.TP.Fisica;

public struct SimpleAnyHitHandler : IRayHitHandler
{
    public bool Hit;
    public BodyHandle IgnorarCuerpo;

    public bool AllowTest(CollidableReference collidable, int childIndex)
    {
        return collidable.Mobility != CollidableMobility.Kinematic && (collidable.BodyHandle != IgnorarCuerpo);
    }

    public bool AllowTest(CollidableReference collidable)
    {
        return collidable.Mobility != CollidableMobility.Kinematic && (collidable.BodyHandle != IgnorarCuerpo);
    }

    public void OnRayHit(in RayData ray, ref float maximumT, float t, in Vector3 normal, CollidableReference collidable, int childIndex)
    {
        Hit = true;
    }
}