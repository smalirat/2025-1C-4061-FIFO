using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Objetos;
using TGC.MonoGame.TP.Objetos.Ball;
using TGC.MonoGame.TP.Skybox;
using TGC.MonoGame.TP.Texturas;

namespace TGC.MonoGame.TP;

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

    private List<TransparentCheckpoint> Checkpoints = new();

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        PhysicsManager = new PhysicsManager();
        TextureManager = new TextureManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        random = new Random(2);
    }

    protected override void Initialize()
    {
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        PhysicsManager.Initialize();

        Skybox = new SimpleSkyBox(ModelManager, EffectManager, TextureManager, TiposSkybox.Roca);
        PlayerBall = new PlayerBall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new Vector3(0f, 50f, 0f), BallType.Rubber);

        InitializeLevel1();

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
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Skybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);

        foreach (var item in FloorWallRamps)
            item.Draw(TargetCamera.View, TargetCamera.Projection);

        foreach (var box in DynamicBoxes)
            box.Draw(TargetCamera.View, TargetCamera.Projection);

        foreach (var staticBox in StaticBoxes)
            staticBox.Draw(TargetCamera.View, TargetCamera.Projection);

        foreach (var powerUp in SpeedPowerUps)
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);

        foreach (var powerUp in JumpPowerUps)
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection);
    }

    private void InitializeLevel1()
    {
        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(0f, 0f, 0f), Quaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(0f, 0f, 150f), Quaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(0f, -23.5f, 296f), Quaternion.CreateFromAxisAngle(Vector3.Right, MathF.PI / 10f), 150f, 150f, true, RampWallTextureType.Dirt));

        // Paredes
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(0f, 75f, -75f), Quaternion.CreateFromAxisAngle(Vector3.Right, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(-75f, 75f, 0f), Quaternion.CreateFromAxisAngle(Vector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(75f, 75f, 0f), Quaternion.CreateFromAxisAngle(Vector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(-75f, 75f, 150f), Quaternion.CreateFromAxisAngle(Vector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new Vector3(75f, 75f, 150f), Quaternion.CreateFromAxisAngle(Vector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));

        // Checkpoint
        Checkpoints.Add(new TransparentCheckpoint(PhysicsManager, new Vector3(0f, 75f, 225f), Quaternion.CreateFromAxisAngle(Vector3.Right, MathF.PI / 2f), 150f, 150f));

        // Obstaculos
        KinematicWalls.Add(new KinematicWall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new Vector3(0f, 13f, 225f), 40f, 20f, 1f, 1f, false, 50f));

        // Cajas dinamicas en piramide
        float boxSize = 5f;
        float spacing = 5f;
        Vector3 baseCenter = new Vector3(0f, 5f, 180f);

        for (int i = -3; i <= 3; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new Vector3(i * spacing, 0f, 0f), Quaternion.Identity, boxSize, 1f, 1f));
        for (int i = -2; i <= 2; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new Vector3(i * spacing, spacing, 0f), Quaternion.Identity, boxSize, 1f, 1f));
        for (int i = -1; i <= 1; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new Vector3(i * spacing, 2 * spacing, 0f), Quaternion.Identity, boxSize, 1f, 1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            baseCenter + new Vector3(0f, 3 * spacing, 0f), Quaternion.Identity, boxSize, 1f, 1f));

        // PowerUps
        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new Vector3(40f, 2f, 180f), Quaternion.Identity, 3f, 3f, 1f, 30f, Color.Yellow));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice,
            new Vector3(-40f, 2f, 180f), Quaternion.CreateFromAxisAngle(Vector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.5f, Color.Red));


        // Cajas estáticas
        List<(Vector3 pos, Quaternion rot, float size)> cajas = new();
        for (var i = 0; i < 30; i++)
        {
            int tipo = random.Next(4);
            float[] sizes = new float[] { 3f, 6f, 9f };

            float floorZ = random.Next(2) == 0 ? 0f : 150f;
            float x = (float)(random.NextDouble() * 140f - 70f);
            float z = (float)(random.NextDouble() * 140f - 70f + floorZ);

            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.Up, (float)random.NextDouble());

            if (tipo == 0)
            {
                float size = sizes[random.Next(sizes.Length)];
                cajas.Add((new Vector3(x, size / 2f + 0.75f, z), rot, size));
            }
            else if (tipo == 1)
            {
                float baseSize = 9f;
                float middleSize = 6f;
                float topSize = 3f;
                var basePos = new Vector3(x, baseSize / 2f + 0.75f, z);
                var middlePos = basePos + new Vector3((baseSize + middleSize) / 2f + 0.01f, 0f, 0f);
                middlePos.Y = middleSize / 2f + 0.75f;
                var topPos = basePos + new Vector3((baseSize + topSize) / 2f + 0.01f, middleSize / 2f + 0.01f, 0f);
                cajas.Add((basePos, rot, baseSize));
                cajas.Add((middlePos, rot, middleSize));
                cajas.Add((topPos, rot, topSize));
            }
            else if (tipo == 2)
            {
                float size = sizes[random.Next(sizes.Length)];
                var leftPos = new Vector3(x, size / 2f + 0.75f, z);
                var rightPos = leftPos + new Vector3(size * 2f, 0f, 0f);
                cajas.Add((leftPos, rot, size));
                cajas.Add((rightPos, rot, size));
            }
            else
            {
                float size = sizes[random.Next(sizes.Length)];
                var bottomPos = new Vector3(x, size / 2f + 0.75f, z);
                var topPos = new Vector3(x, size + size / 2f + 0.75f, z);
                cajas.Add((bottomPos, rot, size));
                cajas.Add((topPos, rot, size));
            }
        }

        foreach (var (pos, rot, size) in cajas)
        {
            StaticBoxes.Add(new StaticBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, pos, rot, size));
        }
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}