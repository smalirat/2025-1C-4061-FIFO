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

    public GameMenu(FontsManager fontsManager, SpriteBatch spriteBatch, Action exitGameAction)
    {
        this.fontsManager = fontsManager;
        this.spriteBatch = spriteBatch;

        menuEntries = new Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]>
        {
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Nuevo juego", () => { GameOptions.State = GameState.Playing; }),
                    new TextMenuEntry("Opciones de bola", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.BallSubOptions)),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.SoundSubOptions)),
                    new TextMenuEntry("Salir", exitGameAction),
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.BallSubOptions),
                new MenuEntry[]
                {
                    new SelectMenuEntry<BallType>("Tipo de bola", new List<BallType> { BallType.Goma, BallType.Metal, BallType.Piedra }, (BallType selected) => { GameOptions.BallType = selected; }),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions))
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions))
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Seguir jugando", () => { GameOptions.State = GameState.Playing; }),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.SoundSubOptions)),
                    new TextMenuEntry("Volver al menu principal", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions)),
                    new TextMenuEntry("Salir", exitGameAction)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver atras", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.NoSubOptions))
                }
            }
        };
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

    public void Draw(GameTime gameTime)
    {
        var menuEntries = this.menuEntries[GetCurrentMenuState()];

        for (int i = 0; i < menuEntries.Length; i++)
        {
            var menuEntry = menuEntries[i];
            var textColor = (i == selectedOptionIndex) ? Color.Yellow : Color.White;
            menuEntry.Draw(spriteBatch, fontsManager.LucidaConsole14, textColor, new XnaVector2(100, 150 + i * 40));
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
                return GameOptions.MasterVolume; 
            }, 
            (int newVolume) => { 
                GameOptions.MasterVolume = newVolume;
                MediaPlayer.Volume = GameOptions.MasterVolume / 100f * GameOptions.BackgroundMusicVolume / 100f;
                SoundEffect.MasterVolume = GameOptions.MasterVolume / 100f * GameOptions.SoundEffectsVolume / 100f;
            });
    }

    private BarMenuEntry GetBackgroundMusicVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen musica de fondo", 
            () => { 
                return GameOptions.BackgroundMusicVolume; 
            },
            (int newVolume) => { 
                GameOptions.BackgroundMusicVolume = newVolume;
                MediaPlayer.Volume = GameOptions.MasterVolume / 100f * GameOptions.BackgroundMusicVolume / 100f;
            });
    }

    private BarMenuEntry GetSoundEffectsVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen efectos de sonido",
            () => { 
                return GameOptions.SoundEffectsVolume; 
            },
            (int newVolume) => {
                GameOptions.SoundEffectsVolume = newVolume;
                SoundEffect.MasterVolume = GameOptions.MasterVolume / 100f * GameOptions.SoundEffectsVolume / 100f;
            });
    }
}