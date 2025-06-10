using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.Menu;

public class GameMenu
{
    private FontsManager fontsManager;
    private SpriteBatch spriteBatch;

    private KeyboardState previousKeyboardState;

    private MenuState mainCurrentMenuState = MenuState.MainMenu;
    private MenuState subOptionMenuState = MenuState.NoSubOptions;

    private Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]> menuEntries;

    private int selectedOptionIndex = 0;

    public GameMenu(FontsManager fontsManager, SpriteBatch spriteBatch, Action exitGameAction, Action newGameAction, Action resetBallAction)
    {
        this.fontsManager = fontsManager;
        this.spriteBatch = spriteBatch;

        menuEntries = new Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]>
        {
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Nuevo juego", newGameAction, fontsManager.LucidaConsole14),
                    new TextMenuEntry("Opciones de bola", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.BallSubOptions), fontsManager.LucidaConsole14),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.SoundSubOptions), fontsManager.LucidaConsole14),
                    new TextMenuEntry("Salir", exitGameAction, fontsManager.LucidaConsole14)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.BallSubOptions),
                new MenuEntry[]
                {
                    new SelectMenuEntry<BallType>(
                        "Tipo de bola",
                        new List<BallType> { BallType.Goma, BallType.Metal, BallType.Piedra },
                        (BallType selected) => { 
                            GameState.BallType = selected;
                            resetBallAction.Invoke();
                        },
                        fontsManager.LucidaConsole14
                    ),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole14)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole14),
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Seguir jugando", GameState.Resume, fontsManager.LucidaConsole14),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.SoundSubOptions), fontsManager.LucidaConsole14),
                    new TextMenuEntry("Volver al menu principal", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole14),
                    new TextMenuEntry("Salir", exitGameAction, fontsManager.LucidaConsole14)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole14)
                }
            }
        };
    }

    public void ChangeToOptionsMenu()
    {
        this.mainCurrentMenuState = MenuState.OptionsMenu;
    }

    public void Update(KeyboardState currentState, float deltaTime, TargetCamera camera)
    {
        if (WasKeyPressed(Keys.S, currentState))
        {
            selectedOptionIndex = (selectedOptionIndex + 1) % menuEntries[GetCurrentMenuState()].Length;
        }
        else if (WasKeyPressed(Keys.W, currentState))
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + menuEntries[GetCurrentMenuState()].Length) % menuEntries[GetCurrentMenuState()].Length;
        }
        else if (WasKeyPressed(Keys.Enter, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.Enter);
        }
        else if (WasKeyPressed(Keys.A, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.A);
        }
        else if (WasKeyPressed(Keys.D, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.D);
        }

        previousKeyboardState = currentState;
    }

    public void Draw(GameTime gameTime, Viewport viewport)
    {
        var menuEntries = this.menuEntries[GetCurrentMenuState()];

        var startY = viewport.Height * 0.3f;
        var spacing = fontsManager.LucidaConsole14.LineSpacing * 1.5f;
        var centerX = viewport.Width / 2f;

        for (int i = 0; i < menuEntries.Length; i++)
        {
            var menuEntry = menuEntries[i];
            var textSize = menuEntry.Size;
            var position = new Vector2(centerX - textSize.X / 2f, startY + i * spacing);
            var textColor = (i == selectedOptionIndex) ? Color.Yellow : Color.White;

            menuEntry.Draw(spriteBatch, textColor, new XnaVector2(100, 150 + i * 40));
        }
    }

    private bool WasKeyPressed(Keys key, KeyboardState currentState)
    {
        return currentState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
    }

    private Tuple<MenuState, MenuState> GetCurrentMenuState()
    {
        return new Tuple<MenuState, MenuState>(mainCurrentMenuState, subOptionMenuState);
    }

    private Action SetCurrentMenuStateAction(MenuState mainCurrentMenuState, MenuState subOptionMenuState)
    {
        return () =>
        {
            this.mainCurrentMenuState = mainCurrentMenuState;
            this.subOptionMenuState = subOptionMenuState;
            this.selectedOptionIndex = 0;
        };
    }

    private BarMenuEntry GetMasterVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen general", 
            () => { 
                return GameState.MasterVolume; 
            }, 
            (int newVolume) => { 
                GameState.MasterVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            fontsManager.LucidaConsole14);
    }

    private BarMenuEntry GetBackgroundMusicVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen musica de fondo", 
            () => { 
                return GameState.BackgroundMusicVolume; 
            },
            (int newVolume) => { 
                GameState.BackgroundMusicVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
            },
            fontsManager.LucidaConsole14);
    }

    private BarMenuEntry GetSoundEffectsVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen efectos de sonido",
            () => { 
                return GameState.SoundEffectsVolume; 
            },
            (int newVolume) => {
                GameState.SoundEffectsVolume = newVolume;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            fontsManager.LucidaConsole14);
    }
}