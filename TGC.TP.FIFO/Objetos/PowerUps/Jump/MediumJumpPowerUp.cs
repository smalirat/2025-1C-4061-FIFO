using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class MediumJumpPowerUp : JumpPowerUp
{
    public MediumJumpPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 0.2f, Color.Orange)
    {
    }
}