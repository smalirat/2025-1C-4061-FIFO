namespace TGC.TP.FIFO.Utilidades;

public static class Converters
{
    public static BepuVector3 ToBepuVector3(this XnaVector3 xnaVector3)
    {
        return new BepuVector3(xnaVector3.X, xnaVector3.Y, xnaVector3.Z);
    }

    public static XnaVector3 ToXnaVector3(this BepuVector3 vepuVector3)
    {
        return new XnaVector3(vepuVector3.X, vepuVector3.Y, vepuVector3.Z);
    }

    public static BepuQuaternion ToBepuQuaternion(this XnaQuaternion xnaQuaternion)
    {
        return new BepuQuaternion(xnaQuaternion.X, xnaQuaternion.Y, xnaQuaternion.Z, xnaQuaternion.W);
    }

    public static XnaQuaternion ToXnaQuaternion(this BepuQuaternion bepuQuaternion)
    {
        return new XnaQuaternion(bepuQuaternion.X, bepuQuaternion.Y, bepuQuaternion.Z, bepuQuaternion.W);
    }
}