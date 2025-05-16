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
    private GraphicsDeviceManager Graphics { get; }
    private readonly ModelManager ModelManager;
    private readonly EffectManager EffectManager;
    private readonly PhysicsManager PhysicsManager;
    private readonly TextureManager TextureManager;
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
    private StaticCylinder StaticCylinder;

    private List<object> mapObjects;
    private List<DynamicBox> movingBoxes = new List<DynamicBox>();
    private List<XnaVector3> boxStartPositions = new List<XnaVector3>();
    private List<DynamicRamp> movingRamps = new List<DynamicRamp>();
    private List<DynamicBox> cajasParaEmpujar = new List<DynamicBox>();
    private List<XnaVector3> rampStartPositions = new List<XnaVector3>();
    private float tiempoTotal;




    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        PhysicsManager = new PhysicsManager();
        TextureManager = new TextureManager();
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

        // Tanda 1 Pisos
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

        //Sus paredes

        for (int i = 0; i <= 3; i++)
        {
            var rightWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(150f + i * 150, 5f, 45f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(rightWall);

            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(150f + i * 150, 5f, -45f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);
        }



        float boxWidth = 7f;
        float boxLength = 7f;
        float boxHeight = 7f;

        for (int i = 0; i <= 4; i++)
        {
            float floorX = 150f + i * 150;
            var floor = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(floorX, 0f, 0f),
                rotation: XnaQuaternion.Identity,
                width: 150f,
                length: 90f,
                color: Color.SandyBrown);

            mapObjects.Add(floor);

            // Generamos cajas
            for (int j = 0; j < 5; j++)
            {
                // fórmula que parece aleatoria pero no lo es
                float offsetX = (float)(Math.Sin((i + 1) * (j + 3)) * 50);
                float offsetZ = (float)(Math.Cos((i + 2) * (j + 5)) * 40);

                float posX = floorX + offsetX;
                float posY = boxHeight / 2f;
                float posZ = offsetZ;

                var box = new DynamicBox(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(posX, posY, posZ),
                    rotation: XnaQuaternion.Identity,
                    width: boxWidth,
                    length: boxLength,
                    height: boxHeight,
                    friction: 0.7f,
                    mass: 10f,
                    color: Color.Purple);

                mapObjects.Add(box);
                movingBoxes.Add(box);
                boxStartPositions.Add(new XnaVector3(posX, posY, posZ));
            }
        }



        //Primera "Curva"

        var curve1leftWall = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(825, 5f, 0f),
            XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
            width: 90f,
            length: 10f,
            color: Color.SeaGreen);
        mapObjects.Add(curve1leftWall);

        var curve1leftWall2 = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(750, 5f, -45f),
            XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
            width: 150f,
            length: 10f,
            color: Color.SeaGreen);
        mapObjects.Add(curve1leftWall2);

        var curve1rightWall = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(705, 5f, 45f),
            XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
            width: 60f,
            length: 10f,
            color: Color.SeaGreen);
        mapObjects.Add(curve1rightWall);


        // Tanda 2 Pisos

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

        //Rampas que se mueven

        for (int i = 0; i <= 16; i++)
        {
            var ramp = new DynamicRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(820f - 5f * i, -1f, 340f),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, MathF.PI / 2f),
                    width: 10f,
                    length: 80f,
                    color: Color.Gray);
            mapObjects.Add(ramp);
            movingRamps.Add(ramp);
            rampStartPositions.Add(new XnaVector3(820 - 5f * i, -1f, 340f));
        }

        //Sus paredes

        for (int i = 0; i <= 1; i++)
        {
            var rightWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(735, 5f, 120f + i * 150),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(rightWall);

            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(825, 5f, 120f + i * 150),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);
        }

        // Tanda 3 Pisos

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
            var rightWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(735, 5f, 120f + 390f + i * 150f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(rightWall);

            if (i == 1)
                continue;


            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(825, 5f, 120f + 390f + i * 150f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);
        }


        //Tanda 4 de pisos

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

        //Sus paredes

        for (int i = 0; i <= 1; i++)
        {
            var rightWall = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(900 + i * 150, 5f, 735),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                    width: 150f,
                    length: 10f,
                    color: Color.SeaGreen);
            mapObjects.Add(rightWall);


            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(900 + i * 150, 5f, 645f),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                width: 150f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);

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

        //Tanda 5 de pisos

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
                    position: new XnaVector3(1369f + i * 50, 90f, 640f + j * 50),
                    rotation: XnaQuaternion.Identity,
                    width: 50f,
                    length: 50f,
                    color: Color.Green);
                mapObjects.Add(rampFloor);
            }

        }

        // cajas a empujar
        var random = new Random();

        int boxCount = 10;
        for (int k = 0; k < boxCount; k++)
        {
            float x = 1369f + random.Next(0, 3) * 50f + (float)random.NextDouble() * 20f - 10f;
            float z = 640f + random.Next(0, 3) * 50f + (float)random.NextDouble() * 20f - 10f;

            // Evita el centro (1,1)
            if (Math.Abs(x - (1369f + 1 * 50f)) < 5f && Math.Abs(z - (640f + 1 * 50f)) < 5f)
                continue;

            float y = 110f + (float)random.NextDouble() * 10f; // Altura aleatoria para que caigan

            var box = new DynamicBox(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(x, y, z),
                rotation: XnaQuaternion.Identity,
                width: 4f,
                length: 4f,
                height: 4f,
                friction: 0.7f,
                mass: 4f,
                color: new Color(random.Next(256), random.Next(256), random.Next(256)));

            mapObjects.Add(box);
            cajasParaEmpujar.Add(box);
        }



        //Sus paredes

        for (int i = 0; i <= 2; i++)
        {

            var rightWall = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1369f + i * 50, 95f, 765f),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                    width: 50f,
                    length: 10f,
                    color: Color.SeaGreen);
            mapObjects.Add(rightWall);


            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(1369f + i * 50, 95f, 615),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                width: 50f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);

        }

        for (int i = 0; i <= 2; i++)
        {

            var rightWall = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1494, 95f, 640f + i * 50),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                    width: 50f,
                    length: 10f,
                    color: Color.SeaGreen);
            mapObjects.Add(rightWall);

            if (i == 1)
                continue;
            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(1344f, 95f, 640f + i * 50),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 50f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);

        }

        //Tanda 6 de pisos

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

        //Sus paredes

        for (int i = 0; i <= 2; i++)
        {

            var rightWall = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1600f + i * 50, 95f, 765f),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                    width: 50f,
                    length: 10f,
                    color: Color.SeaGreen);
            mapObjects.Add(rightWall);


            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(1600f + i * 50, 95f, 615),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f),
                width: 50f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);

        }

        for (int i = 0; i <= 2; i++)
        {

            var rightWall = new FloorWallRamp(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(1725, 95f, 640f + i * 50),
                    XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                    width: 50f,
                    length: 10f,
                    color: Color.SeaGreen);
            mapObjects.Add(rightWall);

            var leftWall = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position: new XnaVector3(1575, 95f, 640f + i * 50),
                XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f) * XnaQuaternion.CreateFromAxisAngle(XnaVector3.Backward, MathF.PI / 2f),
                width: 50f,
                length: 10f,
                color: Color.SeaGreen);
            mapObjects.Add(leftWall);

        }




        StaticCylinder = new StaticCylinder(
                    ModelManager,
                    EffectManager,
                    PhysicsManager,
                    GraphicsDevice,
                    position: new XnaVector3(2000f, -100f, 720f),
                    rotation: XnaQuaternion.Identity,
                    height: 30f,
                    radius: 15f,
                    color: Color.Brown
                    );
        mapObjects.Add(StaticCylinder);


        for (int i = 0; i < 60; i++)
        {
            float angle = i * MathF.PI / 6f;      // 30° entre ramps
            float radius = 100f - i * 1.25f;      // se va cerrando
            float y = 100f - i * 2.5f;            // va bajando

            var position = new XnaVector3(
                2000f + radius * MathF.Cos(angle),
                y,
                720f + radius * MathF.Sin(angle)
            );

            var rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, -angle + MathF.PI / 2f);

            // Color dinámico: va de rojo a azul e
            float t = i / 59f;
            var color = Color.Lerp(Color.Red, Color.Blue, t);

            var spiralRamp = new FloorWallRamp(
                ModelManager,
                EffectManager,
                PhysicsManager,
                GraphicsDevice,
                position,
                rotation,
                width: 30f,
                length: 20f,
                color: color);

            mapObjects.Add(spiralRamp);
        }


        var finalFloor = new FloorWallRamp(
            ModelManager,
            EffectManager,
            PhysicsManager,
            GraphicsDevice,
            position: new XnaVector3(2000f, -300f, 720f),
            rotation: XnaQuaternion.Identity,
            width: 250f,
            length: 250f,
            color: Color.Black);
        mapObjects.Add(finalFloor);



        PlayerBall = new PlayerBall(
            ModelManager,
            EffectManager,
            PhysicsManager,
            TextureManager,
            GraphicsDevice,
            initialPosition: new XnaVector3(0f, 50f, 0f),
                ballType: BallType.Rubber
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

        tiempoTotal += deltaTime;

        for (int i = 0; i < movingBoxes.Count; i++)
        {
            DynamicBox box = movingBoxes[i];
            var startPos = boxStartPositions[i];

            float offset = (float)Math.Sin(tiempoTotal * 2f + i) * 10f;

            var newPosition = new XnaVector3(startPos.X, startPos.Y, startPos.Z + offset);

            PhysicsManager.SetPosition(box.Handle, newPosition);
            box.Update(deltaTime, TargetCamera);

        }

        for (int i = 0; i < movingRamps.Count; i++)
        {
            DynamicRamp ramp = movingRamps[i];
            var startPos = rampStartPositions[i];

            float angleDegrees = -Math.Abs((float)Math.Sin(tiempoTotal + i)) * 45f;
            float angleRadians = MathHelper.ToRadians(angleDegrees);
            var rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, angleRadians);

            PhysicsManager.SetPosition(ramp.Handle, startPos);
            PhysicsManager.SetRotation(ramp.Handle, rotation);
            ramp.Update(deltaTime, TargetCamera);
        }

        foreach (DynamicBox caja in cajasParaEmpujar)
        {
            caja.Update(deltaTime, TargetCamera);
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
            else if (obj is StaticCylinder staticCylinder)
                staticCylinder.Draw(view, projection);
            else if (obj is DynamicRamp dynamicRamp)
                dynamicRamp.Draw(view, projection);

        }
    }

}