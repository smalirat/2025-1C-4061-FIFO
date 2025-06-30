using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.HUDS;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Objetos.Boxes;
using TGC.TP.FIFO.Objetos.PowerUps.Jump;
using TGC.TP.FIFO.Objetos.PowerUps.Speed;
using TGC.TP.FIFO.Objetos.Surfaces;
using TGC.TP.FIFO.Skybox;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO;

public class TGCGame : Game
{
    private readonly GraphicsDeviceManager graphicsDeviceManager;
    private readonly List<IGameObject> gameObjects = [];

    private TargetCamera targetCamera;
    private HUD hud;
    private GameMenu gameMenu;
    private SkyBox skybox;
    private PlayerBall playerBall;
    private Random random;

    public TGCGame()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        random = new Random(6814);
    }

    protected override void Initialize()
    {
        graphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        graphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        graphicsDeviceManager.ApplyChanges();

        PhysicsManager.Initialize();
        GameGlobals.GraphicsDevice = GraphicsDevice;
        GameGlobals.SpriteBatch = new SpriteBatch(GraphicsDevice);

        hud = new HUD();
        skybox = new SkyBox();
        playerBall = new PlayerBall(initialPosition: new Vector3(0f, 50f, 0f));

        InitializeScenario();

        targetCamera = new TargetCamera(playerBall.Position);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        TextureManager.Load(Content);
        AudioManager.Load(Content);
        FontsManager.Load(Content);
        hud.LoadContent(Content);

        gameMenu = new GameMenu(Exit, NewGame, playerBall.Reset);
        AudioManager.PlayBackgroundMusic();

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            GameState.Pause();
            gameMenu.ChangeToOptionsMenu();
            AudioManager.StopRollingSound();
            return;
        }

        if (!GameState.Playing)
        {
            gameMenu.Update(keyboardState, deltaTime, targetCamera);
            return;
        }

        GameState.CheckIfPlayerLost();
        playerBall.Update(keyboardState, deltaTime, targetCamera);
        PhysicsManager.Update(deltaTime);
        targetCamera.Update(playerBall.Position);

        foreach (var gameObject in gameObjects)
        {
            gameObject.Update(keyboardState, deltaTime, targetCamera);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (!GameState.Playing)
        {
            gameMenu.Draw(gameTime, GraphicsDevice);
            return;
        }

        skybox.Draw(targetCamera.View, targetCamera.Projection, playerBall.Position, GraphicsDevice);
        playerBall.Draw(targetCamera.View, targetCamera.Projection, EffectManager.LightPosition, eyePosition: targetCamera.Position);

        foreach (var gameObject in gameObjects)
        {
            gameObject.Draw(targetCamera.View, targetCamera.Projection, EffectManager.LightPosition, eyePosition: targetCamera.Position);
        }

        hud.Draw(playerBall);

        base.Draw(gameTime);
    }

    private void InitializeScenario()
    {
        // Checkpoints
        var initialCheckpoint = new Checkpoint(position: new XnaVector3(0f, 0f, 10f), glow: true);
        var checkpoint2 = new Checkpoint(position: new XnaVector3(0f, -44.5f, 380f), glow: false);
        var checkpoint3 = new Checkpoint(position: new XnaVector3(0f, -44.5f, 580f), glow: false);
        var finalCheckpoint = new Checkpoint(position: new XnaVector3(0f, 175f, 800f), glow: true);

        gameObjects.Add(initialCheckpoint);
        gameObjects.Add(checkpoint2);
        gameObjects.Add(checkpoint3);
        gameObjects.Add(finalCheckpoint);

        hud.SetCheckpoints([initialCheckpoint, checkpoint2, checkpoint3, finalCheckpoint]);

        // Pisos
        gameObjects.Add(new Floor(position: new XnaVector3(0f, 0f, 74.6f), width: 150f, length: 300f));
        gameObjects.Add(new Floor(position: new XnaVector3(0f, -46.5f, 591.2f), width: 150f, length: 450f));

        // Rampas
        gameObjects.Add(new Ramp(position: new XnaVector3(0f, -23.2f, 296f), width: 150f, length: 150f));

        // Paredes
        gameObjects.Add(new Wall(position: new XnaVector3(0f, 75f, -75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(0f, -46.7f + 75f, 817f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), width: 150f, height: 150f));
        gameObjects.Add(new Wall(position: new XnaVector3(75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), width: 150f, height: 150f));

        // Power Ups de Velocidad
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(-44f, -44.5f, 442f)));
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(45f, -44.5f, 394f)));
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(0f, -44.5f, 515f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(0f, -44.5f, 442f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(-44f, -44.5f, 394f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(45f, -44.5f, 515f)));
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(45f, -44.5f, 442f)));
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(0f, -44.5f, 394f)));
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(-44f, -44.5f, 515f)));

        // Power Ups de Salto
        gameObjects.Add(new LowJumpPowerUp(position: new XnaVector3(-15f, 2f, 30f)));
        gameObjects.Add(new LowJumpPowerUp(position: new XnaVector3(-5f, 2f, 140f)));
        gameObjects.Add(new MediumJumpPowerUp(position: new XnaVector3(15f, 2f, 30f)));
        gameObjects.Add(new MediumJumpPowerUp(position: new XnaVector3(-47f, 2f, 92f)));
        gameObjects.Add(new HighJumpPowerUp(position: new XnaVector3(-15f, 2f, 70f)));
        gameObjects.Add(new HighJumpPowerUp(position: new XnaVector3(25f, 2f, 80f)));

        // Paredes Obstaculos
        gameObjects.Add(new KinematicWall(position: new XnaVector3(0f, 11f, 225f), width: 40f, height: 20f, movementSpeed: 50f));

        for (int i = 0; i < 5; i++)
        {
            gameObjects.Add(new KinematicWall(position: new XnaVector3(0f, -36.5f, 431f * i), width: 20f, height: 20f, movementSpeed: 150f - i * 4));
        }

        // Pisos Flotantes Movedizos
        gameObjects.Add(new KinematicFloor(new XnaVector3(10f, -46.7f + 2f, 712f), XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(new XnaVector3(20f, -46.7f + 22f, 712f), XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-10f, -46.7f + 42f, 712f), XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-20f, -46.7f + 62f, 712f), XnaVector3.Backward));
        gameObjects.Add(new KinematicFloor(new XnaVector3(10f, -46.7f + 82f, 712f), XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(new XnaVector3(20f, -46.7f + 102f, 712f), XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-10f, -46.7f + 122f, 712f), XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-20f, -46.7f + 142f, 712f), XnaVector3.Backward));
        gameObjects.Add(new KinematicFloor(new XnaVector3(10f, -46.7f + 162f, 712f), XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(new XnaVector3(20f, -46.7f + 182f, 712f), XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-10f, -46.7f + 222f, 712f), XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(new XnaVector3(-20f, -46.7f + 242f, 712f), XnaVector3.Backward));

        // Cajas Dinamicas Dispersas
        gameObjects.Add(new DynamicBox(new XnaVector3(-55f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 5, 1f, 0.1f));
        gameObjects.Add(new DynamicBox(new XnaVector3(-30f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        gameObjects.Add(new DynamicBox(new XnaVector3(-10f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 6, 1f, 0.1f));
        gameObjects.Add(new DynamicBox(new XnaVector3(22f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        gameObjects.Add(new DynamicBox(new XnaVector3(30f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 8, 1f, 0.1f));
        gameObjects.Add(new DynamicBox(new XnaVector3(52f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 3, 1f, 0.1f));

        // Cajas Dinamicas en Piramide
        float boxSize = 5f;
        float spacing = 5f;
        XnaVector3 baseCenter = new XnaVector3(0f, 5f, 180f);

        for (int i = -3; i <= 3; i++)
        {
            gameObjects.Add(new DynamicBox(baseCenter + new XnaVector3(i * spacing, 0f, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        for (int i = -2; i <= 2; i++)
        {
            gameObjects.Add(new DynamicBox(baseCenter + new XnaVector3(i * spacing, spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        for (int i = -1; i <= 1; i++)
        {
            gameObjects.Add(new DynamicBox(baseCenter + new XnaVector3(i * spacing, 2 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        gameObjects.Add(new DynamicBox(baseCenter + new XnaVector3(0f, 3 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));

        // Cajas Estáticas Dispersas
        List<(XnaVector3 pos, XnaQuaternion rot, float size)> cajas = new();

        float[] staticBoxSizes = { 3f, 6f, 9f };

        List<XnaVector2> staticBoxPositions = new();

        for (int i = 0; i < 30; i++)
        {
            XnaVector2 position;
            bool valid;

            int attempts = 0;
            do
            {
                float xx = (float)(random.NextDouble() * 60f);
                float zz = (float)(random.NextDouble() * 200f);
                position = new XnaVector2(xx, zz);

                valid = staticBoxPositions.All(p => XnaVector2.Distance(p, position) >= 18f);
                attempts++;
            } while (!valid);

            staticBoxPositions.Add(position);
        }

        for (int i = 0; i < staticBoxPositions.Count; i++)
        {
            float xxx = staticBoxPositions[i].X;
            float zzz = staticBoxPositions[i].Y;

            // Una caja sola tamaño random
            if (i % 3 == 0)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float ssize = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(xxx, ssize / 2f + 0.75f, zzz), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), ssize));
            }
            // Dos cajas del mismo tamaño apiladas
            else if (i % 3 == 1)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float ssize = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(xxx, ssize / 2f + 0.75f, zzz), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), ssize));
                cajas.Add((new XnaVector3(xxx, ssize + ssize / 2f + 0.75f, zzz), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), ssize));
            }
            else
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float ssize = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(xxx, ssize / 2f + 0.75f, zzz), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), ssize));
                cajas.Add((new XnaVector3(xxx + ssize + 2f, ssize / 2f + 0.75f, zzz), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), ssize));
            }
        }

        foreach (var (pos, rot, ssize) in cajas)
        {
            gameObjects.Add(new StaticBox(pos, rot, ssize));
        }

        // Cajas Estáticas en Piramide
        float size = 10f;
        float spacingX = 12f;
        float spacingY = size + 0.1f;
        float baseFloorY = -46.5f;
        float z = 800f;

        int cantidadMaxima = 11;
        int cantidadActual = cantidadMaxima;
        int filaY = 0;

        while (cantidadActual >= 1)
        {
            for (int subFila = 0; subFila < 2; subFila++) // 3 filas por nivel
            {
                // Centrar la fila en X
                float totalAncho = (cantidadActual - 1) * spacingX;
                float startX = -totalAncho / 2f;

                for (int i = 0; i < cantidadActual; i++)
                {
                    float x = startX + i * spacingX;
                    float y = baseFloorY + filaY * spacingY + size / 2f;

                    gameObjects.Add(new StaticBox(new XnaVector3(x, y, z), XnaQuaternion.Identity, size));
                }

                filaY++;
            }

            cantidadActual--;
        }
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    public void NewGame()
    {
        GameState.NewGame();

        foreach (var gameObject in gameObjects)
        {
            gameObject.Reset();
        }

        playerBall.Reset();
    }
}