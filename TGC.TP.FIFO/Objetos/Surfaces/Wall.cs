using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Wall : FloorWallRamp
{
    public Wall(XnaVector3 position, XnaQuaternion rotation, float width, float height) : base(position, rotation, width, height, FloorWallRampType.Wall, RampWallTextureType.Stones)
    {
    }
}