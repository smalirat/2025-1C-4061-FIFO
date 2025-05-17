using BepuPhysics;
using TGC.MonoGame.TP.Fisica;

namespace TGC.MonoGame.TP.Objetos;

public class TransparentCheckpoint : IColisionable
{
    private readonly PhysicsManager physicsManager;

    private readonly StaticHandle boundingVolume;

    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public BodyType BodyType => BodyType.Checkpoint;

    public bool CanPlayerBallJumpOnIt => false;

    private const float Height = 1.25f;

    public TransparentCheckpoint(PhysicsManager physicsManager,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float length)
    {

        this.physicsManager = physicsManager;

        boundingVolume = this.physicsManager.AddStaticBox(width, Height, length, position, rotation, this);
    }

    public void NotifyCollition(IColisionable with)
    {
    }

    public XnaVector3 GetPlayerBallRespawnPosition()
    {
        return new XnaVector3(Position.X, Position.Y, Position.Z);
    }
}