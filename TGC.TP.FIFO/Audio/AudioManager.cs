using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.Audio;

public static class AudioManager
{
    public const string ContentFolderSongs = "Songs/";
    public const string ContentFolderSoundEffects = "SoundEffects/";

    // Música de fondo
    public static Song BackgroundMusic { get; private set; }

    // Efectos de sonido para diferentes tipos de bola
    public static SoundEffect MetalJumpSound { get; private set; }

    public static SoundEffect RubberJumpSound { get; private set; }
    public static SoundEffect StoneJumpSound { get; private set; }
    public static SoundEffect BallRollingSound { get; private set; }
    public static SoundEffect SpeedPowerUpSound { get; private set; }
    public static SoundEffect JumpPowerUpSound { get; private set; }
    public static SoundEffect CheckpointSound { get; private set; }
    public static SoundEffect MetalHitSound { get; private set; }
    public static SoundEffect RubberHitSound { get; private set; }
    public static SoundEffect RockHitSound { get; private set; }
    public static SoundEffect WoodBoxHitSound { get; private set; }
    public static SoundEffectInstance BallRollInstance { get; set; }
    public static SoundEffectInstance SpeedPowerUpInstance { get; set; }
    public static SoundEffectInstance JumpPowerUpInstance { get; set; }
    public static SoundEffectInstance CheckpointInstance { get; set; }
    public static SoundEffectInstance MetalHitSoundInstance { get; set; }
    public static SoundEffectInstance RubberHitSoundInstance { get; set; }
    public static SoundEffectInstance RockHitSoundInstance { get; set; }
    public static SoundEffectInstance WoodBoxHitSoundInstance { get; set; }

    public static void Load(ContentManager content)
    {
        // Cargar música
        BackgroundMusic = content.Load<Song>(ContentFolderSongs + "Marble_It_Up_Gameplay_Sound");

        BallRollingSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Ball_Rolling");
        MetalJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Metal-Ball-Bounce");
        RubberJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Goma_Ball_Bounce");
        StoneJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Regular_Ball_Bounce");
        SpeedPowerUpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Speed-Power-Up");
        JumpPowerUpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Jump-Power-Up");
        CheckpointSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Checkpoint-Sound-Effect");
        MetalHitSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "choque_metal");
        RubberHitSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "choque_plastico");
        RockHitSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "choque_piedra");
        WoodBoxHitSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "wood-box-hit");

        BallRollInstance = BallRollingSound.CreateInstance();
        SpeedPowerUpInstance = SpeedPowerUpSound.CreateInstance();
        JumpPowerUpInstance = JumpPowerUpSound.CreateInstance();
        CheckpointInstance = CheckpointSound.CreateInstance();
        MetalHitSoundInstance = MetalHitSound.CreateInstance();
        RubberHitSoundInstance = RubberHitSound.CreateInstance();
        RockHitSoundInstance = RockHitSound.CreateInstance();
        WoodBoxHitSoundInstance = WoodBoxHitSound.CreateInstance();

        // Configurar las instancias para que se repitan
        BallRollInstance.IsLooped = true;
    }

    public static void PlayBackgroundMusic()
    {
        MediaPlayer.Play(BackgroundMusic);
        MediaPlayer.IsRepeating = true;
    }

    public static void StopBackgroundMusic()
    {
        MediaPlayer.Stop();
    }

    public static void PlayJumpSound(BallType ballType)
    {
        if (!GameState.Playing)
        {
            return;
        }

        switch (ballType)
        {
            case BallType.Metal:
                MetalJumpSound.Play();
                break;

            case BallType.Goma:
                RubberJumpSound.Play();
                break;

            case BallType.Piedra:
                StoneJumpSound.Play();
                break;
        }
    }

    public static void PlayWallHitSound(BallType ballType, float contactSpeed)
    {
        if (!GameState.Playing || contactSpeed < GameState.GetMinBallSpeedForSounds())
        {
            return;
        }

        switch (ballType)
        {
            case BallType.Metal:
                if (MetalHitSoundInstance.State != SoundState.Playing)
                {
                    MetalHitSoundInstance.Play();
                }
                break;

            case BallType.Goma:
                if (RubberHitSoundInstance.State != SoundState.Playing)
                {
                    RubberHitSoundInstance.Play();
                }
                break;

            case BallType.Piedra:
                if (RockHitSoundInstance.State != SoundState.Playing)
                {
                    RockHitSoundInstance.Play();
                }
                RockHitSound.Play();
                break;
        }
    }

    public static void PlayWoodBoxHitSound(float contactSpeed)
    {
        if (!GameState.Playing || contactSpeed < GameState.GetMinBallSpeedForSounds())
        {
            return;
        }

        if (WoodBoxHitSoundInstance.State != SoundState.Playing)
        {
            WoodBoxHitSoundInstance.Play();
        }
    }

    public static void PlaySpeedPowerUpSound()
    {
        if (!GameState.Playing)
        {
            return;
        }

        if (SpeedPowerUpInstance.State != SoundState.Playing)
        {
            SpeedPowerUpInstance.Play();
        }
    }

    public static void PlayJumpPowerUpSound()
    {
        if (!GameState.Playing)
        {
            return;
        }

        if (JumpPowerUpInstance.State != SoundState.Playing)
        {
            JumpPowerUpInstance.Play();
        }
    }

    public static void PlayCheckpointSound()
    {
        if (!GameState.Playing)
        {
            return;
        }

        if (CheckpointInstance.State != SoundState.Playing)
        {
            CheckpointInstance.Play();
        }
    }

    public static void PlayRollingSound()
    {
        if (!GameState.Playing)
        {
            return;
        }

        float volume = 1.0f;
        BallRollInstance.Volume = volume;
        BallRollInstance.Play();
    }

    public static void StopRollingSound()
    {
        BallRollInstance.Stop();
    }

    public static void UpdateRollingSound(BallType ballType, float speed)
    {
        // Ajustar el volumen basado en la velocidad
        float volume = MathHelper.Clamp(speed / 10f, 0f, 1f);
        if (BallRollInstance.State == SoundState.Playing)
            BallRollInstance.Volume = volume;
    }
}