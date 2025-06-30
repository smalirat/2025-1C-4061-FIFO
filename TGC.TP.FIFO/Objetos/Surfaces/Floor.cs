using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Floor : FloorWallRamp
{
    public Floor(PhysicsManager physicsManager, GraphicsDevice graphicsDevice, XnaVector3 position, float width, float length) : base(physicsManager, graphicsDevice, position, XnaQuaternion.Identity, width, length, FloorWallRampType.Floor, RampWallTextureType.Dirt)
    {
    }
}
