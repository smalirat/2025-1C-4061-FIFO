using BepuPhysics;
using BepuPhysics.Constraints;
using BepuUtilities.Memory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Skybox;

namespace TGC.MonoGame.TP;

public class TGCGame : Game
{
    public const string ContentFolderMusic = "Music/";
    public const string ContentFolderSounds = "Sounds/";
    public const string ContentFolderSpriteFonts = "SpriteFonts/";
    public const string ContentFolderTextures = "Textures/";

    private PhysicsObject box;
    private PhysicsObject sideBox;
    private GraphicsDeviceManager Graphics { get; }

    private Simulation Simulation;
    public BufferPool BufferPool { get; private set; }
    public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

    private ModelManager ModelManager;
    private EffectManager EffectManager;

    private TargetCamera TargetCamera { get; set; }

    private Nivel1 Nivel1;
    private Nivel2 Nivel2;
    private Nivel3 Nivel3;
    private Nivel4 Nivel4;

    private Pelota Pelota;

    private List<Piso> PisosAnillo = new();

    private SimpleSkyBox SimpleSkybox { get; set; }

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        BufferPool = new BufferPool();

        var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

        Simulation = Simulation.Create(
            BufferPool,
            new NarrowPhaseCallbacks(new SpringSettings(30, 1), maximumRecoveryVelocity: 2f, frictionCoefficient: 0.4f),
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -60, 0)),
            new SolveDescription(8, 4));

        Nivel1 = new Nivel1(ModelManager, EffectManager);
        Nivel2 = new Nivel2(ModelManager, EffectManager);
        Nivel3 = new Nivel3(ModelManager, EffectManager);
        Nivel4 = new Nivel4(ModelManager, EffectManager);

        // Generar pista rectangular con rampas ocasionales y altura acumulada
        int largo = 100;          // Cuántas baldosas a lo largo (X)
        int ancho = 10;           // Cuántas baldosas a lo ancho (Z)
        float tamano = 10f;
        float inclinacion = MathHelper.ToRadians(15f); // Inclinación de rampa
        int frecuenciaRampas = 8; // Cada cuántas filas se pone una rampa

        float alturaActual = 0f;  // Altura acumulada a medida que bajamos

        for (int x = 0; x < largo; x++)
        {
            bool esRampa = (x % frecuenciaRampas == 0 && x != 0);

            // Si esta fila es una rampa, la próxima fila debe bajar
            if (esRampa)
                alturaActual -= tamano * MathF.Tan(inclinacion);

            for (int z = 0; z < ancho; z++)
            {
                var posicion = new Vector3(x * tamano, alturaActual, z * tamano);
                Quaternion rotacion = esRampa
                    ? Quaternion.CreateFromAxisAngle(Vector3.Cross(Vector3.Right, Vector3.Down), inclinacion)
                    : Quaternion.Identity;

                var piso = new Piso(EffectManager, Simulation, GraphicsDevice, posicion, rotacion, tamano, tamano);
                PisosAnillo.Add(piso);
            }
        }

        // Colocar pelota centrada al inicio
        var centroZ = (ancho / 2f) * tamano;

        Pelota = new Pelota(
            EffectManager,
            Simulation,
            GraphicsDevice,
            initialPosition: new Vector3(0f, 3f, centroZ),
            diameter: 6f,
            mass: 0.6f
        );

        TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio);
        SimpleSkybox = new SimpleSkyBox();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        box = new PhysicsObject(
              ModelManager.BoxModel,
              EffectManager,
              Simulation,
              GraphicsDevice,
              position: new Vector3(495f, -40, 45f),
              rotation: Quaternion.Identity,
              width: 1000f,
              length: 1000f
          );

        sideBox = new PhysicsObject(
          ModelManager.BoxModel,
          EffectManager,
          Simulation,
          GraphicsDevice,
          position: new Vector3(20f, 0f, 45f),
          rotation: Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2),
          width: 20f,
          length: 20f
      );

        SimpleSkybox.LoadContent(Content, TiposSkybox.Nieve);

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float safeDeltaTime = Math.Max(deltaTime, 1f / 240f);

        Pelota.Update(keyboardState, deltaTime, TargetCamera);
        Simulation.Timestep(safeDeltaTime, ThreadDispatcher);
        TargetCamera.Update(Pelota.Position);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Matrix centro = Matrix.CreateTranslation(400f, -100, 20f);
        Matrix escala = Matrix.CreateScale(1000f, 40f, 1000f);
        // En el método Draw()
        box.Draw(TargetCamera.View, TargetCamera.Projection, escala, Color.SandyBrown, centro, Matrix.Identity);
        sideBox.Draw(
            TargetCamera.View,
            TargetCamera.Projection,
            Matrix.CreateScale(10f, 2f, 10f),
            Color.Red
        );

        SimpleSkybox.Draw(TargetCamera.View, TargetCamera.Projection, Pelota.Position, GraphicsDevice);

        Pelota.Draw(TargetCamera.View, TargetCamera.Projection);
        
        foreach (var piso in PisosAnillo)
            piso.Draw(TargetCamera.View, TargetCamera.Projection);

    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}
