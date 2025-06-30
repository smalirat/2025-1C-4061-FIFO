using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public class MediumSpeedPowerUp : SpeedPowerUp
{
    public MediumSpeedPowerUp(PhysicsManager physicsManager, XnaVector3 position) : base(physicsManager, position, 30f, Color.Orange)
    {
    }
}