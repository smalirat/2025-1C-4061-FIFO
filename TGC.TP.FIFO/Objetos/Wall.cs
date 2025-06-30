using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos;

public class Wall : FloorWallRamp
{
    public Wall(PhysicsManager physicsManager, GraphicsDevice graphicsDevice, XnaVector3 position, XnaQuaternion rotation, float width, float length) : base(physicsManager, graphicsDevice, position, rotation, width, length, FloorWallRampType.Wall, RampWallTextureType.Stones)
    {
    }
}