using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class LowJumpPowerUp : JumpPowerUp
{
    public LowJumpPowerUp(XnaVector3 position) : base(position, jumpMultiplier: 0.1f, color: Color.Yellow)
    {
    }
}