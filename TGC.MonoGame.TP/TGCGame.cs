using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;

            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        private GraphicsDeviceManager Graphics { get; }
        private SpriteBatch SpriteBatch { get; set; }
        private Model ModelBox { get; set; }
        private Model ModelMarble { get; set; }
        private Model ModelCurve { get; set; }
        private Effect Effect { get; set; }
        private float Rotation { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private Random Random;
        private const int SEED = 0;
        private FreeCamera FreeCamera { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.Zero, screenCenter);


            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(Vector3.UnitZ * 200, Vector3.Zero, Vector3.Up);
            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            //SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargo el modelo del logo.
            ModelBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            ModelMarble = Content.Load<Model>(ContentFolder3D + "marble/marble_high");
            ModelCurve = Content.Load<Model>(ContentFolder3D + "curves/curve");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Asigno los efectos para cada parte de las mesh.

            foreach (var mesh in ModelBox.Meshes)
            {
                // recordar que un mesh puede tener mas de 1 mesh part (cada 1 puede tener su propio efecto).
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in ModelMarble.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }

            foreach (var mesh in ModelCurve.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = Effect;
                }
            }


            base.LoadContent();
        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la logica de computo del modelo, asi como tambien verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Aca deberiamos poner toda la logica de actualizacion del juego.

            // Capturar Input teclado
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //Salgo del juego.
                Exit();
            }

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            World = Matrix.CreateRotationY(Rotation);

            FreeCamera.Update(gameTime);
            View = FreeCamera.View;
            Projection = FreeCamera.Projection;

            base.Update(gameTime);
        }

        private Color RandomColor(Random random)
        {
            // Construye un color aleatorio en base a un entero de 32 bits
            return new Color((uint)random.Next());
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        ///

        private void DrawModelBoxes(Model model, Matrix[] baseTransforms, int rows, int cols, float spacing) // Revisar como se distribuyen las columnas y filas
        {
            // esto sirve para acomodar en que parte del espacio quiero que se dibujen los modelos
            var offset = new Vector3(-150f, 60f, 0);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Posición ordenada en filas
                    var position = new Vector3(col * spacing, row * spacing, 0) + offset;
                    var worldMatrix = Matrix.CreateTranslation(position);

                    // Color aleatorio
                    Effect.Parameters["DiffuseColor"].SetValue(RandomColor(Random).ToVector3());

                    foreach (var mesh in model.Meshes)
                    {
                        var relativeTransform = baseTransforms[mesh.ParentBone.Index];
                        Effect.Parameters["World"].SetValue(relativeTransform * worldMatrix);
                        mesh.Draw();
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

            //Dibujo de la canica

            var baseTransforsMarble = new Matrix[ModelMarble.Bones.Count];
            ModelBox.CopyAbsoluteBoneTransformsTo(baseTransforsMarble);

            foreach (var mesh in ModelMarble.Meshes)
            {

                var relativeTransform = baseTransforsMarble[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * World * Matrix.CreateTranslation(-60f, 70f, 0f));
                mesh.Draw();
            }




            //Dibujo de las cajas
            Random = new Random(SEED);

            var baseTransforsBox = new Matrix[ModelBox.Bones.Count];
            ModelBox.CopyAbsoluteBoneTransformsTo(baseTransforsBox);

            DrawModelBoxes(ModelBox, baseTransforsBox, 2, 5, 20f);


            //Dibujo una curva

            var baseTransforsCurve = new Matrix[ModelCurve.Bones.Count];
            ModelBox.CopyAbsoluteBoneTransformsTo(baseTransforsCurve);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Black.ToVector3());

            foreach (var mesh in ModelCurve.Meshes)
            {

                var relativeTransform = baseTransforsMarble[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * World * Matrix.CreateTranslation(-60f, 40f, 0f));
                mesh.Draw();
            }


        }

        /// <summary>
        ///     Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }
    }
}

