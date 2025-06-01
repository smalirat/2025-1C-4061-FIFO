namespace TGC.TP.FIFO.Fisica;

public interface IColisionable
{
    BodyType BodyType { get; }

    bool CanPlayerBallJumpOnIt { get; }

    void NotifyCollition(IColisionable with);
}