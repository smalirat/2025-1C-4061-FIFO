using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class LowSpeedPowerUp : SpeedPowerUp
{
    public LowSpeedPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 15f, Color.Yellow)
    {
    }
}