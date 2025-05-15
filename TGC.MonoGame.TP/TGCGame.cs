using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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
    private FreeCamera FreeCamera { get; set; }
    private bool IsGodModeEnabled = true;


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
    private List<object> mapObjects;

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

        mapObjects = new List<object>();

        // Pisos conectados en horizontal (eje X)
        for (int i = 0; i <= 4; i++)
        {
            var floor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(150f + i * 150, 0f, 0f),
                rotation: XnaQuaternion.Identity,
                width: 150f,
                length: 90f,
                color: Color.SandyBrown);
            mapObjects.Add(floor);
        }


        for (int i = 0; i <= 1; i++)
        {
            var floor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(780, 0f, 120f + i * 150),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, MathF.PI / 2f),
                width: 150f,
                length: 90f,
                color: Color.IndianRed);
            mapObjects.Add(floor);
        }

        for (int i = 1; i <= 2; i++)
        {
            var floor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(780, 0f, 360f + i * 150f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, MathF.PI / 2f),
                width: 150f,
                length: 90f,
                color: Color.IndianRed);
            mapObjects.Add(floor);
        }

        for (int i = 0; i <= 1; i++)
        {
            var floor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(900 + i * 150, 0f, 690),
                XnaQuaternion.Identity,
                width: 150f,
                length: 90f,
                color: Color.BlueViolet);
            mapObjects.Add(floor);
        }

        for (int i = 0; i <= 2; i++)
        {
            var rampFloor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(1162f + i * 72.5f, 15f + i * 30f, 690f),
                rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 8),
                width: 80f,
                length: 30f,
                color: Color.Gray);

            mapObjects.Add(rampFloor);
        }

        for (int i = 0; i <= 2; i++)
        {
            for (int j = 0; j <= 2; j++)
            {
                if (i == 1 && j == 1)
                    continue;

                var rampFloor = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1367f + i * 50, 90f, 640f + j * 50),
                    rotation: XnaQuaternion.Identity,
                    width: 50f,
                    length: 50f,
                    color: Color.Green);
                mapObjects.Add(rampFloor);
            }

        }

        for (int i = 0; i <= 2; i++)
        {
            for (int j = 0; j <= 2; j++)
            {
                var rampFloor = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1600f + i * 50, 90f, 640f + j * 50),
                    rotation: XnaQuaternion.Identity,
                    width: 50f,
                    length: 50f,
                    color: Color.MediumVioletRed);
                mapObjects.Add(rampFloor);
            }

        }

        for (int i = 0; i < 20; i++)
        {
            float angle = i * MathF.PI / 6f;         // 30° entre ramps
            float radius = 40f - i * 1.5f;           // se va cerrando
            float y = 50f - i * 2.5f;                // va bajando

            var position = new XnaVector3(
                radius * MathF.Cos(angle),
                y,
                radius * MathF.Sin(angle)
            );

            var rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, -angle + MathF.PI / 2f);

            var spiralRamp = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position,
                rotation,
                width: 18f,
                length: 10f,
                color: Color.OrangeRed);

            mapObjects.Add(spiralRamp);


        }



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
            position: new XnaVector3(45.5f, 22f, 0f), // cambió eje de Z a X
            rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
            width: 150f,
            length: 45f,
            color: Color.SaddleBrown);

        Ramp = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(-60f, 15f, 0f),
            rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 4f),
            width: 150f,
            length: 45f,
            color: Color.RosyBrown);

        StaticBox = new StaticBox(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(30f, 5f, -20f),
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
            position: new XnaVector3(20f, 50f, 10f),
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
            position: new XnaVector3(10f, 0f, -20f),
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

        FreeCamera = new FreeCamera(
            aspectRatio: GraphicsDevice.Viewport.AspectRatio,
            position: new Vector3(1000f, 100f, 600f),
            screenCenter: GraphicsDevice.Viewport.Bounds.Center
        );

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

        if (IsGodModeEnabled)
        {
            FreeCamera.Update(gameTime);
        }
        else
        {
            DynamicBox.Update(deltaTime, TargetCamera);
            PlayerBall.Update(keyboardState, deltaTime, TargetCamera);
            TargetCamera.Update(PlayerBall.Position);
        }

        PhysicsManager.Update(deltaTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix view = IsGodModeEnabled ? FreeCamera.View : TargetCamera.View;
        Matrix projection = IsGodModeEnabled ? FreeCamera.Projection : TargetCamera.Projection;


        SimpleSkybox.Draw(view, projection, PlayerBall.Position, GraphicsDevice);
        Floor.Draw(view, projection);
        Wall.Draw(view, projection);
        Ramp.Draw(view, projection);
        StaticTree.Draw(view, projection);
        StaticStone.Draw(view, projection);
        DynamicBox.Draw(view, projection);
        StaticBox.Draw(view, projection);
        SpeedPowerUp.Draw(view, projection);
        JumpPowerUp.Draw(view, projection);
        Checkpoint.Draw(view, projection);
        PlayerBall.Draw(view, projection);
        DrawMap(view, projection);
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    private void DrawMap(Matrix view, Matrix projection)
    {
        foreach (var obj in mapObjects)
        {
            if (obj is FloorWallRamp floor)
                floor.Draw(view, projection);
            else if (obj is FloorWallRamp wall)
                wall.Draw(view, projection);
            else if (obj is FloorWallRamp ramp)
                ramp.Draw(view, projection);
            else if (obj is StaticBox staticBox)
                staticBox.Draw(view, projection);
            else if (obj is DynamicBox dynamicBox)
                dynamicBox.Draw(view, projection);
            else if (obj is StaticTree staticTree)
                staticTree.Draw(view, projection);
            else if (obj is StaticStone staticStone)
                staticStone.Draw(view, projection);
            else if (obj is JumpPowerUp jumpPowerUp)
                jumpPowerUp.Draw(view, projection);
            else if (obj is SpeedPowerUp speedPowerUp)
                speedPowerUp.Draw(view, projection);
            else if (obj is Checkpoint checkpoint)
                checkpoint.Draw(view, projection);
        }
    }

}