using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class HighSpeedPowerUp : SpeedPowerUp
{
    public HighSpeedPowerUp(XnaVector3 position) : base(position, speedMultiplier: 60f, color: Color.Red)
    {
    }
}