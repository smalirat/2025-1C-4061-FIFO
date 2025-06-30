using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class HighSpeedPowerUp : SpeedPowerUp
{
    public HighSpeedPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 60f, Color.Red)
    {
    }
}