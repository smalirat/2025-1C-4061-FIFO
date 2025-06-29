using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.Fisica;

public interface IColisionable
{
    BodyType BodyType { get; }

    bool CanPlayerBallJumpOnIt { get; }

    void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed);

    void NotifyCollition(IColisionable with);
}