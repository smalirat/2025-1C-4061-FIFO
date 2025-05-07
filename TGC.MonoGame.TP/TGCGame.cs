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

namespace TGC.MonoGame.TP;

/// <summary>
///     Esta es la clase principal del juego.
///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
/// </summary>
public class TGCGame : Game
{
    // Paths de contenidos
    public const string ContentFolderMusic = "Music/";
    public const string ContentFolderSounds = "Sounds/";
    public const string ContentFolderSpriteFonts = "SpriteFonts/";
    public const string ContentFolderTextures = "Textures/";

    // Graphics device manager
    private GraphicsDeviceManager Graphics { get; }

    // Motor de fisica
    private Simulation Simulation;

    public BufferPool BufferPool { get; private set; }
    public SimpleThreadDispatcher ThreadDispatcher { get; private set; }

    // Managers
    private ModelManager ModelManager;

    private EffectManager EffectManager;

    // Camaras
    private TargetCamera TargetCamera { get; set; }

    // Niveles
    private Nivel1 Nivel1;
    private Nivel2 Nivel2;
    private Nivel3 Nivel3;
    private Nivel4 Nivel4;

    // Piso
    private Piso Piso;

    // Pelota
    private Pelota Pelota;

    public TGCGame()
    {
        // Maneja la configuracion y la administracion del dispositivo grafico.
        Graphics = new GraphicsDeviceManager(this);

        // Carga y encapsula a todos los modelos
        ModelManager = new ModelManager();

        // Carga y encapsula a todos los efectos
        EffectManager = new EffectManager();

        // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
        // Carpeta raiz donde va a estar toda la Media.
        Content.RootDirectory = "Content";

        // Hace que el mouse sea visible.
        IsMouseVisible = true;
    }

    /// <summary>
    ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
    ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
    ///     La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.
    /// </summary>
    protected override void Initialize()
    {
        // Configuro las dimensiones de la pantalla.
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        // Configuramos el motor de fisica
        BufferPool = new BufferPool();

        var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

        Simulation = Simulation.Create(
            BufferPool,
            new NarrowPhaseCallbacks(new SpringSettings(30, 1), maximumRecoveryVelocity: 2f, frictionCoefficient: 0.4f),
            new PoseIntegratorCallbacks(new System.Numerics.Vector3(0, -60, 0)), // -30f es la gravedad
            new SolveDescription(8, 4));

        // Niveles
        Nivel1 = new Nivel1(ModelManager, EffectManager);
        Nivel2 = new Nivel2(ModelManager, EffectManager);
        Nivel3 = new Nivel3(ModelManager, EffectManager);
        Nivel4 = new Nivel4(ModelManager, EffectManager);

        // Pelota
        Pelota = new Pelota(EffectManager, Simulation, GraphicsDevice, initialPosition: new Vector3(0, 10f, 0), diameter: 6f, mass: 0.6f);

        // Piso
        Piso = new Piso(EffectManager, Simulation, GraphicsDevice, new Vector3(0, 0, 0), 300f, 150f);

        // Configuramos la camara
        TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio);

        base.Initialize();
    }

    /// <summary>
    ///     Llamada una sola vez durante la inicializacion de la aplicacion, luego de Initialize, y una vez que fue configurado GraphicsDevice.
    ///     Debe ser usada para cargar los recursos y otros elementos del contenido.
    ///     Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
    /// </summary>
    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        base.LoadContent();
    }

    /// <summary>
    ///     Se llama en cada frame.
    ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
    ///     ante ellas.
    ///     Aca deberiamos poner toda la logica de actualizacion del juego.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        // Aca deberiamos poner toda la logica de actualizacion del juego.
        var keyboardState = Keyboard.GetState();

        // Capturar inputs del teclado
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            // Si se toco la tecla ESC, se sale del juego.
            Exit();
        }

        // Tiempo transcurrido entre frames
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float safeDeltaTime = Math.Max(deltaTime, 1f / 120f);


        // Actualizar pelota (input)
        Pelota.Update(keyboardState, deltaTime);

        // Avanzar la simulación de física
        Simulation.Timestep(safeDeltaTime, ThreadDispatcher);

        // Actualizar la cámara
        TargetCamera.Update(Pelota.Position);

        base.Update(gameTime);
    }

    /// <summary>
    ///     Llamada para cada frame.
    ///     La logica de dibujo debe ir aca.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        // Se limpia la pantalla
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Se dibuja la pelota
        Pelota.Draw(TargetCamera.View, TargetCamera.Projection);

        // Se dibujan los niveles
        // Nivel1.Draw(GraphicsDevice, TargetCamera.View, TargetCamera.Projection);
        // Nivel2.Draw(GraphicsDevice, TargetCamera.View, TargetCamera.Projection);
        // Nivel3.Draw(GraphicsDevice, TargetCamera.View, TargetCamera.Projection);
        // Nivel4.Draw(GraphicsDevice, TargetCamera.View, TargetCamera.Projection);

        // Dibujar el piso
        Piso.Draw(TargetCamera.View, TargetCamera.Projection);
    }

    /// <summary>
    ///     Libero los recursos que se cargaron en el juego.
    /// </summary>
    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }
}