using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Skybox;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO;

public class TGCGame : Game
{
    private readonly GraphicsDeviceManager Graphics;
    private readonly ModelManager ModelManager;
    private readonly EffectManager EffectManager;
    private readonly PhysicsManager PhysicsManager;
    private readonly TextureManager TextureManager;

    private TargetCamera TargetCamera;
    private PlayerBall PlayerBall;

    private List<FloorWallRamp> FloorWallRamps = new();

    private SimpleSkyBox Skybox;

    private List<DynamicBox> DynamicBoxes = new();
    private List<StaticBox> StaticBoxes = new();

    private List<KinematicWall> KinematicWalls = new();
    private List<KinematicFloor> KinematicFloors = new();

    private Random random;

    private List<SpeedPowerUp> SpeedPowerUps = new();
    private List<JumpPowerUp> JumpPowerUps = new();

    private List<Checkpoint> Checkpoints = new();

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        PhysicsManager = new PhysicsManager();
        TextureManager = new TextureManager();
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

        Skybox = new SimpleSkyBox(ModelManager, EffectManager, TextureManager, TiposSkybox.Roca);
        PlayerBall = new PlayerBall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(0, 50f, 0f), BallType.Metal);

        InitializeLevel1();
        InitializeLevel2();
        InitializeLevel3();

        TargetCamera = new TargetCamera(MathF.PI / 3f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100000f, PlayerBall.Position, 30f, 0.01f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        TextureManager.Load(Content);
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        PlayerBall.Update(keyboardState, deltaTime, TargetCamera);

        PhysicsManager.Update(deltaTime);
        TargetCamera.Update(PlayerBall.Position);

        foreach (var box in DynamicBoxes)
        {
            box.Update(deltaTime, TargetCamera);
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

        Skybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);

        foreach (var item in FloorWallRamps)
        {
            item.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var box in DynamicBoxes)
        {
            box.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var staticBox in StaticBoxes)
        {
            staticBox.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var powerUp in SpeedPowerUps)
        {
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var powerUp in JumpPowerUps)
        {
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection);
    }

    private void InitializeLevel1()
    {
        // Checkpoint
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0f, 0f, 0f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue));

        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, 0f, 0f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -0.1f, 150f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -23.5f, 296f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 10f), 150f, 150f, true, RampWallTextureType.Dirt));

        // Paredes
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, 75f, -75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));

        // Obstaculos
        KinematicWalls.Add(new KinematicWall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(0f, 11f, 225f), 40f, 20f, 1f, 1f, false, 50f));

        // Cajas dinamicas en piramide
        float boxSize = 5f;
        float spacing = 5f;
        XnaVector3 baseCenter = new XnaVector3(0f, 5f, 180f);

        for (int i = -3; i <= 3; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, 0f, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        for (int i = -2; i <= 2; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        for (int i = -1; i <= 1; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, 2 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            baseCenter + new XnaVector3(0f, 3 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));

        // PowerUps
        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-15f, 2f, 30f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.1f, Color.Yellow));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(15f, 2f, 30f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.2f, Color.Orange));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-15f, 2f, 70f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.3f, Color.Red));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(25f, 2f, 80f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.3f, Color.Red));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-47f, 2f, 92f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.2f, Color.Orange));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-5f, 2f, 140f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.1f, Color.Yellow));

        // Cajas estáticas
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
                float x = (float)(random.NextDouble() * 115.0 - 55.0);
                float z = (float)(random.NextDouble() * 260.0 - 60.0);
                position = new XnaVector2(x, z);

                // Verificar que no esté cerca de otra posición existente
                valid = staticBoxPositions.All(p => XnaVector2.Distance(p, position) >= 18f);
                attempts++;

                if (attempts > 1000)
                {
                    throw new Exception("No se pudieron generar suficientes posiciones únicas");
                }
            } while (!valid);

            staticBoxPositions.Add(position);
        }

        for (int i = 0; i < staticBoxPositions.Count; i++)
        {
            float x = staticBoxPositions[i].X;
            float z = staticBoxPositions[i].Y;

            // Una caja sola tamaño random
            if (i % 3 == 0)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
            // Dos cajas del mismo tamaño apiladas
            else if (i % 3 == 1)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
                cajas.Add((new XnaVector3(x, size + size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
            else
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
                cajas.Add((new XnaVector3(x + size + 2f, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
        }

        foreach (var (pos, rot, size) in cajas)
        {
            StaticBoxes.Add(new StaticBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, pos, rot, size));
        }
    }

    private void InitializeLevel2()
    {
        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.5f, 442f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.6f, 442f + 150f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        // PowerUps
        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-44f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(45f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-44f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(45f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(-44f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(45f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        // Checkpoint
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0, -46.5f + 2f, 380f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue));

        // Obstaculos
        for (int i = 0; i < 5; i++)
        {
            KinematicWalls.Add(new KinematicWall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(0, -46.5f + 10f, 410f + 21f * i), 20f, 20f, 1f, 1f, false, 50f - i * 4));
        }

        // Cajas dinamicas
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-55f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 5, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-30f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-10, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 6, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(22f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(30f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 8, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(52f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 3, 1f, 0.1f));
    }

    private void InitializeLevel3()
    {
        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.7f, 742f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        // Paredes
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.7f + 75f, 742f + 75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));

        // Subi baja
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 2f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 22f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 42f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 62f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 82f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 102f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 122f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 142f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 162f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 182f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 222f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 242f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));

        // Cajas estaticas
        float size = 10f;
        float spacingX = 12f;
        float spacingY = size + 0.5f;
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

                    StaticBoxes.Add(new StaticBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                        new XnaVector3(x, y, z), XnaQuaternion.Identity, size));
                }

                filaY++;
            }

            cantidadActual--;
        }

        // Checkpoints
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0, -46.5f + 2f, 580f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue));

        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new XnaVector3(0f, 184f, 800f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue));
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}