using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public class HighJumpPowerUp : JumpPowerUp
{
    public HighJumpPowerUp(XnaVector3 position) : base(position, jumpMultiplier: 0.3f, color: Color.Red)
    {
    }
}