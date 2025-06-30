using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class LowJumpPowerUp : JumpPowerUp
{
    public LowJumpPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 0.1f, Color.Yellow)
    {
    }
}