using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using System.Numerics;

public struct ClosestRayHitHandler : IRayHitHandler
{
    public bool Hit;
    public float T;
    public Vector3 Normal;
    public Vector3 Position;
    public CollidableReference HitCollidable;

    public bool AllowTest(CollidableReference collidable) => true;
    public bool AllowTest(CollidableReference collidable, int childIndex) => true;

    public void OnRayHit(
        in RayData ray,
        ref float maximumT,
        float t,
        in Vector3 normal,
        CollidableReference collidable,
        int childIndex)
    {
        if (!Hit || t < T)
        {
            Hit = true;
            T = t;
            Normal = normal;
            Position = ray.Origin + ray.Direction * t;
            HitCollidable = collidable;
            maximumT = t;
        }
    }
}
