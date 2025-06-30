using System.Diagnostics;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.Menu;

public static class GameState
{
    // Volumen
    public static int MasterVolume = 100;

    public static int BackgroundMusicVolume = 100;
    public static int SoundEffectsVolume = 100;

    // Checkpoints
    public static int TotalCheckpointsChecked = 0;
    public static int TotalCheckpoints = 4;

    public static bool Playing = false;
    public static BallType BallType = BallType.Goma;

    // Condiciones de victoria / derrota
    public static bool Won = false;
    public static bool Lost = false;
    public static Stopwatch Cronometer = new Stopwatch();
    public const int TotalSecondsBeforeLosing = 60 * 5;
    public const int TotalSecondsBeforeAboutToLose = 30;

    // Contactos
    public const float MinBallSpeedForSounds = 30f;

    public static void CheckpointChecked()
    {
        TotalCheckpointsChecked++;
        if (TotalCheckpointsChecked == TotalCheckpoints)
        {
            Won = true;
            Cronometer.Stop();
        }
    }

    public static void NewGame()
    {
        TotalCheckpointsChecked = 0;
        Cronometer.Restart();
        Playing = true;
        Won = false;
        Lost = false;
    }

    public static void Resume()
    {
        Cronometer.Start();
        Playing = true;
    }

    public static void Pause()
    {
        Cronometer.Stop();
        Playing = false;
    }

    public static void CheckIfPlayerLost()
    {
        if (Cronometer.Elapsed.TotalSeconds >= TotalSecondsBeforeLosing)
        {
            Lost = true;
            Cronometer.Stop();
        }
    }
}