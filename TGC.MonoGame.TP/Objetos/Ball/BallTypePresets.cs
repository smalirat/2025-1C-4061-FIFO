using System.Collections.Generic;

namespace TGC.MonoGame.TP.Objetos.Ball;

public static class BallPresets
{
    public static readonly Dictionary<BallType, BallProperties> Presets = new()
    {
        [BallType.Piedra] = new BallProperties
        {
            ImpulseForce = 0.5f,              // Lenta
            JumpForce = 0.2f,                 // Apenas salta
            Friction = 0.9f,                  // Mucha fricción
            DampingRatio = 1f,                // Se amortigua mucho (sin rebote)
            MaximumRecoveryVelocity = 0f,     // No rebota
            SpringFrequency = 35f,            // Contacto duro
            Mass = 10f,                       // Pesada
            Radius = 3f                       // Grande
        },

        [BallType.Metal] = new BallProperties
        {
            ImpulseForce = 1.2f,              // Aceleración fuerte
            JumpForce = 0.8f,                 // Puede saltar un poco
            Friction = 0.4f,                  // Se desliza
            DampingRatio = 0.9f,              // Rebote leve
            MaximumRecoveryVelocity = 1.2f,   // Rebote controlado
            SpringFrequency = 30f,           
            Mass = 7f,                        // No tan pesada
            Radius = 1f                       // Chica
        },

        [BallType.Goma] = new BallProperties
        {
            ImpulseForce = 30f,              // Más lenta
            JumpForce = 2000f,                // Muy saltarina
            Friction = 0.3f,                  // Se desliza fácilmente
            DampingRatio = 0.2f,              // Muy elástica
            MaximumRecoveryVelocity = 6f,     // Rebota mucho
            SpringFrequency = 20f,
            Mass = 2f,                        // Liviana
            Radius = 2f                       // Mediana
        }
    };
}