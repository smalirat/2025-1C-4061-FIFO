namespace TGC.TP.FIFO.Fisica;

public interface ICollisionable
{
    bool CanPlayerBallJumpOnIt();
    void NotifyCollition(ICollisionable with, XnaVector3? contactNormal, float contactSpeed);
}