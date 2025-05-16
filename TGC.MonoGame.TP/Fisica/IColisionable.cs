namespace TGC.MonoGame.TP.Fisica;

public interface IColisionable
{
    BodyType BodyType { get; }

    void NotifyCollition(IColisionable with);
}