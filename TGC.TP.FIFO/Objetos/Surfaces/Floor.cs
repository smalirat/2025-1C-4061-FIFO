namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Floor : FloorWallRamp
{
    public Floor(XnaVector3 position, float width, float length) : base(position, XnaQuaternion.Identity, width, length, FloorWallRampType.Floor, RampWallTextureType.Dirt)
    {
    }
}
