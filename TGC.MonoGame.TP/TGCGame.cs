using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Content.Models;

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

        private const int SEED = 0;

        private Random Random;

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

        private Model BannerHigh { get; set; }
        private Model Bend { get; set; }
        private Model BumpA { get; set; }
        private Model BumpD { get; set; }
        private Model BumpSolidB { get; set; }
        private Model CurveLarge { get; set; }
        private Effect Effect { get; set; }
        private Model EndSquare { get; set; }
        private EscenaRecta escenaRecta { get; set; }
        private FreeCamera FreeCamera { get; set; }
        private Model Funnel { get; set; }
        private Model FunnelLong { get; set; }
        private GraphicsDeviceManager Graphics { get; }

        private Model HelixHalfLeft { get; set; }

        private Model HelixHalfRight { get; set; }

        private Model HelixLargeHalfLeft { get; set; }

        private Model HelixLargeHalfRight { get; set; }

        private Model HelixLargeLeft { get; set; }

        private Model HelixLargeQuarterLeft { get; set; }

        private Model HelixLargeQuarterRight { get; set; }

        private Model HelixLargeRight { get; set; }

        private Model HelixLeft { get; set; }

        private Model HelixRight { get; set; }

        private Model ModelBox { get; set; }

        private Model ModelCurve { get; set; }

        private Model NormalTree { get; set; }

        // Modelos
        private Pelota pelotita { get; set; }

        private Matrix Projection { get; set; }
        private Model RampLongA { get; set; }
        private Model RampLongB { get; set; }
        private Model RampLongC { get; set; }
        private Model RampLongD { get; set; }
        private float Rotation { get; set; }
        private Model SlantLongA { get; set; }
        private Model SlantLongC { get; set; }
        private Model SlantLongD { get; set; }
        private Model Split { get; set; }
        private Model SplitDouble { get; set; }
        private Model SplitDoubleSides { get; set; }
        private Model SplitLeft { get; set; }
        private Model SplitRight { get; set; }
        private Model Straight { get; set; }
        private Model Support { get; set; }
        private Model TallTree { get; set; }
        private Model Tunnel { get; set; }
        private Matrix View { get; set; }
        private Model WallHalf { get; set; }
        private Model WaveA { get; set; }
        private Model WaveB { get; set; }
        private Model WaveC { get; set; }
        private Matrix World { get; set; }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Effect.Parameters["View"].SetValue(View);
            Effect.Parameters["Projection"].SetValue(Projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

            //Dibujo de la canica/pelotita
            pelotita.Draw(World, View, Projection);

            Random = new Random(SEED);

            //Dibujo el escenario
            DrawLevel1();
            DrawLevel2();
            //DrawLevel3();
            DrawLevel4();
            DrawLevel5();
            DrawLevel6();

            // OTROS
            //DrawLevel0();
            //DrawModelBoxes(ModelBox, baseTransforsBox, 2, 5, 20f);
            //Dibujo de las cajas
            //var baseTransforsBox = new Matrix[ModelBox.Bones.Count];
            //ModelBox.CopyAbsoluteBoneTransformsTo(baseTransforsBox);
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            FreeCamera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, new Vector3(0, 0, 300), screenCenter);

            Matrix VistaNivel1 = Matrix.CreateLookAt(new Vector3(340, 350, 510), new Vector3(0, 0, 0), Vector3.Up);

            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
            View = VistaNivel1;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 10, 2500000);

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

            // Cargo los modelos.
            pelotita = new Pelota(Content);
            escenaRecta = new EscenaRecta(Content);
            ModelBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            ModelCurve = Content.Load<Model>(ContentFolder3D + "curves/curve");
            SlantLongA = Content.Load<Model>(ContentFolder3D + "slants/slant_long_A");
            SlantLongC = Content.Load<Model>(ContentFolder3D + "slants/slant_long_C");
            BumpA = Content.Load<Model>(ContentFolder3D + "bump/bump_A");
            BumpSolidB = Content.Load<Model>(ContentFolder3D + "bump/bump_solid_B");
            RampLongA = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_A");
            RampLongB = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_B");
            RampLongC = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_C");
            RampLongD = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_D");
            CurveLarge = Content.Load<Model>(ContentFolder3D + "curves/curve_large");
            Straight = Content.Load<Model>(ContentFolder3D + "straights/straight");
            Split = Content.Load<Model>(ContentFolder3D + "splits/split");
            SplitLeft = Content.Load<Model>(ContentFolder3D + "splits/split_left");
            SplitRight = Content.Load<Model>(ContentFolder3D + "splits/split_right");
            SplitDouble = Content.Load<Model>(ContentFolder3D + "splits/split_double");
            SplitDoubleSides = Content.Load<Model>(ContentFolder3D + "splits/split_double_sides");
            Tunnel = Content.Load<Model>(ContentFolder3D + "extras/tunnel");
            HelixLeft = Content.Load<Model>(ContentFolder3D + "helixs/helix_left");
            HelixRight = Content.Load<Model>(ContentFolder3D + "helixs/helix_right");
            HelixHalfLeft = Content.Load<Model>(ContentFolder3D + "helixs/helix_half_left");
            HelixHalfRight = Content.Load<Model>(ContentFolder3D + "helixs/helix_half_right");
            HelixLargeHalfLeft = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_half_left");
            HelixLargeHalfRight = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_half_right");
            HelixLargeLeft = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_left");
            HelixLargeRight = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_right");
            HelixLargeQuarterLeft = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_quarter_left");
            HelixLargeQuarterRight = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_quarter_right");
            WaveA = Content.Load<Model>(ContentFolder3D + "waves/wave_A");
            WaveB = Content.Load<Model>(ContentFolder3D + "waves/wave_B");
            WaveC = Content.Load<Model>(ContentFolder3D + "waves/wave_C");
            Funnel = Content.Load<Model>(ContentFolder3D + "funnels/funnel");
            FunnelLong = Content.Load<Model>(ContentFolder3D + "funnels/funnel_long");
            WallHalf = Content.Load<Model>(ContentFolder3D + "extras/wallHalf");
            NormalTree = Content.Load<Model>(ContentFolder3D + "tree/tree");
            TallTree = Content.Load<Model>(ContentFolder3D + "tree/tree_tall");
            SlantLongD = Content.Load<Model>(ContentFolder3D + "slants/slan_long_D");
            EndSquare = Content.Load<Model>(ContentFolder3D + "endHoles/end_square");
            Support = Content.Load<Model>(ContentFolder3D + "supports/support_base");
            BumpD = Content.Load<Model>(ContentFolder3D + "bump/bump_D");
            BannerHigh = Content.Load<Model>(ContentFolder3D + "banners/banner_high");
            Bend = Content.Load<Model>(ContentFolder3D + "bend/bend_medium");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Asigno los efectos para cada parte de las mesh.
            TrackLoader.AsignarEfecto(ModelBox, Effect);
            TrackLoader.AsignarEfecto(ModelCurve, Effect);
            TrackLoader.AsignarEfecto(SlantLongA, Effect);
            TrackLoader.AsignarEfecto(SlantLongC, Effect);
            TrackLoader.AsignarEfecto(BumpA, Effect);
            TrackLoader.AsignarEfecto(BumpSolidB, Effect);
            TrackLoader.AsignarEfecto(RampLongA, Effect);
            TrackLoader.AsignarEfecto(RampLongB, Effect);
            TrackLoader.AsignarEfecto(RampLongC, Effect);
            TrackLoader.AsignarEfecto(RampLongD, Effect);
            TrackLoader.AsignarEfecto(CurveLarge, Effect);
            TrackLoader.AsignarEfecto(Straight, Effect);
            TrackLoader.AsignarEfecto(Split, Effect);
            TrackLoader.AsignarEfecto(SplitLeft, Effect);
            TrackLoader.AsignarEfecto(SplitRight, Effect);
            TrackLoader.AsignarEfecto(SplitDouble, Effect);
            TrackLoader.AsignarEfecto(SplitDoubleSides, Effect);
            TrackLoader.AsignarEfecto(Tunnel, Effect);
            TrackLoader.AsignarEfecto(HelixLeft, Effect);
            TrackLoader.AsignarEfecto(HelixRight, Effect);
            TrackLoader.AsignarEfecto(HelixHalfLeft, Effect);
            TrackLoader.AsignarEfecto(HelixHalfRight, Effect);
            TrackLoader.AsignarEfecto(HelixLargeHalfLeft, Effect);
            TrackLoader.AsignarEfecto(HelixLargeHalfRight, Effect);
            TrackLoader.AsignarEfecto(HelixLargeLeft, Effect);
            TrackLoader.AsignarEfecto(HelixLargeRight, Effect);
            TrackLoader.AsignarEfecto(HelixLargeQuarterLeft, Effect);
            TrackLoader.AsignarEfecto(HelixLargeQuarterRight, Effect);
            TrackLoader.AsignarEfecto(WaveA, Effect);
            TrackLoader.AsignarEfecto(WaveB, Effect);
            TrackLoader.AsignarEfecto(WaveC, Effect);
            TrackLoader.AsignarEfecto(Funnel, Effect);
            TrackLoader.AsignarEfecto(FunnelLong, Effect);
            TrackLoader.AsignarEfecto(WallHalf, Effect);
            TrackLoader.AsignarEfecto(NormalTree, Effect);
            TrackLoader.AsignarEfecto(TallTree, Effect);
            TrackLoader.AsignarEfecto(SlantLongD, Effect);
            TrackLoader.AsignarEfecto(EndSquare, Effect);
            TrackLoader.AsignarEfecto(Support, Effect);
            TrackLoader.AsignarEfecto(BumpD, Effect);
            TrackLoader.AsignarEfecto(BannerHigh, Effect);
            TrackLoader.AsignarEfecto(Bend, Effect);

            base.LoadContent();
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

            pelotita.Update(gameTime, keyboard);//movimientos ikjl

            // Basado en el tiempo que paso se va generando una rotacion.
            Rotation += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            World = Matrix.CreateRotationY(Rotation);

            FreeCamera.Update(gameTime);
            View = FreeCamera.View;
            Projection = FreeCamera.Projection;

            base.Update(gameTime);
        }

        private void DrawLevel1()
        {
            escenaRecta.Draw(World, View, Projection, new Vector3(0, -20, 0));
            escenaRecta.Draw(World, View, Projection, new Vector3(0, -16, -80));
            escenaRecta.Draw(World, View, Projection, new Vector3(0, -12, -160));
        }

        private void DrawLevel2()
        {
            float zBasePosition = 0f;
            float yBasePosition = 0f;
            Matrix globalOffset = Matrix.CreateTranslation(10f, -160f, -500f);

            Color[] colors = new Color[]{
                Color.Peru,
                Color.Wheat
            };

            int colorIndex = 0;

            for (int i = 0; i < 30; i++)
            {
                DrawModel(Funnel, true, 0f, yBasePosition, zBasePosition, globalOffset, colors[colorIndex]);
                zBasePosition += 10f;
                yBasePosition += 5f;

                colorIndex++;

                if (colorIndex > 1)
                    colorIndex = 0;
            }
        }


        private void DrawLevel4()
        {
            // MATRIZ DE OFFSET GLOBAL
            Matrix globalOffset = Matrix.CreateTranslation(0f, -270f, -600f);

            // --- Straight #1 ---
            var baseTransformsStraight = new Matrix[Straight.Bones.Count];
            Straight.CopyAbsoluteBoneTransformsTo(baseTransformsStraight);

            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(0f, 0f, 0f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #1 ---
            var baseTransformsCurveLarge = new Matrix[CurveLarge.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsCurveLarge);

            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(10f, 0f, 20f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #2 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(30f, 0f, 30f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveA #1 ---
            var baseTransformsWaveA = new Matrix[WaveA.Bones.Count];
            WaveA.CopyAbsoluteBoneTransformsTo(baseTransformsWaveA);

            foreach (var mesh in WaveA.Meshes)
            {
                var relativeTransform = baseTransformsWaveA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(55f, 0f, 28.75f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #3 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(80f, 0f, 30f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #2 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(100f, 0f, 20f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveB #1 ---
            var baseTransformsWaveB = new Matrix[WaveB.Bones.Count];
            WaveB.CopyAbsoluteBoneTransformsTo(baseTransformsWaveB);

            foreach (var mesh in WaveB.Meshes)
            {
                var relativeTransform = baseTransformsWaveB[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(112.5f, 0f, -15f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #4 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(110f, 0f, -40f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveC #1 ---
            var baseTransformsWaveC = new Matrix[WaveC.Bones.Count];
            WaveC.CopyAbsoluteBoneTransformsTo(baseTransformsWaveC);

            foreach (var mesh in WaveC.Meshes)
            {
                var relativeTransform = baseTransformsWaveC[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(115f, 0f, -65f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #5 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(110f, 0f, -90f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #3 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(120f, 0f, -110f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #6 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(140f, 0f, -120f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveA #2 ---
            foreach (var mesh in WaveA.Meshes)
            {
                var relativeTransform = baseTransformsWaveA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(165f, 0f, -121.25f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #7 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(190f, 0f, -120f) * globalOffset);
                mesh.Draw();
            }

            // --- SplitDoubleSides #1---
            var baseTransformsSplitDoubleSides = new Matrix[SplitDoubleSides.Bones.Count];
            SplitDoubleSides.CopyAbsoluteBoneTransformsTo(baseTransformsSplitDoubleSides);

            foreach (var mesh in SplitDoubleSides.Meshes)
            {
                var relativeTransform = baseTransformsSplitDoubleSides[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Blue.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(205f, 0f, -120f) * globalOffset);
                mesh.Draw();
            }

            // #### CAMINO A (IZQUIERDA) ####
            // --- Straight #8 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(210f, 0f, -140f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #4 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(200f, 0f, -160f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #9 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(180f, 0f, -170f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #10 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(170f, 0f, -170f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveA #3 ---
            foreach (var mesh in WaveA.Meshes)
            {
                var relativeTransform = baseTransformsWaveA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(145f, 0f, -171.25f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #11 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(120f, 0f, -170f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveA #4 ---
            foreach (var mesh in WaveA.Meshes)
            {
                var relativeTransform = baseTransformsWaveA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(3 * MathHelper.PiOver2) * Matrix.CreateTranslation(95f, 0f, -168.75f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #12 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(70f, 0f, -170f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #13 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(60f, 0f, -170f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #5 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(40f, 0f, -160f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #14 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(30f, 0f, -140f) * globalOffset);
                mesh.Draw();
            }

            // #### CAMINO B (DERECHA) ####
            // --- Straight #15 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(210f, 0f, -100f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #6 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(200f, 0f, -80f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #16 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(180f, 0f, -70f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveB #2 ---
            foreach (var mesh in WaveB.Meshes)
            {
                var relativeTransform = baseTransformsWaveB[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(3 * MathHelper.PiOver2) * Matrix.CreateTranslation(155f, 0f, -67.5f) * globalOffset);
                mesh.Draw();
            }

            // --- WaveC #2 ---
            foreach (var mesh in WaveC.Meshes)
            {
                var relativeTransform = baseTransformsWaveC[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(85f, 0f, -75f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #17 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(60f, 0f, -70f) * globalOffset);
                mesh.Draw();
            }

            // --- CurveLarge #7 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(40f, 0f, -80f) * globalOffset);
                mesh.Draw();
            }

            // --- Straight #18 ---
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Green.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(30f, 0f, -100f) * globalOffset);
                mesh.Draw();
            }

            // #### UNION DE CAMINOS ####
            // --- SplitDoubleSides #2 ---
            foreach (var mesh in SplitDoubleSides.Meshes)
            {
                var relativeTransform = baseTransformsSplitDoubleSides[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Yellow.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(25f, 0f, -120f) * globalOffset);
                mesh.Draw();
            }
            // --- CurveLarge #8 ---
            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(-MathHelper.PiOver2) * Matrix.CreateTranslation(40f, 0f, -80f) * globalOffset);
                mesh.Draw();
            }
        }

        private void DrawLevel5()
        {
            Matrix globalOffset = Matrix.CreateTranslation(60f, -270f, 600f);
            Matrix globalRotation = Matrix.CreateRotationY(MathHelper.Pi);

            // --- INICIO

            // --- Straight #1
            var baseTransformsStraight = new Matrix[Straight.Bones.Count];
            Straight.CopyAbsoluteBoneTransformsTo(baseTransformsStraight);

            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 20f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #2
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 30f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Tunel #1
            var baseTransformsTunnel = new Matrix[Tunnel.Bones.Count];
            Tunnel.CopyAbsoluteBoneTransformsTo(baseTransformsTunnel);

            foreach (var mesh in Tunnel.Meshes)
            {
                var relativeTransform = baseTransformsTunnel[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 5f, 20f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Tunel #2
            foreach (var mesh in Tunnel.Meshes)
            {
                var relativeTransform = baseTransformsTunnel[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 5f, 30f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #3
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-60f, 0f, 40f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Ramp Long D
            var baseTransformsRampLongD = new Matrix[RampLongD.Bones.Count];
            RampLongD.CopyAbsoluteBoneTransformsTo(baseTransformsRampLongD);

            foreach (var mesh in RampLongD.Meshes)
            {
                var relativeTransform = baseTransformsRampLongD[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-60f, -15f, 55f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- CURVE LARGE  ---

            var baseTransformsCurveLarge = new Matrix[CurveLarge.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsCurveLarge);

            foreach (var mesh in CurveLarge.Meshes)
            {
                var relativeTransform = baseTransformsCurveLarge[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-70f, -15f, 80f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #4
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-90f, -15f, 90f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            //Double Split Sides

            var baseTransformsSplitDoubleSides = new Matrix[SplitDoubleSides.Bones.Count];
            CurveLarge.CopyAbsoluteBoneTransformsTo(baseTransformsSplitDoubleSides);

            foreach (var mesh in SplitDoubleSides.Meshes)
            {
                var relativeTransform = baseTransformsSplitDoubleSides[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2 * -1) * Matrix.CreateTranslation(-105f, -15f, 90f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // Bump A (derecha)
            var baseTransformsBumpA = new Matrix[BumpA.Bones.Count];
            BumpA.CopyAbsoluteBoneTransformsTo(baseTransformsBumpA);

            foreach (var mesh in BumpA.Meshes)
            {
                var relativeTransform = baseTransformsBumpA[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-110f, -15f, 55f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #4
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 30f) * globalOffset * globalRotation);
                mesh.Draw();
            }
            // --- Straight #5
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 20f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #6
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -15f, 10f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Helix Large Right #1
            var baseTransformsHelixLargeRight = new Matrix[HelixLargeRight.Bones.Count];
            HelixLargeRight.CopyAbsoluteBoneTransformsTo(baseTransformsHelixLargeRight);

            foreach (var mesh in HelixLargeRight.Meshes)
            {
                var relativeTransform = baseTransformsHelixLargeRight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-135f, -35f, 5f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Helix Large Right #2
            foreach (var mesh in HelixLargeRight.Meshes)
            {
                var relativeTransform = baseTransformsHelixLargeRight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-135f, -55f, 5f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- Straight #6
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -80f, -30f) * globalOffset * globalRotation);
                mesh.Draw();
            }
            // --- Straight #7
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -80f, -40f) * globalOffset * globalRotation);
                mesh.Draw();
            }
            // --- Straight #8
            foreach (var mesh in Straight.Meshes)
            {
                var relativeTransform = baseTransformsStraight[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-110f, -80f, -50f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            // --- SLANT LONG C  ---
            var baseTransformsSlantLongC = new Matrix[SlantLongC.Bones.Count];
            SlantLongC.CopyAbsoluteBoneTransformsTo(baseTransformsSlantLongC);

            foreach (var mesh in SlantLongC.Meshes)
            {
                var relativeTransform = baseTransformsSlantLongC[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-110f, -90f, -65f) * globalOffset * globalRotation);
                mesh.Draw();
            }
            // --- SLANT LONG C #2  ---

            foreach (var mesh in SlantLongC.Meshes)
            {
                var relativeTransform = baseTransformsSlantLongC[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-110f, -95f, -75f) * globalOffset * globalRotation);
                mesh.Draw();
            }
            foreach (var mesh in SlantLongC.Meshes)
            {
                var relativeTransform = baseTransformsSlantLongC[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(Color.Purple.ToVector3());
                Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(-110f, -100f, -85f) * globalOffset * globalRotation);
                mesh.Draw();
            }

            //Tunnel
        }

        private void DrawLevel6()
        {
            // Posicion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(-50f, -900f, 140f);
            Matrix globalRotation = Matrix.CreateRotationY(MathHelper.Pi);

            // Dibujamos rampas rectas con caida hacia abajo
            var rampDownScale = Matrix.CreateScale(xScale: 1, yScale: 1.5f, zScale: 2);
            var rampDownColor = Color.Yellow;
            var rampDownPositions = new List<Vector3>
            {
                new Vector3(0, 460, 300),
                new Vector3(0, 410, 270),
                new Vector3(0, 360, 230),
                new Vector3(0, 310, 200),
                new Vector3(0, 260, 170),
                new Vector3(0, 210, 130),
                new Vector3(0, 160, 100),
                new Vector3(0, 110, 70),
                new Vector3(0, 60, 40),
                new Vector3(0, 10, 10)
            };
            var rampDownRotation = Matrix.CreateRotationX(0);

            foreach (var rampDownPosition in rampDownPositions)
            {
                var translation = Matrix.CreateTranslation(rampDownPosition);
                DrawLevel6Element(rampDownScale, translation, rampDownRotation, SlantLongD, rampDownColor);
            }

            // Dibujamos los "pisos"
            var floorScale = Matrix.CreateScale(xScale: 50f, yScale: 1f, zScale: 500f);
            var floorColor = Color.SandyBrown;
            var floorPositions = new List<Vector3>
            {
                new Vector3(0, -40, -200)
            };
            var floorRotation = Matrix.CreateRotationX(0);

            foreach (var floorPosition in floorPositions)
            {
                var translation = Matrix.CreateTranslation(floorPosition);
                DrawLevel6Element(floorScale, translation, floorRotation, ModelBox, floorColor);
            }

            // Dibujamos las rectas
            var straightScale = Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 6f);
            var straightColor = Color.Violet;
            var straightPositions = new List<Vector3>
            {
                new Vector3(0, -40, -30),
                new Vector3(0, -40, -90),
                new Vector3(-10, -40, -350),
                new Vector3(10, -40, -350),
                new Vector3(0, -40, -450),
                new Vector3(0, -40, -510),
                new Vector3(0, -40, -570),
                new Vector3(0, -40, -630),
            };
            var straightRotation = Matrix.CreateRotationX(0);

            foreach (var straightPosition in straightPositions)
            {
                var translation = Matrix.CreateTranslation(straightPosition);
                DrawLevel6Element(straightScale, translation, straightRotation, Straight, straightColor);
            }

            // Dibujamos las divisiones
            var splitScale = Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f);
            var splitColor = Color.Red;
            var splitPositions = new List<Vector3>
            {
                new Vector3(0, -40, 140),
            };
            var splitRotation = Matrix.CreateRotationY(MathHelper.Pi);

            foreach (var splitPosition in splitPositions)
            {
                var translation = Matrix.CreateTranslation(splitPosition);
                DrawLevel6Element(splitScale, translation, splitRotation, SplitDouble, splitColor);
            }

            // Dibujamos las conjunciones
            var conjuctionScale = Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f);
            var conjuctionColor = Color.Red;
            var conjuctionPositions = new List<Vector3>
            {
                new Vector3(0, -40, -400),
            };
            var conjuctionRotation = Matrix.CreateRotationY(0);

            foreach (var conjuctionPosition in conjuctionPositions)
            {
                var translation = Matrix.CreateTranslation(conjuctionPosition);
                DrawLevel6Element(conjuctionScale, translation, conjuctionRotation, SplitDouble, conjuctionColor);
            }

            // Dibujamos las rampas subi baja
            var upDownRampScale = Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f);
            var upDownRampColor = Color.Black;
            var upDownRampPositions = new List<Vector3>
            {
                new Vector3(10, -40, -180),
                new Vector3(10, -40, -220),
                new Vector3(10, -40, -260),
                new Vector3(10, -40, -300),
            };
            var upDownRampRotation = Matrix.CreateRotationX(0);

            foreach (var upDownRampPosition in upDownRampPositions)
            {
                var translation = Matrix.CreateTranslation(upDownRampPosition);
                DrawLevel6Element(upDownRampScale, translation, upDownRampRotation, BumpD, upDownRampColor);
            }

            // Dibujamos las rampas subi baja leves
            var slowUpDownRampScale = Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f);
            var slowUpDownRampColor = Color.White;
            var slowUpDownRampPositions = new List<Vector3>
            {
                new Vector3(-10, -40, -180),
                new Vector3(-10, -40, -220),
                new Vector3(-10, -40, -260),
                new Vector3(-10, -40, -300),
            };
            var slowUpDownRampRotation = Matrix.CreateRotationX(0);

            foreach (var slowUpDownRampPosition in slowUpDownRampPositions)
            {
                var translation = Matrix.CreateTranslation(slowUpDownRampPosition);
                DrawLevel6Element(slowUpDownRampScale, translation, slowUpDownRampRotation, BumpA, slowUpDownRampColor);
            }

            // Dibujamos los tuneles
            var tunnelColor = Color.Teal;
            var tunnelScale = Matrix.CreateScale(xScale: 3f, yScale: 5f, zScale: 16f);
            var tunnelPositions = new List<Vector3>
            {
                new Vector3(0, -40, -530),
            };
            var tunnelRotation = Matrix.CreateRotationX(0);

            foreach (var tunnelPosition in tunnelPositions)
            {
                var translation = Matrix.CreateTranslation(tunnelPosition);
                DrawLevel6Element(tunnelScale, translation, tunnelRotation, Tunnel, tunnelColor);
            }

            // Dibujamos las banderas
            var bannerColor = Color.DarkBlue;
            var bannerScale = Matrix.CreateScale(xScale: 10f, yScale: 7f, zScale: 1f);
            var bannerPositions = new List<Vector3>
            {
                new Vector3(0, -40, -200),
                new Vector3(0, -40, -100),
                new Vector3(0, -40, -300),
                new Vector3(0, -40, -400),
                new Vector3(0, -40, -680),
            };
            var bannerRotation = Matrix.CreateRotationX(0);

            foreach (var bannerPosition in bannerPositions)
            {
                var translation = Matrix.CreateTranslation(bannerPosition);
                DrawLevel6Element(bannerScale, translation, bannerRotation, BannerHigh, bannerColor);
            }

            // Dibujamos el final
            DrawLevel6Element(Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f),
                Matrix.CreateTranslation(new Vector3(0, -40, 665)),
                Matrix.CreateRotationY(MathHelper.Pi),
                EndSquare,
                Color.Red);

            // Dibujamos los arboles
            var treePossibleColors = new Color[]
            {
                Color.Green,
                Color.GreenYellow,
                Color.DarkGreen,
                Color.DarkOliveGreen,
                Color.DarkSeaGreen
            };

            var normalTreeScale = Matrix.CreateScale(xScale: 2f, yScale: 2f, zScale: 2f);
            var normalTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40, -40, -560),
                new Vector3(40, -40, 220),
                new Vector3(40, -40, -320),
                new Vector3(40, -40, -80),
                new Vector3(40, -40, 280),
                new Vector3(40, -40, -440),
                new Vector3(40, -40, -20),

                // Izquierda
                new Vector3(-40f, -40f, -560f),
                new Vector3(-40f, -40f, 220f),
                new Vector3(-40f, -40f, -320f),
                new Vector3(-40f, -40f, -80f),
                new Vector3(-40f, -40f, 280f),
                new Vector3(-40f, -40f, -440f),
                new Vector3(-40f, -40f, -20f),
                new Vector3(-40f, -40f, -180f),

                // Parte sin rampas (random)
                new Vector3(-14.6f, -40f, 125.7f),
                new Vector3(37.4f, -40f, 185.8f),
                new Vector3(-9.3f, -40f, 240.9f),
                new Vector3(21.1f, -40f, 155.3f),
                new Vector3(-3.7f, -40f, 45.6f)
            };
            var normalTreeRotation = Matrix.CreateRotationX(0);

            foreach (var treePosition in normalTreePositions)
            {
                var treeColor = treePossibleColors[Random.Next(treePossibleColors.Length)];
                var translation = Matrix.CreateTranslation(treePosition);
                DrawLevel6Element(normalTreeScale, translation, normalTreeRotation, NormalTree, treeColor);
            }

            var tallTreeScale = Matrix.CreateScale(xScale: 2f, yScale: 2f, zScale: 2f);
            var tallTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40, -40, -260),
                new Vector3(40, -40, -500),
                new Vector3(40, -40, 100),
                new Vector3(40, -40, -620),
                new Vector3(40, -40, -140),
                new Vector3(40, -40, -380),
                new Vector3(40, -40, 160),
                new Vector3(40, -40, 40),
                new Vector3(40, -40, -670),

                // Izquierda
                new Vector3(-40f, -40f, -260f),
                new Vector3(-40f, -40f, -500f),
                new Vector3(-40f, -40f, 100f),
                new Vector3(-40f, -40f, -620f),
                new Vector3(-40f, -40f, -140f),
                new Vector3(-40f, -40f, -380f),
                new Vector3(-40f, -40f, 160f),
                new Vector3(-40f, -40f, 40f),

                // Parte sin rampas (random)
                new Vector3(-12.8f, -40f, 56.3f),
                new Vector3(24.9f, -40f, 95.2f),
                new Vector3(8.1f, -40f, 145.6f),
                new Vector3(-35.4f, -40f, 120.4f),
                new Vector3(29.6f, -40f, 200.8f),
                new Vector3(16.3f, -40f, 215.5f),
                new Vector3(33.7f, -40f, 170.2f),
                new Vector3(-27.2f, -40f, 205.3f),
                new Vector3(3.0f, -40f, 160.1f),
                new Vector3(12.5f, -40f, 230.4f),
                new Vector3(-5.9f, -40f, 100.7f),
                new Vector3(20.2f, -40f, 130.6f),
                new Vector3(-31.3f, -40f, 190.2f),
                new Vector3(6.8f, -40f, 250.1f),
                new Vector3(11.7f, -40f, 70.3f)
            };
            var tallTreeRotation = Matrix.CreateRotationX(0);

            foreach (var tallTreePosition in tallTreePositions)
            {
                var treeColor = treePossibleColors[Random.Next(treePossibleColors.Length)];
                var translation = Matrix.CreateTranslation(tallTreePosition);
                DrawLevel6Element(tallTreeScale, translation, tallTreeRotation, TallTree, treeColor);
            }

            // Dibujamos las rocas
            var rockPossibleColors = new Color[]
            {
                Color.Gray,
                Color.DarkGray,
                Color.DimGray,
                Color.DarkSlateGray
            };

            var rockPossibleScales = new Matrix[]
            {
                Matrix.CreateScale(xScale: 1.1f, yScale: 1f, zScale: 1f),
                Matrix.CreateScale(xScale: 1.0f, yScale: 1f, zScale: 1f),
                Matrix.CreateScale(xScale: 1.2f, yScale: 1f, zScale: 1f),
                Matrix.CreateScale(xScale: 0.9f, yScale: 1f, zScale: 1f),
                Matrix.CreateScale(xScale: 1.0f, yScale: 1f, zScale: 0.9f),
                Matrix.CreateScale(xScale: 1.1f, yScale: 1f, zScale: 1.2f),
                Matrix.CreateScale(xScale: 0.8f, yScale: 1f, zScale: 1.0f),
                Matrix.CreateScale(xScale: 1.0f, yScale: 1f, zScale: 1.05f),
                Matrix.CreateScale(xScale: 1.15f, yScale: 1f, zScale: 1f),
                Matrix.CreateScale(xScale: 1.0f, yScale: 1f, zScale: 1.1f)
            };

            var normalRockPositions = new List<Vector3>
            {
                // Derecha
                new Vector3(27.6f, -40, -695),
                new Vector3(35.2f, -40, -670),
                new Vector3(45.1f, -40, -660),
                new Vector3(28.3f, -40, -640),
                new Vector3(37.4f, -40, -620),
                new Vector3(42.3f, -40, -600),
                new Vector3(30.1f, -40, -580),
                new Vector3(46.7f, -40, -560),
                new Vector3(29.5f, -40, -550),
                new Vector3(38.2f, -40, -520),
                new Vector3(44.5f, -40, -500),
                new Vector3(26.9f, -40, -480),
                new Vector3(41.0f, -40, -460),
                new Vector3(34.6f, -40, -440),
                new Vector3(47.0f, -40, -420),
                new Vector3(32.8f, -40, -400),
                new Vector3(39.5f, -40, -380),
                new Vector3(36.4f, -40, -360),
                new Vector3(27.2f, -40, -340),
                new Vector3(43.8f, -40, -310),
                new Vector3(33.9f, -40, -280),
                new Vector3(46.3f, -40, -250),
                new Vector3(31.3f, -40, -230),
                new Vector3(36.0f, -40, -210),
                new Vector3(28.9f, -40, -180),
                new Vector3(40.7f, -40, -150),
                new Vector3(47.2f, -40, -120),
                new Vector3(31.7f, -40, -90),
                new Vector3(42.1f, -40, -60),
                new Vector3(29.3f, -40, -30),
                new Vector3(44.1f, -40, 0),
                new Vector3(37.9f, -40, 40),
                new Vector3(32.6f, -40, 70),
                new Vector3(43.4f, -40, 100),
                new Vector3(30.2f, -40, 140),
                new Vector3(45.6f, -40, 170),
                new Vector3(33.8f, -40, 200),
                new Vector3(39.2f, -40, 230),
                new Vector3(25.7f, -40, 260),
                new Vector3(47.0f, -40, 280),

                // Izquierda
                new Vector3(-25.5f, -40f, -695f),
                new Vector3(-30.3f, -40f, -670f),
                new Vector3(-35.0f, -40f, -660f),
                new Vector3(-26.9f, -40f, -640f),
                new Vector3(-28.4f, -40f, -620f),
                new Vector3(-37.2f, -40f, -600f),
                new Vector3(-29.7f, -40f, -580f),
                new Vector3(-33.1f, -40f, -560f),
                new Vector3(-38.4f, -40f, -550f),
                new Vector3(-32.1f, -40f, -520f),
                new Vector3(-26.8f, -40f, -500f),
                new Vector3(-30.2f, -40f, -480f),
                new Vector3(-34.5f, -40f, -460f),
                new Vector3(-31.8f, -40f, -440f),
                new Vector3(-29.3f, -40f, -420f),
                new Vector3(-38.0f, -40f, -400f),
                new Vector3(-27.7f, -40f, -380f),
                new Vector3(-33.0f, -40f, -360f),
                new Vector3(-37.3f, -40f, -340f),
                new Vector3(-32.2f, -40f, -310f),
                new Vector3(-34.8f, -40f, -280f),
                new Vector3(-39.5f, -40f, -250f),
                new Vector3(-28.1f, -40f, -230f),
                new Vector3(-31.6f, -40f, -210f),
                new Vector3(-35.9f, -40f, -180f),
                new Vector3(-29.4f, -40f, -150f),
                new Vector3(-32.3f, -40f, -120f),
                new Vector3(-34.0f, -40f, -90f),
                new Vector3(-38.6f, -40f, -60f),
                new Vector3(-26.7f, -40f, -30f),
                new Vector3(-36.3f, -40f, 0f),
                new Vector3(-27.9f, -40f, 40f),
                new Vector3(-29.5f, -40f, 70f),
                new Vector3(-35.7f, -40f, 100f),
                new Vector3(-28.0f, -40f, 140f),
                new Vector3(-39.8f, -40f, 170f),
                new Vector3(-30.9f, -40f, 200f),
                new Vector3(-32.0f, -40f, 230f),
                new Vector3(-31.4f, -40f, 260f),
                new Vector3(-26.0f, -40f, 280f),

                // Parte sin rampas (random)
                new Vector3(-38.2f, -40f, 45.7f),
                new Vector3(25.6f, -40f, 60.1f),
                new Vector3(10.3f, -40f, 120.4f),
                new Vector3(33.7f, -40f, 110.9f),
                new Vector3(-29.4f, -40f, 95.2f),
                new Vector3(-18.1f, -40f, 210.3f),
                new Vector3(39.8f, -40f, 178.5f),
                new Vector3(-4.9f, -40f, 245.6f),
                new Vector3(19.5f, -40f, 170.7f),
                new Vector3(12.6f, -40f, 250.1f),
                new Vector3(-25.3f, -40f, 185.4f),
                new Vector3(-10.8f, -40f, 150.2f),
                new Vector3(35.4f, -40f, 222.1f),
                new Vector3(-32.5f, -40f, 129.6f),
                new Vector3(2.2f, -40f, 204.8f),
                new Vector3(6.1f, -40f, 280f),
                new Vector3(20.8f, -40f, 230.9f),
                new Vector3(38.1f, -40f, 140.3f),
                new Vector3(-15.6f, -40f, 160.1f),
                new Vector3(28.4f, -40f, 80.7f),
                new Vector3(0.7f, -40f, 50.6f),
                new Vector3(-27.9f, -40f, 230.2f),
                new Vector3(33.1f, -40f, 120.3f),
                new Vector3(-1.4f, -40f, 95.4f),
                new Vector3(37.2f, -40f, 190.4f),
                new Vector3(5.0f, -40f, 115.2f),
                new Vector3(-36.9f, -40f, 120.5f),
                new Vector3(29.7f, -40f, 170.3f),
                new Vector3(-8.3f, -40f, 140.9f),
                new Vector3(1.9f, -40f, 135.6f),
                new Vector3(-19.2f, -40f, 245.9f),
                new Vector3(4.3f, -40f, 180.7f),
                new Vector3(-2.5f, -40f, 200.8f),
                new Vector3(16.0f, -40f, 220.1f),
                new Vector3(36.8f, -40f, 110.6f),
                new Vector3(-5.6f, -40f, 70.3f),
                new Vector3(3.2f, -40f, 120.6f),
                new Vector3(22.3f, -40f, 180.2f),
                new Vector3(-13.9f, -40f, 190.1f),
                new Vector3(31.2f, -40f, 100.5f),
                new Vector3(15.4f, -40f, 120.2f)
            };
            var normalRockRotation = Matrix.CreateRotationX(0);

            foreach (var normalRockPosition in normalRockPositions)
            {
                var rockColor = rockPossibleColors[Random.Next(rockPossibleColors.Length)];
                var rockScale = rockPossibleScales[Random.Next(rockPossibleScales.Length)];
                var translation = Matrix.CreateTranslation(normalRockPosition);
                DrawLevel6Element(rockScale, translation, normalRockRotation, Support, rockColor);
            }

            void DrawLevel6Element(Matrix scale, Matrix translation, Matrix rotation, Model model, Color color)
            {
                var baseModelTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(baseModelTransforms);

                var transform = scale * translation * rotation * globalOffset * globalRotation;

                foreach (var mesh in model.Meshes)
                {
                    var relativeTransform = baseModelTransforms[mesh.ParentBone.Index];

                    Effect.Parameters["DiffuseColor"].SetValue(color.ToVector3());
                    Effect.Parameters["World"].SetValue(relativeTransform * transform);

                    mesh.Draw();
                }
            }
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqui el codigo referido al renderizado.
        /// </summary>
        ///
        private void DrawModel(Model model, bool rotate, float xPosition, float yPosition, float zPosition, Matrix offset, Color color)
        {
            var baseTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(baseTransforms);
            foreach (var mesh in model.Meshes)
            {
                var relativeTransform = baseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["DiffuseColor"].SetValue(color.ToVector3());
                if (rotate)
                {
                    Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(xPosition, yPosition, zPosition) * offset);
                }
                else
                {
                    Effect.Parameters["World"].SetValue(relativeTransform * Matrix.CreateTranslation(xPosition, yPosition, zPosition) * offset);
                }
                mesh.Draw();
            }
        }
    }
}