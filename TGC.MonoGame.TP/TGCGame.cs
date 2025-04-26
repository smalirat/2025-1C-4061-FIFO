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
        private Model SlantLongA { get; set; }
        private Model BumpA { get; set; }
        private Model BumpSolidB { get; set; }
        private Model RampLongA { get; set; }
        private Model RampLongD { get; set; }
        private Model Straight { get; set; }
        private Model SplitDoubleSides { get; set; }
        private Model Tunnel { get; set; }
        private Model CurveLarge { get; set; }
        private Effect Effect { get; set; }
        private float Rotation { get; set; }
        private Matrix World { get; set; }
        private Matrix View { get; set; }
        private Matrix Projection { get; set; }
        private Random Random;
        private const int SEED = 0;
        private FreeCamera FreeCamera { get; set; }

        Vector3 cameraPosition = new Vector3(340, 350, 510);
        Vector3 cameraTarget = new Vector3(240, 300, 360);
        Vector3 cameraUp = Vector3.Up;
        float cameraSpeed = 5f;

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.Zero, screenCenter);

            Matrix VistaNivel1 = Matrix.CreateLookAt(new Vector3(340, 350, 510), new Vector3(240, 300, 360), Vector3.Up);



            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            View = VistaNivel1;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 250);

            base.Initialize();
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el procesamiento
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        ///


        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.
            //SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Cargo los modelos.

            ModelBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            ModelMarble = Content.Load<Model>(ContentFolder3D + "marble/marble_high");
            ModelCurve = Content.Load<Model>(ContentFolder3D + "curves/curve");
            SlantLongA = Content.Load<Model>(ContentFolder3D + "slants/slant_long_A");
            BumpA = Content.Load<Model>(ContentFolder3D + "bump/bump_A");
            BumpSolidB = Content.Load<Model>(ContentFolder3D + "bump/bump_solid_B");
            RampLongA = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_A");
            RampLongD = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_D");
            CurveLarge = Content.Load<Model>(ContentFolder3D + "curves/curve_large");
            Straight = Content.Load<Model>(ContentFolder3D + "straights/straight");
            SplitDoubleSides = Content.Load<Model>(ContentFolder3D + "splits/split_double_sides");
            Tunnel = Content.Load<Model>(ContentFolder3D + "extras/tunnel");
            // preguntar si se pueden declarar en otro archivo? dejarian de ser private

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");


            // Asigno los efectos para cada parte de las mesh.

            TrackLoader.AsignarEfecto(ModelBox, Effect);
            TrackLoader.AsignarEfecto(ModelMarble, Effect);
            TrackLoader.AsignarEfecto(ModelCurve, Effect);
            TrackLoader.AsignarEfecto(SlantLongA, Effect);
            TrackLoader.AsignarEfecto(BumpA, Effect);
            TrackLoader.AsignarEfecto(BumpSolidB, Effect);
            TrackLoader.AsignarEfecto(RampLongA, Effect);
            TrackLoader.AsignarEfecto(RampLongD, Effect);
            TrackLoader.AsignarEfecto(CurveLarge, Effect);
            TrackLoader.AsignarEfecto(Straight, Effect);
            TrackLoader.AsignarEfecto(SplitDoubleSides, Effect);
            TrackLoader.AsignarEfecto(Tunnel, Effect);



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

            //Movimiento Camara

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();
            
            if  (keyboard.IsKeyDown(Keys.W))
                cameraPosition.Z -= cameraSpeed;
            if (keyboard.IsKeyDown(Keys.S))
                cameraPosition.Z += cameraSpeed;
            if (keyboard.IsKeyDown(Keys.A))
                cameraPosition.X -= cameraSpeed;
            if (keyboard.IsKeyDown(Keys.D))
                cameraPosition.X += cameraSpeed;
            if (keyboard.IsKeyDown(Keys.Q))
                cameraPosition.Y += cameraSpeed;
            if (keyboard.IsKeyDown(Keys.E))
                cameraPosition.Y -= cameraSpeed;

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


        private void DrawLevel1()
        {
            // MATRIZ DE OFFSET GLOBAL
            Matrix globalOffset = Matrix.CreateTranslation(100f, 0f, 0f);

            // --- SLANT LONG A #1 ---
            var baseTransformsSlant = new Matrix[SlantLongA.Bones.Count];
            SlantLongA.CopyAbsoluteBoneTransformsTo(baseTransformsSlant);

            foreach (var mesh in SlantLongA.Meshes)
            {
                var relativeTransform = baseTransformsSlant[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 20f) * globalOffset);
                mesh.Draw();
            }

            // --- SLANT LONG A #2 ---
            foreach (var mesh in SlantLongA.Meshes)
            {
                var relativeTransform = baseTransformsSlant[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 50f) * globalOffset);
                mesh.Draw();
            }

            // --- MODEL CURVE ---
            var baseTransformsCurve = new Matrix[ModelCurve.Bones.Count];
            ModelCurve.CopyAbsoluteBoneTransformsTo(baseTransformsCurve);

            foreach (var mesh in ModelCurve.Meshes)
            {
                var relativeTransform = baseTransformsCurve[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Black.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 80f) * globalOffset);
                mesh.Draw();
            }

            // --- BUMP SOLID B ---
            var baseTransformsBump = new Matrix[BumpSolidB.Bones.Count];
            BumpSolidB.CopyAbsoluteBoneTransformsTo(baseTransformsBump);

            foreach (var mesh in BumpSolidB.Meshes)
            {
                var relativeTransform = baseTransformsBump[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Orange.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 110f) * globalOffset);
                mesh.Draw();
            }

            // --- RAMP SHORT A ---
            var baseTransformsRampLongA = new Matrix[RampLongA.Bones.Count];
            RampLongA.CopyAbsoluteBoneTransformsTo(baseTransformsRampLongA);

            foreach (var mesh in RampLongA.Meshes)
            {
                var relativeTransform = baseTransformsRampLongA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 150f) * globalOffset);
                mesh.Draw();
            }


            // --- CURVE LARGE  ---
            var baseTransformsCurveLarge = new Matrix[CurveLarge.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsCurveLarge);

            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 200f) * globalOffset);
                mesh.Draw();
            }
        }

        private void DrawLevel2()
        {
            Matrix globalOffset = Matrix.CreateTranslation(300f, 300f, 300f);

            // --- INICIO

            // --- Straight #1
            var baseTransformsStraight = new Matrix[Straight.Bones.Count];
            Straight.CopyAbsoluteBoneTransformsTo(baseTransformsStraight);

            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 20f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #2
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 30f) * globalOffset);
                mesh.Draw();
            }

            // --- Tunel #1
            var baseTransformsTunnel = new Matrix[Tunnel.Bones.Count];
            Tunnel.CopyAbsoluteBoneTransformsTo(baseTransformsTunnel);

            foreach (var mesh in Tunnel.Meshes)
            {
                var relativeTransform = baseTransformsTunnel[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 5f, 20f) * globalOffset);
                mesh.Draw();
            }

            // --- Tunel #2
            foreach (var mesh in Tunnel.Meshes)
            {
                var relativeTransform = baseTransformsTunnel[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 5f, 30f) * globalOffset);
                mesh.Draw();
            }

            // --- Primera Curva Amplia ---
            // --- Straight #3
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 40f) * globalOffset);
                mesh.Draw();
            }

            // --- Ramp Long D
            var baseTransformsRampLongD = new Matrix[RampLongD.Bones.Count];
            RampLongD.CopyAbsoluteBoneTransformsTo(baseTransformsRampLongD);

            foreach (var mesh in RampLongD.Meshes)
            {
                var relativeTransform = baseTransformsRampLongD[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-60f, -15f, 55f) * globalOffset);
                mesh.Draw();
            }

            // --- CURVE LARGE  ---

            var baseTransformsCurveLarge = new Matrix[CurveLarge.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsCurveLarge);

            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-70f, -15f, 80f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #4
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-90f, -15f, 90f) * globalOffset);
                mesh.Draw();
            }

            //Double Split

            var baseTransformsSplitDoubleSides = new Matrix[SplitDoubleSides.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsSplitDoubleSides);

            foreach (var mesh in SplitDoubleSides.Meshes)
            {
                var relativeTransform = baseTransformsSplitDoubleSides[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2 * -1) * Matrix.CreateTranslation(-105f, -15f, 90f) * globalOffset);
                mesh.Draw();
            }

            // Bump A (derecha)
            var baseTransformsBumpA = new Matrix[BumpA.Bones.Count];
            BumpA.CopyAbsoluteBoneTransformsTo(baseTransformsBumpA);

            foreach (var mesh in BumpA.Meshes)
            {
                var relativeTransform = baseTransformsBumpA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-110f, -15f, 55f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #4
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 30f) * globalOffset);
                mesh.Draw();
            }
            // --- Straight #5
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 20f) * globalOffset);
                mesh.Draw();
            }
            // --- Straight #6
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 10f) * globalOffset);
                mesh.Draw();
            }
            // -- Helix  (izquierda)


            // --- Recta después de la curva ---
            var baseTransformsSlant1 = new Matrix[SlantLongA.Bones.Count];
            SlantLongA.CopyAbsoluteBoneTransformsTo(baseTransformsSlant1);

            foreach (var mesh in SlantLongA.Meshes)
            {
                var relativeTransform = baseTransformsSlant1[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(80f, 0f, 140f) * globalOffset);
                mesh.Draw();
            }

            // --- Bucle Central Grande ---
            var baseTransformsCurveLarge2 = new Matrix[CurveLarge.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsCurveLarge2);

            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge2[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.White.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(150f, 0f, 220f) * globalOffset);
                mesh.Draw();
            }

            // --- Salida del Bucle con Rampa ---
            var baseTransformsRamp2 = new Matrix[RampLongA.Bones.Count];
            RampLongA.CopyAbsoluteBoneTransformsTo(baseTransformsRamp2);

            foreach (var mesh in RampLongA.Meshes)
            {
                var relativeTransform = baseTransformsRamp2[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(220f, -20f, 180f) * globalOffset);
                mesh.Draw();
            }

            // --- Pequeña Curva ---
            var baseTransformsModelCurve = new Matrix[ModelCurve.Bones.Count];
            ModelCurve.CopyAbsoluteBoneTransformsTo(baseTransformsModelCurve);

            foreach (var mesh in ModelCurve.Meshes)
            {
                var relativeTransform = baseTransformsModelCurve[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Black.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(260f, -20f, 120f) * globalOffset);
                mesh.Draw();
            }

            // --- Final - Bache para Caída ---
            var baseTransformsBump = new Matrix[BumpSolidB.Bones.Count];
            BumpSolidB.CopyAbsoluteBoneTransformsTo(baseTransformsBump);

            foreach (var mesh in BumpSolidB.Meshes)
            {
                var relativeTransform = baseTransformsBump[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Orange.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(280f, -40f, 80f) * globalOffset);
                mesh.Draw();
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

            var baseTransforsSlantLongA = new Matrix[SlantLongA.Bones.Count];
            ModelBox.CopyAbsoluteBoneTransformsTo(baseTransforsSlantLongA);

            foreach (var mesh in SlantLongA.Meshes)
            {

                var relativeTransform = baseTransforsSlantLongA[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * World * Matrix.CreateTranslation(-60f, 20f, 0f));
                mesh.Draw();
            }

            DrawLevel1();
            DrawLevel2();



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



