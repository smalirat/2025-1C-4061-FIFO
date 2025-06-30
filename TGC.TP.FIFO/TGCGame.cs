using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

    public TGCGame()
    {
        graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    private void InitializeScenario()
    {
        // Globales
        var yInitialFloor = 0f;
        var yFinalFloor = -46.5f;
        var frontWallRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f);
        var rightWallRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f);
        var leftWallRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f);

        // Checkpoints
        var initialCheckpoint = new Checkpoint(position: new XnaVector3(0f, yInitialFloor, 10f), glow: true);
        var checkpoint2 = new Checkpoint(position: new XnaVector3(0f, yFinalFloor, 380f), glow: false);
        var checkpoint3 = new Checkpoint(position: new XnaVector3(0f, yFinalFloor, 580f), glow: false);
        var finalCheckpoint = new Checkpoint(position: new XnaVector3(0f, 175f, 800f), glow: true);

        gameObjects.Add(initialCheckpoint);
        gameObjects.Add(checkpoint2);
        gameObjects.Add(checkpoint3);
        gameObjects.Add(finalCheckpoint);

        hud.SetCheckpoints([initialCheckpoint, checkpoint2, checkpoint3, finalCheckpoint]);

        // Pisos
        gameObjects.Add(new Floor(position: new XnaVector3(0f, yInitialFloor, 74.6f), width: 150f, length: 300f)); // Piso inicial
        gameObjects.Add(new Floor(position: new XnaVector3(0f, yFinalFloor, 591.2f), width: 150f, length: 450f)); // Piso final

        // Rampas
        gameObjects.Add(new Ramp(position: new XnaVector3(0f, -23.2f, 296f), width: 150f, length: 150f));

        // Paredes
        gameObjects.Add(new Wall(position: new XnaVector3(0f, 75f, -75f), rotation: frontWallRotation, width: 150f, height: 150f)); // Piso inicial - atras
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, 75f, 0f), rotation: rightWallRotation, width: 150f, height: 150f)); // Piso inicial - derecha (1)
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, 75f, 150f), rotation: rightWallRotation, width: 150f, height: 150f)); // Piso inicial - derecha (2
        gameObjects.Add(new Wall(position: new XnaVector3(75f, 75f, 0f), rotation: leftWallRotation, width: 150f, height: 150f)); // Piso final - izquierda (1)
        gameObjects.Add(new Wall(position: new XnaVector3(75f, 75f, 150f), rotation: leftWallRotation, width: 150f, height: 150f)); // Piso final - izquierda (2)
        gameObjects.Add(new Wall(position: new XnaVector3(-75f, 28.3f, 742f), rotation: rightWallRotation, width: 150f, height: 150f)); // Piso final - derecha
        gameObjects.Add(new Wall(position: new XnaVector3(75f, 28.3f, 742f), rotation: leftWallRotation, width: 150f, height: 150f)); // Piso final - izquierda
        gameObjects.Add(new Wall(position: new XnaVector3(0f, 28.3f, 817f), rotation: frontWallRotation, width: 150f, height: 150f)); // Piso final - atras

        // Power Ups de Velocidad
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(45f, yFinalFloor, 442f)));
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(0f, yFinalFloor, 394f)));
        gameObjects.Add(new LowSpeedPowerUp(position: new XnaVector3(-44f, yFinalFloor, 515f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(0f, yFinalFloor, 442f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(-44f, yFinalFloor, 394f)));
        gameObjects.Add(new MediumSpeedPowerUp(position: new XnaVector3(45f, yFinalFloor, 515f)));
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(-44f, yFinalFloor, 442f)));
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(45f, yFinalFloor, 394f)));
        gameObjects.Add(new HighSpeedPowerUp(position: new XnaVector3(0f, yFinalFloor, 515f)));

        // Power Ups de Salto
        gameObjects.Add(new LowJumpPowerUp(position: new XnaVector3(-15f, yInitialFloor, 30f)));
        gameObjects.Add(new LowJumpPowerUp(position: new XnaVector3(-5f, yInitialFloor, 140f)));
        gameObjects.Add(new MediumJumpPowerUp(position: new XnaVector3(15f, yInitialFloor, 30f)));
        gameObjects.Add(new MediumJumpPowerUp(position: new XnaVector3(-47f, yInitialFloor, 92f)));
        gameObjects.Add(new HighJumpPowerUp(position: new XnaVector3(-15f, yInitialFloor, 70f)));
        gameObjects.Add(new HighJumpPowerUp(position: new XnaVector3(25f, yInitialFloor, 80f)));

        // Paredes Obstaculos
        gameObjects.Add(new KinematicWall(position: new XnaVector3(0f, yInitialFloor + 11f, 225f), width: 40f, height: 20f, movementSpeed: 50f));

        for (int i = 0; i < 5; i++)
        {
            gameObjects.Add(new KinematicWall(position: new XnaVector3(0f, -36.5f, 410f + 21f * i), width: 20f, height: 20f, movementSpeed: 50f - i * 4));
        }

        // Pisos Flotantes Movedizos
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(10f, -44.7f, 712f), movementDirection: XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(20f, -24.7f, 712f), movementDirection: XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-10f, -4.7f, 712f), movementDirection: XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-20f, 15.3f, 712f), movementDirection: XnaVector3.Backward));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(10f, 35.3f, 712f), movementDirection: XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(20f, 55.3f, 712f), movementDirection: XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-10f, 75.3f, 712f), movementDirection: XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-20f, 95.3f, 712f), movementDirection: XnaVector3.Backward));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(10f, 115.3f, 712f), movementDirection: XnaVector3.Left));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(20f, 135.3f, 712f), movementDirection: XnaVector3.Forward));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-10f, 175.3f, 712f), movementDirection: XnaVector3.Right));
        gameObjects.Add(new KinematicFloor(initialPosition: new XnaVector3(-20f, 195.3f, 712f), movementDirection: XnaVector3.Backward));

        // Cajas Dinamicas Dispersas
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-30f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.53f * 2 * MathF.PI), sideLength: 2f));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(22f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.17f * 2 * MathF.PI), sideLength: 2f));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(52f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.67f * 2 * MathF.PI), sideLength: 3f));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-55f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.96f * 2 * MathF.PI), sideLength: 5f));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-10f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.63f * 2 * MathF.PI), sideLength: 6f));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(30f, yFinalFloor + 10f, 550f), initialRotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.30f * 2 * MathF.PI), sideLength: 8f));

        // Cajas Dinamicas en Piramide
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-15f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-10f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-5f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(0f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(5f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(10f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(15f, 5f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-10f, 10f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-5f, 10f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(0f, 10f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(5f, 10f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(10f, 10f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(-5f, 15f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(0f, 15f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(5f, 15f, 180f)));
        gameObjects.Add(new DynamicBox(initialPosition: new XnaVector3(0f, 20f, 180f)));

        // Cajas Estáticas Dispersas
        gameObjects.Add(new StaticBox(position: new XnaVector3(46.72f, 2.25f, 137.13f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.75f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-32.37f, 2.25f, 88.80f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.92f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-27.37f, 2.25f, 88.80f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.92f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12.16f, 2.25f, 121.03f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.48f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(21.15f, 2.25f, 148.35f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 6.23f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(21.15f, 5.25f, 148.35f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 6.23f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12.46f, 2.25f, 153.14f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.75f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-7.46f, 2.25f, 153.14f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.75f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(53.77f, 2.25f, -42.27f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.74f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(28.55f, 2.25f, 36.97f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 4.12f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(17.43f, 2.25f, 17.62f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.27f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(22.43f, 2.25f, 17.62f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.27f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12.02f, 2.25f, 6.44f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.80f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12.02f, 5.25f, 6.44f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.80f), sideLength: 3f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6.22f, 3.75f, -15.34f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.57f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(55.90f, 3.75f, 179.12f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.16f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(51.28f, 3.75f, 67.12f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.18f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(59.28f, 3.75f, 67.12f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.18f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12.90f, 5.25f, 74.10f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.81f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-33.26f, 5.25f, 114.40f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.76f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-33.26f, 14.25f, 114.40f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.76f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-8.70f, 5.25f, -30.37f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.68f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-8.70f, 14.25f, -30.37f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.68f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-25.83f, 3.75f, 191.58f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.79f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-17.83f, 3.75f, 191.58f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.79f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(51.50f, 3.75f, 9.71f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.11f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(59.50f, 3.75f, 9.71f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.11f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(9.61f, 3.75f, 118.55f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.82f), sideLength: 6f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(40.65f, 5.25f, 154.09f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.73f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(40.65f, 14.25f, 154.09f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.73f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-48.53f, 5.25f, 145.82f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.60f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-37.53f, 5.25f, 145.82f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 3.60f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30.75f, 5.25f, 117.62f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.85f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-3.55f, 5.25f, 58.18f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.08f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-3.55f, 14.25f, 58.18f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 2.08f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-25.38f, 5.25f, -12.74f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 6.07f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-34.37f, 5.25f, -42.76f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.58f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-34.37f, 14.25f, -42.76f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.58f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-38.51f, 5.25f, 41.18f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.82f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-27.51f, 5.25f, 41.18f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 1.82f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-50.22f, 5.25f, 22.47f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.33f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-50.22f, 14.25f, 22.47f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.33f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-11.93f, 5.25f, 90.40f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 4.31f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-0.93f, 5.25f, 90.40f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 4.31f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42.11f, 5.25f, 84.89f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 4.42f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42.11f, 14.25f, 84.89f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 4.42f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-51.12f, 5.25f, 175.91f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.73f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-40.12f, 5.25f, 175.91f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 5.73f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(22.30f, 5.25f, 189.16f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.79f), sideLength: 9f));
        gameObjects.Add(new StaticBox(position: new XnaVector3(22.30f, 14.25f, 189.16f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0.79f), sideLength: 9f));

        // Cajas Estáticas en Piramide
        gameObjects.Add(new StaticBox(position: new XnaVector3(-60f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-48f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(48f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(60f, -41.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-60f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-48f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(48f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(60f, -31.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-54f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-42f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(54f, -21.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-54f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-42f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(54f, -11.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-48f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(48f, -1.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-48f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(48f, 9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-42f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42f, 19.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-42f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(42f, 29.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, 39.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-36f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(36f, 49.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, 59.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-30f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(30f, 69.6f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, 79.7f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 79.7f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 79.7f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 79.7f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, 79.7f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-24f, 89.8f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 89.8f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 89.8f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 89.8f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(24f, 89.8f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 99.9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 99.9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 99.9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 99.9f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-18f, 110f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 110f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 110f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(18f, 110f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 120.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 120.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 120.1f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-12f, 130.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 130.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(12f, 130.2f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 140.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 140.3f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(-6f, 150.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(6f, 150.4f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 160.5f, 800f)));
        gameObjects.Add(new StaticBox(position: new XnaVector3(0f, 170.6f, 800f)));
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