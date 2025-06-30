using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class MediumJumpPowerUp : JumpPowerUp
{
    public MediumJumpPowerUp(XnaVector3 position) : base(position, jumpMultiplier: 0.2f, color: Color.Orange)
    {
    }
}