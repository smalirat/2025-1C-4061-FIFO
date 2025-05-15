using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
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
    private ModelManager ModelManager;
    private EffectManager EffectManager;
    private PhysicsManager PhysicsManager;

    private TargetCamera TargetCamera { get; set; }

    private PlayerBall PlayerBall;
    private Floor Piso;

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

        PlayerBall = new PlayerBall(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            initialPosition: new Vector3(0f, 3f, 0f),
            ballType: BallType.Goma
        );

        Piso = new Floor(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new Vector3(0f, 0f, 0f),
            rotation: Quaternion.Identity,
            width: 100f,
            length: 100f,
            color: Color.Brown);

        TargetCamera = new TargetCamera(
            fov: MathF.PI / 3f,
            aspectRatio: GraphicsDevice.Viewport.AspectRatio,
            nearPlaneDistance: 0.1f,
            farPlaneDistance: 1000000f,
            initialTargetPosition: PlayerBall.Position,
            cameraTargetDistance: 30f,
            mouseSensitivity: 0.01f);

        SimpleSkybox = new SimpleSkyBox();

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

        PlayerBall.Update(keyboardState, deltaTime, TargetCamera);
        PhysicsManager.Update(deltaTime);
        TargetCamera.Update(PlayerBall.Position);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SimpleSkybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);
        Piso.Draw(TargetCamera.View, TargetCamera.Projection);
        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection);
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}