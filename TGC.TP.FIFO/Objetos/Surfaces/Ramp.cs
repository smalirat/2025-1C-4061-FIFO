using System;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Ramp : FloorWallRamp
{
    public Ramp(XnaVector3 position, float width, float length) : base(position, XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 10f), width, length, FloorWallRampType.Floor, RampWallTextureType.Dirt)
    {
    }
}
