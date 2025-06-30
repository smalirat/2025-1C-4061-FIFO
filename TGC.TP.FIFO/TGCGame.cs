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
using TGC.TP.FIFO.HUDS;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Objetos.PowerUps.Jump;
using TGC.TP.FIFO.Objetos.PowerUps.Speed;
using TGC.TP.FIFO.Skybox;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO;

public class TGCGame : Game
{
    private readonly GraphicsDeviceManager Graphics;
    private readonly PhysicsManager PhysicsManager;
    private SpriteBatch SpriteBatch;

    private TargetCamera TargetCamera;
    private HUD HUD;

    private Random random;
    private GameMenu GameMenu;
    private SimpleSkyBox Skybox;

    private PlayerBall PlayerBall;

    private readonly List<Floor> Floors = [];
    private readonly List<Wall> Walls = [];
    private readonly List<DynamicBox> DynamicBoxes = [];
    private readonly List<StaticBox> StaticBoxes = [];
    private readonly List<KinematicWall> KinematicWalls = [];
    private readonly List<KinematicFloor> KinematicFloors = [];
    private readonly List<SpeedPowerUp> SpeedPowerUps = [];
    private readonly List<JumpPowerUp> JumpPowerUps = [];
    private readonly List<Checkpoint> Checkpoints = [];

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        PhysicsManager = new PhysicsManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        random = new Random(6814);
    }

    protected override void Initialize()
    {
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        PhysicsManager.Initialize();

        SpriteBatch = new SpriteBatch(GraphicsDevice);
        HUD = new HUD(SpriteBatch, GraphicsDevice);
        Skybox = new SimpleSkyBox(GraphicsDevice);
        PlayerBall = new PlayerBall(PhysicsManager, GraphicsDevice, new Vector3(0, 50f, 0f));

        InitializeScenario();

        TargetCamera = new TargetCamera(MathF.PI / 3f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100000f, PlayerBall.Position, 30f, 0.01f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        TextureManager.Load(Content);
        AudioManager.Load(Content);
        FontsManager.Load(Content);
        HUD.LoadContent(Content);

        GameMenu = new GameMenu(PhysicsManager, GraphicsDevice, SpriteBatch, Exit, NewGame, PlayerBall.Reset);
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
            GameMenu.ChangeToOptionsMenu();
            AudioManager.StopRollingSound();
            return;
        }

        if (!GameState.Playing)
        {
            GameMenu.Update(keyboardState, deltaTime, TargetCamera);
            return;
        }

        GameState.CheckIfPlayerLost();

        PlayerBall.Update(keyboardState, deltaTime, TargetCamera);

        PhysicsManager.Update(deltaTime);
        TargetCamera.Update(PlayerBall.Position);

        foreach (var dynamicBox in DynamicBoxes)
        {
            dynamicBox.Update(deltaTime, TargetCamera);
        }

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Update(deltaTime, TargetCamera);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Update(deltaTime, TargetCamera);
        }

        foreach (var jumpPowerUp in JumpPowerUps)
        {
            jumpPowerUp.Update(deltaTime);
        }

        foreach (var speedPowerUp in SpeedPowerUps)
        {
            speedPowerUp.Update(deltaTime);
        }

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Update(deltaTime);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var eyePosition = TargetCamera.Position;

        if (!GameState.Playing)
        {
            GameMenu.Draw(gameTime, GraphicsDevice);
            return;
        }

        Skybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);

        foreach (var floors in Floors)
        {
            floors.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var walls in Walls)
        {
            walls.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var dynamicBox in DynamicBoxes)
        {
            dynamicBox.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var staticBox in StaticBoxes)
        {
            staticBox.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var speedPowerUp in SpeedPowerUps)
        {
            speedPowerUp.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var jumpPowerUp in JumpPowerUps)
        {
            jumpPowerUp.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        }

        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection, EffectManager.LightPosition, eyePosition);
        HUD.Draw(PlayerBall, Checkpoints);

        base.Draw(gameTime);
    }

    private void InitializeScenario()
    {
        // Checkpoints
        Checkpoints.Add(new Checkpoint(PhysicsManager, position: new XnaVector3(0f, 0f, 10f), glow: true)); // Inicial
        Checkpoints.Add(new Checkpoint(PhysicsManager, position: new XnaVector3(0f, -44.5f, 380f), glow: false));
        Checkpoints.Add(new Checkpoint(PhysicsManager, position: new XnaVector3(0f, -44.5f, 580f), glow: false));
        Checkpoints.Add(new Checkpoint(PhysicsManager, position: new XnaVector3(0f, 175f, 800f), glow: true)); // Final

        // Pisos
        Floors.Add(new Floor(PhysicsManager, GraphicsDevice, new XnaVector3(0f, 0f, 74.6f), XnaQuaternion.Identity, 150f, 300f));
        Floors.Add(new Floor(PhysicsManager, GraphicsDevice, new XnaVector3(0f, -46.5f, 442f + 74.6f * 2), XnaQuaternion.Identity, 150f, 450f));
        Floors.Add(new Floor(PhysicsManager, GraphicsDevice, new XnaVector3(0f, -23.2f, 296f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 10f), 150f, 150f));

        // Paredes
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(0f, 75f, -75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(-75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(-75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(0f, -46.7f + 75f, 742f + 75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(-75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f));
        Walls.Add(new Wall(PhysicsManager, GraphicsDevice, new XnaVector3(75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f));

        // Power Ups de Velocidad
        SpeedPowerUps.Add(new HighSpeedPowerUp(PhysicsManager, position: new XnaVector3(-44f, -44.5f, 442f)));
        SpeedPowerUps.Add(new HighSpeedPowerUp(PhysicsManager, position: new XnaVector3(45f, -44.5f, 394f)));
        SpeedPowerUps.Add(new HighSpeedPowerUp(PhysicsManager, position: new XnaVector3(0f, -44.5f, 515f)));
        SpeedPowerUps.Add(new MediumSpeedPowerUp(PhysicsManager, position: new XnaVector3(0f, -44.5f, 442f)));
        SpeedPowerUps.Add(new MediumSpeedPowerUp(PhysicsManager, position: new XnaVector3(-44f, -44.5f, 394f)));
        SpeedPowerUps.Add(new MediumSpeedPowerUp(PhysicsManager, position: new XnaVector3(45f, -44.5f, 515f)));
        SpeedPowerUps.Add(new LowSpeedPowerUp(PhysicsManager, position: new XnaVector3(45f, -44.5f, 442f)));
        SpeedPowerUps.Add(new LowSpeedPowerUp(PhysicsManager, position: new XnaVector3(0f, -44.5f, 394f)));
        SpeedPowerUps.Add(new LowSpeedPowerUp(PhysicsManager, position: new XnaVector3(-44f, -44.5f, 515f)));

        // Power Ups de Salto
        JumpPowerUps.Add(new LowJumpPowerUp(PhysicsManager, position: new XnaVector3(-15f, 2f, 30f)));
        JumpPowerUps.Add(new LowJumpPowerUp(PhysicsManager, position: new XnaVector3(-5f, 2f, 140f)));
        JumpPowerUps.Add(new MediumJumpPowerUp(PhysicsManager, position: new XnaVector3(15f, 2f, 30f)));
        JumpPowerUps.Add(new MediumJumpPowerUp(PhysicsManager, position: new XnaVector3(-47f, 2f, 92f)));
        JumpPowerUps.Add(new HighJumpPowerUp(PhysicsManager, position: new XnaVector3(-15f, 2f, 70f)));
        JumpPowerUps.Add(new HighJumpPowerUp(PhysicsManager, position: new XnaVector3(25f, 2f, 80f)));

        // Paredes Obstaculos
        KinematicWalls.Add(new KinematicWall(PhysicsManager, GraphicsDevice, new XnaVector3(0f, 11f, 225f), 40f, 20f, 50f));

        for (int i = 0; i < 5; i++)
        {
            KinematicWalls.Add(new KinematicWall(PhysicsManager, GraphicsDevice, new XnaVector3(0, -46.5f + 10f, 410f + 21f * i), 20f, 20f, 150f - i * 4));
        }

        // Pisos Flotantes Movedizos
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(10f, -46.7f + 2f, 712f), XnaVector3.Left));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(20f, -46.7f + 22f, 712f), XnaVector3.Forward));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-10f, -46.7f + 42f, 712f), XnaVector3.Right));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-20f, -46.7f + 62f, 712f), XnaVector3.Backward));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(10f, -46.7f + 82f, 712f), XnaVector3.Left));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(20f, -46.7f + 102f, 712f), XnaVector3.Forward));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-10f, -46.7f + 122f, 712f), XnaVector3.Right));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-20f, -46.7f + 142f, 712f), XnaVector3.Backward));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(10f, -46.7f + 162f, 712f), XnaVector3.Left));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(20f, -46.7f + 182f, 712f), XnaVector3.Forward));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-10f, -46.7f + 222f, 712f), XnaVector3.Right));
        KinematicFloors.Add(new KinematicFloor(PhysicsManager, GraphicsDevice, new XnaVector3(-20f, -46.7f + 242f, 712f), XnaVector3.Backward));

        // Cajas Dinamicas Dispersas
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(-55f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 5, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(-30f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(-10f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 6, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(22f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(30f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 8, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, new XnaVector3(52f, -36.5f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 3, 1f, 0.1f));

        // Cajas Dinamicas en Piramide
        float boxSize = 5f;
        float spacing = 5f;
        XnaVector3 baseCenter = new XnaVector3(0f, 5f, 180f);

        for (int i = -3; i <= 3; i++)
        {
            DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, baseCenter + new XnaVector3(i * spacing, 0f, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        for (int i = -2; i <= 2; i++)
        {
            DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, baseCenter + new XnaVector3(i * spacing, spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        for (int i = -1; i <= 1; i++)
        {
            DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, baseCenter + new XnaVector3(i * spacing, 2 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        }

        DynamicBoxes.Add(new DynamicBox(PhysicsManager, GraphicsDevice, baseCenter + new XnaVector3(0f, 3 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));

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
            StaticBoxes.Add(new StaticBox(PhysicsManager, GraphicsDevice, pos, rot, ssize));
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

                    StaticBoxes.Add(new StaticBox(PhysicsManager, GraphicsDevice, new XnaVector3(x, y, z), XnaQuaternion.Identity, size));
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

        foreach (Checkpoint checkpoint in Checkpoints)
        {
            checkpoint.Reset();
        }

        foreach (DynamicBox dynamicBox in DynamicBoxes)
        {
            dynamicBox.Reset();
        }

        PlayerBall.Reset();
    }
}