using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class LowSpeedPowerUp : SpeedPowerUp
{
    public LowSpeedPowerUp(XnaVector3 position) : base(position, speedMultiplier: 15f, color: Color.Yellow)
    {
    }
}