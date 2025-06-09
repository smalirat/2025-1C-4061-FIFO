using System.Collections.Generic;

namespace TGC.TP.FIFO.Objetos.Ball;

public static class BallPresets
{
    public static readonly Dictionary<BallType, BallProperties> Presets = new()
    {
        [BallType.Piedra] = new BallProperties
        {
            ImpulseForce = 60f,
            JumpForce = 30000f,
            Friction = 0.05f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 6f,
            Radius = 3f
        },

        [BallType.Metal] = new BallProperties
        {
            ImpulseForce = 30f,
            JumpForce = 2000f,
            Friction = 0.05f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 2f,
            Radius = 1f
        },

        [BallType.Goma] = new BallProperties
        {
            ImpulseForce = 30f,
            JumpForce = 2000f,
            Friction = 0.8f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 1.25f,
            Radius = 2f 
        }
    };
}