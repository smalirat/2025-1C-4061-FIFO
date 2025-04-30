using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Escenarios;
using TGC.MonoGame.TP.Modelos;

namespace TGC.MonoGame.TP
{
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

        private ModelManager ModelManager;

        // Matrices de vista y proyeccion
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }

        // Camaras
        private FreeCamera FreeCamera { get; set; }

        // Pelota ("personaje" principal)
        private Pelota pelota { get; set; }

        // Escenario (pistas por donde se mueve la pelota)
        private EscenarioManager EscenarioManager;

        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);

            // Maneja el load y encapsula a todos los modelos
            ModelManager = new ModelManager();

            // Maneja al escenario principal (pistas por donde va la pelotita)
            EscenarioManager = new EscenarioManager(ModelManager);

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

            // Configuramos nuestras matrices de la escena.
            View = Matrix.CreateLookAt(Vector3.UnitZ * 1000, Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView: MathHelper.PiOver4, // FOV
                aspectRatio: GraphicsDevice.Viewport.AspectRatio,
                nearPlaneDistance: 1,
                farPlaneDistance: 250);

            // Configuramos la camara
            var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 0, 300), screenCenter);

            base.Initialize();
        }

        /// <summary>
        ///     Llamada una sola vez durante la inicializacion de la aplicacion, luego de Initialize, y una vez que fue configurado GraphicsDevice.
        ///     Debe ser usada para cargar los recursos y otros elementos del contenido.
        ///     Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
        /// </summary>
        protected override void LoadContent()
        {
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
            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }

            // Matrices
            View = FreeCamera.View;
            Projection = FreeCamera.Projection;

            // Actualizo posicion de camara segun inputs (teclado / mouse)
            FreeCamera.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        ///     Llamada para cada frame.
        ///     La logica de dibujo debe ir aca.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            EscenarioManager.DrawAllLevels(GraphicsDevice, View, Projection);
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
}