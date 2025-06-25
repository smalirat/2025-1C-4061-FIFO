using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Menu;

public class GameMenu
{
    private readonly FontsManager fontsManager;
    private readonly SpriteBatch spriteBatch;
    private readonly ModelManager modelManager;
    private readonly TextureManager textureManager;
    private readonly PhysicsManager physicsManager;
    private readonly EffectManager effectManager;
    private readonly AudioManager audioManager;
    private readonly GraphicsDevice graphicsDevice;
    private readonly TargetCamera Camera;

    private FloorWallRamp Piso;
    private FloorWallRamp ParedIzquierda;
    private FloorWallRamp ParedFondo;
    private Checkpoint DummyCheckpoint;
    private PlayerBall DummyPlayerBall;

    private float currentRotation = 0f;

    public List<StaticBox> StaticBoxes = new List<StaticBox>();

    private KeyboardState previousKeyboardState;

    private MenuState mainCurrentMenuState = MenuState.MainMenu;
    private MenuState subOptionMenuState = MenuState.NoSubOptions;

    private Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]> menuEntries;

    private int selectedOptionIndex = 0;

    private XnaVector3 BallPosition = new XnaVector3(3000f, 2995f, 3010f);

    private Texture2D BackgroundMenuTexture;

    public GameMenu(ModelManager modelManager, TextureManager textureManager, PhysicsManager physicsManager, EffectManager effectManager, AudioManager audioManager, GraphicsDevice graphicsDevice, FontsManager fontsManager, SpriteBatch spriteBatch, Action exitGameAction, Action newGameAction, Action resetBallAction)
    {
        this.fontsManager = fontsManager;
        this.spriteBatch = spriteBatch;
        this.modelManager = modelManager;
        this.textureManager = textureManager;
        this.physicsManager = physicsManager;
        this.audioManager = audioManager;
        this.graphicsDevice = graphicsDevice;
        this.effectManager = effectManager;

        BackgroundMenuTexture = new Texture2D(graphicsDevice, 1, 1);
        BackgroundMenuTexture.SetData([Color.White]);

        Camera = new TargetCamera(MathF.PI / 3f, this.graphicsDevice.Viewport.AspectRatio, 0.1f, 100000f, new XnaVector3(3000f, 3000f, 3000f), 30f, 0.01f);

        DummyPlayerBall = new PlayerBall(
            this.modelManager,
            this.effectManager,
            this.physicsManager,
            this.textureManager,
            this.audioManager,
            this.graphicsDevice,
            BallPosition);

        Piso = new FloorWallRamp(
            this.modelManager,
            this.effectManager,
            this.physicsManager,
            this.textureManager,
            this.audioManager,
            this.graphicsDevice,
            new XnaVector3(3050f, 2990f, 2950f),
            XnaQuaternion.Identity,
            150f,
            150f,
            false,
            RampWallTextureType.Dirt);

        ParedFondo = new FloorWallRamp(
            this.modelManager,
            this.effectManager,
            this.physicsManager,
            this.textureManager,
            this.audioManager,
            this.graphicsDevice,
            new XnaVector3(3050f, 3065f, 2925f),
            XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
            150f,
            150f,
            false,
            RampWallTextureType.Stones);

        ParedIzquierda = new FloorWallRamp(
            this.modelManager,
            this.effectManager,
            this.physicsManager,
            this.textureManager,
            this.audioManager,
            this.graphicsDevice,
            new XnaVector3(2975f, 3065f, 2950f),
            XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f),
            150f,
            150f,
            false,
            RampWallTextureType.Stones);

        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2996.5f, 3010f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2996.5f, 3013f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2996.5f, 3016f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2996.5f, 3019f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2998.5f, 3018f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2998.5f, 3015f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 2998.5f, 3012f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 3000.5f, 3017f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 3000.5f, 3014f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(2991f, 3002.5f, 3015.5f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(3020f, 2996f, 2995f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 60f), 10f));
        StaticBoxes.Add(new StaticBox(modelManager, effectManager, physicsManager, textureManager, audioManager, graphicsDevice, new XnaVector3(3000f, 2996f, 2975f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, -60f), 10f));

        DummyCheckpoint = new Checkpoint(this.modelManager, this.effectManager, this.physicsManager, this.graphicsDevice, this.audioManager, new XnaVector3(3010f, 2995f, 2990f), XnaQuaternion.Identity, 0.5f, 0.5f, 0.5f, Color.Blue);

        menuEntries = new Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]>
        {
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Nuevo juego", newGameAction, fontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de juego", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.BallSubOptions), fontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.SoundSubOptions), fontsManager.LucidaConsole20),
                    new TextMenuEntry("Salir", exitGameAction, fontsManager.LucidaConsole20)
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
                            DummyPlayerBall.Reset();
                        },
                        fontsManager.LucidaConsole20
                    ),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole20)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole20),
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Seguir jugando", GameState.Resume, fontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.SoundSubOptions), fontsManager.LucidaConsole20),
                    new TextMenuEntry("Volver al menu principal", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole20),
                    new TextMenuEntry("Salir", exitGameAction, fontsManager.LucidaConsole20)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.NoSubOptions), fontsManager.LucidaConsole20)
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
        currentRotation += GetRotationIncrement() * deltaTime;

        DummyCheckpoint.Update(deltaTime);
        DummyPlayerBall.UpdatePositionAndRotation(BallPosition, XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, currentRotation));

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

    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        var eyePosition = this.Camera.Position;

        Piso.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);
        ParedFondo.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);
        ParedIzquierda.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);
        DummyCheckpoint.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);
        DummyPlayerBall.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);

        foreach (StaticBox staticBox in StaticBoxes)
        {
            staticBox.Draw(this.Camera.View, this.Camera.Projection, effectManager.LightPosition, eyePosition);
        }

        var originalDepthStencilState = graphicsDevice.DepthStencilState;
        var originalBlendState = graphicsDevice.BlendState;

        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.BlendState = BlendState.AlphaBlend;

        var menuEntries = this.menuEntries[GetCurrentMenuState()];

        var startY = graphicsDevice.Viewport.Height * 0.3f;
        var spacing = fontsManager.LucidaConsole20.LineSpacing * 2.5f;
        var centerX = graphicsDevice.Viewport.Width / 2f;

        for (int i = 0; i < menuEntries.Length; i++)
        {
            var menuEntry = menuEntries[i];
            var textSize = menuEntry.Size;
            var position = new Vector2(centerX - textSize.X / 2f, startY + i * spacing);
            var textColor = (i == selectedOptionIndex) ? Color.Yellow : Color.White;

            // Fondo negro translúcido
            int padding = 10;
            var backgroundRect = new Rectangle(
                (int)(position.X - padding),
                (int)(position.Y - padding),
                (int)(textSize.X + padding * 2),
                (int)(textSize.Y + padding * 2)
            );
            spriteBatch.Begin();
            spriteBatch.Draw(this.BackgroundMenuTexture, backgroundRect, Color.Black * 0.6f);
            spriteBatch.End();

            // Dibujo del texto encima
            menuEntry.Draw(spriteBatch, textColor, position);
        }


        graphicsDevice.DepthStencilState = originalDepthStencilState;
        graphicsDevice.BlendState = originalBlendState;
    }

    private float GetRotationIncrement()
    {
        if (GameState.BallType == BallType.Metal)
        {
            return 4f;
        }

        if (GameState.BallType == BallType.Piedra)
        {
            return 0.5f;
        }

        return 1f;
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
            () =>
            {
                return GameState.MasterVolume;
            },
            (int newVolume) =>
            {
                GameState.MasterVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            fontsManager.LucidaConsole20);
    }

    private BarMenuEntry GetBackgroundMusicVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen musica de fondo",
            () =>
            {
                return GameState.BackgroundMusicVolume;
            },
            (int newVolume) =>
            {
                GameState.BackgroundMusicVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
            },
            fontsManager.LucidaConsole20);
    }

    private BarMenuEntry GetSoundEffectsVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen efectos de sonido",
            () =>
            {
                return GameState.SoundEffectsVolume;
            },
            (int newVolume) =>
            {
                GameState.SoundEffectsVolume = newVolume;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            fontsManager.LucidaConsole20);
    }
}