namespace TGC.MonoGame.TP.Fisica;

public interface IColisionable
{
    BodyType BodyType { get; }

    bool CanPlayerBallJumpOnIt { get; }

    void NotifyCollition(IColisionable with);
}