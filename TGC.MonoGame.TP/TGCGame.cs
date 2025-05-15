using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Objetos;
using TGC.MonoGame.TP.Objetos.Ball;
using TGC.MonoGame.TP.Skybox;

namespace TGC.MonoGame.TP;

public class TGCGame : Game
{
    private GraphicsDeviceManager Graphics { get; }
    private readonly ModelManager ModelManager;
    private readonly EffectManager EffectManager;
    private readonly PhysicsManager PhysicsManager;

    private TargetCamera TargetCamera { get; set; }

    private PlayerBall PlayerBall;
    private FloorWallRamp Floor;
    private FloorWallRamp Wall;
    private FloorWallRamp Ramp;

    private StaticBox StaticBox;
    private DynamicBox DynamicBox;

    private StaticTree StaticTree;

    private StaticStone StaticStone;

    private JumpPowerUp JumpPowerUp;
    private SpeedPowerUp SpeedPowerUp;

    private Checkpoint Checkpoint;

    private SimpleSkyBox SimpleSkybox { get; set; }

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        PhysicsManager = new PhysicsManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        PhysicsManager.Initialize();

        SimpleSkybox = new SimpleSkyBox();

        PlayerBall = new PlayerBall(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            initialPosition: new XnaVector3(0f, 50f, 0f),
            ballType: BallType.Goma
        );

        Floor = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(0f, 0f, 0f),
            rotation: XnaQuaternion.Identity,
            width: 150f,
            length: 90f,
            color: Color.Brown);

        Wall = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(0f, 22f, 45.5f),
            rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
            width: 150f,
            length: 45f,
            color: Color.SaddleBrown);

        Ramp = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(0f, 15f, -60f),
            rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 4f),
            width: 150f,
            length: 45f,
            color: Color.RosyBrown);

        StaticBox = new StaticBox(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(-20f, 5f, 30f),
            rotation: XnaQuaternion.Identity,
            width: 5f,
            length: 5f,
            height: 10f,
            color: Color.Purple);

        DynamicBox = new DynamicBox(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(10f, 50f, 20f),
            rotation: XnaQuaternion.Identity,
            width: 5f,
            length: 5f,
            height: 5f,
            friction: 0.5f,
            mass: 1f,
            color: Color.Blue);

        StaticTree = new StaticTree(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(35f, 0f, 35f),
            rotation: XnaQuaternion.Identity,
            radius: 5f,
            height: 20f,
            color: Color.Green);

        StaticStone = new StaticStone(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new Vector3(-25f, 0f, -25f),
            rotation: Quaternion.Identity,
            radius: 2f,
            height: 4f,
            color: Color.Gray);

        SpeedPowerUp = new SpeedPowerUp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new Vector3(30f, 8f, -40f),
            rotation: Quaternion.Identity,
            radius: 3f,
            color: Color.Turquoise);

        JumpPowerUp = new JumpPowerUp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new Vector3(-40f, 8f, 30f),
            rotation: Quaternion.Identity,
            radius: 3f,
            color: Color.Snow);

        Checkpoint = new Checkpoint(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(-20f, 0f, 10f),
            rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, MathF.PI / 2f),
            width: 20f,
            depth: 10f,
            height: 20f,
            color: Color.Wheat);

        TargetCamera = new TargetCamera(
            fov: MathF.PI / 3f,
            aspectRatio: GraphicsDevice.Viewport.AspectRatio,
            nearPlaneDistance: 0.1f,
            farPlaneDistance: 1000000f,
            initialTargetPosition: PlayerBall.Position,
            cameraTargetDistance: 30f,
            mouseSensitivity: 0.01f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);

        SimpleSkybox.LoadContent(Content, TiposSkybox.Nieve);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        DynamicBox.Update(deltaTime, TargetCamera);

        PlayerBall.Update(keyboardState, deltaTime, TargetCamera);

        PhysicsManager.Update(deltaTime);
        TargetCamera.Update(PlayerBall.Position);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SimpleSkybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);

        Floor.Draw(TargetCamera.View, TargetCamera.Projection);
        Wall.Draw(TargetCamera.View, TargetCamera.Projection);
        Ramp.Draw(TargetCamera.View, TargetCamera.Projection);

        StaticTree.Draw(TargetCamera.View, TargetCamera.Projection);
        StaticStone.Draw(TargetCamera.View, TargetCamera.Projection);
        DynamicBox.Draw(TargetCamera.View, TargetCamera.Projection);
        StaticBox.Draw(TargetCamera.View, TargetCamera.Projection);
        SpeedPowerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        JumpPowerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        Checkpoint.Draw(TargetCamera.View, TargetCamera.Projection);

        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection);
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}