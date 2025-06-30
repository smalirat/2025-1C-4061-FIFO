using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class HighJumpPowerUp : JumpPowerUp
{
    public HighJumpPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 0.3f, Color.Red)
    {
    }
}