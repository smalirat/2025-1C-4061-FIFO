using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class MediumSpeedPowerUp : SpeedPowerUp
{
    public MediumSpeedPowerUp(XnaVector3 position) : base(position, speedMultiplier: 30f, color: Color.Orange)
    {
    }
}