using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
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

        // Managers
        private ModelManager ModelManager;
        private EffectManager EffectManager;

        // Matrices de vista y proyeccion
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private Matrix MarbleWorld { get; set; }

        // Camaras
        private FreeCamera FreeCamera { get; set; }
        private TargetCamera TargetCamera { get; set; }
        private const float CameraFollowRadius = 10f;
        private const float CameraUpDistance = 10f;
        private const float MarbleSpeed = 100f;
        private const float MarbleJumpSpeed = 150f;
        private const float Gravity = 350f;
        private const float MarbleRotatingVelocity = 0.06f;
        private const float Epsilon = 0.00001f;
        private Matrix _marbleScale;
        private Matrix _marbleRotation;
        private Vector3 _marblePosition;
        private Vector3 _marbleVelocity;
        private Vector3 _marbleAcceleration;
        private Vector3 _marbleFrontDirection;
        private bool _onGround;


        // Pelota ("personaje" principal)
        private Pelota pelota { get; set; }

        // Niveles
        private Nivel1 Nivel1;
        private Nivel2 Nivel2;
        private Nivel3 Nivel3;
        private Nivel4 Nivel4;

        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);


            // Carga y encapsula a todos los modelos
            ModelManager = new ModelManager();

            // Carga y encapsula a todos los efectos
            EffectManager = new EffectManager();

            // Niveles
            Nivel1 = new Nivel1(ModelManager, EffectManager);
            Nivel2 = new Nivel2(ModelManager, EffectManager);
            Nivel3 = new Nivel3(ModelManager, EffectManager);
            Nivel4 = new Nivel4(ModelManager, EffectManager);

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
            // var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            //FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 0, 300), screenCenter);


            // Configuramos las camara
            _onGround = true;
            _marbleFrontDirection = Vector3.Forward; //ojo esto
            _marbleVelocity = Vector3.Zero;
            _marbleRotation = Matrix.Identity;
            MarbleWorld = Matrix.Identity;

            // Create the Target Camera
            TargetCamera = new TargetCamera(GraphicsDevice.Viewport.AspectRatio);

            // Set the Acceleration (which in this case won't change) to the Gravity pointing down
            _marbleAcceleration = Vector3.Down * Gravity;

            // Initialize the Velocity as zero
            _marbleVelocity = Vector3.Zero;



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

            // Capturar Input teclado
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }

            //Movimiento Camara


            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 movement = Vector3.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) movement.Z -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) movement.Z += 1;
            if (keyboardState.IsKeyDown(Keys.A)) movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) movement.X += 1;

            if (movement != Vector3.Zero)
            {
                movement.Normalize();
                _marblePosition += movement * MarbleSpeed * deltaTime;
            }

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            World = Matrix.CreateRotationY(Rotation);

            MarbleWorld = _marbleRotation * Matrix.CreateTranslation(_marblePosition);

            // Actualizar la cámara
            TargetCamera.Update(gameTime, MarbleWorld);
            //FreeCamera.Update(gameTime);
            View = TargetCamera.View;
            Projection = TargetCamera.Projection;



            base.Update(gameTime);
        }

        /// <summary>
        ///     Llamada para cada frame.
        ///     La logica de dibujo debe ir aca.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Nivel1.Draw(GraphicsDevice, View, Projection);
            Nivel2.Draw(GraphicsDevice, View, Projection);
            Nivel3.Draw(GraphicsDevice, View, Projection);
            Nivel4.Draw(GraphicsDevice, View, Projection);
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