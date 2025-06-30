using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos;

public class Floor : FloorWallRamp
{
    public Floor(PhysicsManager physicsManager, GraphicsDevice graphicsDevice, XnaVector3 position, XnaQuaternion rotation, float width, float length) : base(physicsManager, graphicsDevice, position, rotation, width, length, FloorWallRampType.Floor, RampWallTextureType.Dirt)
    {
    }
}
