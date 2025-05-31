using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using TGC.MonoGame.TP.Objetos.Ball;


namespace TGC.MonoGame.TP.Audio;

public class AudioManager
{
    public const string ContentFolderSongs = "Songs/";
    public const string ContentFolderSoundEffects = "SoundEffects/";

    // Música de fondo
    public Song BackgroundMusic { get; private set; }

    // Efectos de sonido para diferentes tipos de bola
    public SoundEffect MetalJumpSound { get; private set; }
    public SoundEffect RubberJumpSound { get; private set; }
    public SoundEffect StoneJumpSound { get; private set; }
    public SoundEffect BallRollingSound { get; private set; }
    public SoundEffect CollisionSound { get; private set; }
    public Song SpeedPowerUpSound { get; private set; }
    public SoundEffect JumpPowerUpSound { get; private set; }
    public SoundEffect CheckpointSound { get; private set; }
    private SoundEffectInstance BallRollInstance { get; set; }


    public void Load(ContentManager content)
    {
        // Cargar música
        BackgroundMusic = content.Load<Song>(ContentFolderSongs + "Marble_It_Up_Gameplay_Sound");

        // FIX ME: Los guiones y un archivo de sonido deberia ser wav
        BallRollingSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Ball_Rolling");
        MetalJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Metal-Ball-Bounce");
        RubberJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Goma_Ball_Bounce");
        StoneJumpSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Regular_Ball_Bounce");
        //CollisionSound = content.Load<SoundEffect>(ContentFolderAudio + "");
        SpeedPowerUpSound = content.Load<Song>(ContentFolderSoundEffects + "Speed-Power-Up"); //esta como mp3 por eso
        //JumpPowerUpSound = content.Load<SoundEffect>(ContentFolderAudio + "Jump_Power_Up");
        CheckpointSound = content.Load<SoundEffect>(ContentFolderSoundEffects + "Checkpoint-Sound-Effect");

        BallRollInstance = BallRollingSound.CreateInstance();


        // Configurar las instancias para que se repitan
        BallRollInstance.IsLooped = true;

    }

    public void PlayBackgroundMusic()
    {
        MediaPlayer.Play(BackgroundMusic);
        MediaPlayer.IsRepeating = true;
    }

    public void StopBackgroundMusic()
    {
        MediaPlayer.Stop();
    }

    public void PlayJumpSound(Objetos.Ball.BallType ballType)
    {
        switch (ballType)
        {
            case BallType.Metal:
                MetalJumpSound.Play();
                break;
            case BallType.Rubber:
                RubberJumpSound.Play();
                break;
            case BallType.Stone:
                StoneJumpSound.Play();
                break;
        }
    }

    /*public void PlayCollisionSound()
    {
        CollisionSound.Play();
    }*/

    public void PlayJumpPowerUpSound()
    {
        JumpPowerUpSound.Play();
    }

    public void PlaySpeedPowerUpSound()
    {
        JumpPowerUpSound.Play();
    }

    public void PlayCheckpointSound()
    {
        CheckpointSound.Play();
    }

    public void PlayRollingSound()
    {
        float volume = 1.0f;
        BallRollInstance.Volume = volume;
        BallRollInstance.Play();
    }

    public void StopRollingSound()
    {
        BallRollInstance.Stop();
    }


    public void UpdateRollingSound(BallType ballType, float speed)
    {
        // Ajustar el volumen basado en la velocidad
        float volume = MathHelper.Clamp(speed / 10f, 0f, 1f);
        if (BallRollInstance.State == SoundState.Playing)
            BallRollInstance.Volume = volume;
    }

}