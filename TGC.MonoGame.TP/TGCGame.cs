using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Cameras;
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
        // Recursos
        public const string ContentFolder3D = "Models/";

        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderMusic = "Music/";
        public const string ContentFolderSounds = "Sounds/";
        public const string ContentFolderSpriteFonts = "SpriteFonts/";
        public const string ContentFolderTextures = "Textures/";

        // Graphics device manager
        private GraphicsDeviceManager Graphics { get; }

        // Semilla para lograr aleatoreidad deterministica
        // Es decir, random, pero siempre el mismo random
        private const int RANDOM_SEED = 0;

        private Random Random;

        // Matrices
        private Matrix View { get; set; }

        private Matrix World { get; set; }
        private Matrix Projection { get; set; }

        // Efectos
        private Effect Effect { get; set; }

        // Camaras
        private FreeCamera FreeCamera { get; set; }

        // Pelota ("personaje" principal)
        private Pelota pelota { get; set; }

        // Modelos
        private Model BannerHighModel { get; set; }
        private Model BendModel { get; set; }
        private Model BumpAModel { get; set; }
        private Model BumpDModel { get; set; }
        private Model BumpSolidBModel { get; set; }
        private Model CurveLargeModel { get; set; }
        private Model EndSquareModel { get; set; }
        private Model FunnelModel { get; set; }
        private Model FunnelLongModel { get; set; }
        private Model HelixHalfLeftModel { get; set; }
        private Model HelixHalfRightModel { get; set; }
        private Model HelixLargeHalfLeftModel { get; set; }
        private Model HelixLargeHalfRightModel { get; set; }
        private Model HelixLargeLeftModel { get; set; }
        private Model HelixLargeQuarterLeftModel { get; set; }
        private Model HelixLargeQuarterRightModel { get; set; }
        private Model HelixLargeRightModel { get; set; }
        private Model HelixLeftModel { get; set; }
        private Model HelixRightModel { get; set; }
        private Model BoxModel { get; set; }
        private Model CurveModel { get; set; }
        private Model NormalTreeModel { get; set; }
        private Model RampLongAModel { get; set; }
        private Model RampLongBModel { get; set; }
        private Model RampLongCModel { get; set; }
        private Model RampLongDModel { get; set; }
        private Model SlantLongAModel { get; set; }
        private Model SlantLongCModel { get; set; }
        private Model SlantLongDModel { get; set; }
        private Model SplitModel { get; set; }
        private Model SplitDoubleModel { get; set; }
        private Model SplitDoubleSidesModel { get; set; }
        private Model SplitLeftModel { get; set; }
        private Model SplitRightModel { get; set; }
        private Model StraightModel { get; set; }
        private Model SupportModel { get; set; }
        private Model TallTreeModel { get; set; }
        private Model TunnelModel { get; set; }
        private Model WallHalfModel { get; set; }
        private Model WaveAModel { get; set; }
        private Model WaveBModel { get; set; }
        private Model WaveCModel { get; set; }

        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);

            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";

            // Hace que el mouse sea visible.
            IsMouseVisible = true;
        }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // La logica de inicializacion que no depende del contenido se recomienda poner en este metodo.

            // Configuro las dimensiones de la pantalla.
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            Graphics.ApplyChanges();

            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;
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
        /// </summary>
        protected override void LoadContent()
        {
            // Aca es donde deberiamos cargar todos los contenido necesarios antes de iniciar el juego.

            // Cargo los modelos.
            BoxModel = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            CurveModel = Content.Load<Model>(ContentFolder3D + "curves/curve");
            SlantLongAModel = Content.Load<Model>(ContentFolder3D + "slants/slant_long_A");
            SlantLongCModel = Content.Load<Model>(ContentFolder3D + "slants/slant_long_C");
            BumpAModel = Content.Load<Model>(ContentFolder3D + "bump/bump_A");
            BumpSolidBModel = Content.Load<Model>(ContentFolder3D + "bump/bump_solid_B");
            RampLongAModel = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_A");
            RampLongBModel = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_B");
            RampLongCModel = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_C");
            RampLongDModel = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_D");
            CurveLargeModel = Content.Load<Model>(ContentFolder3D + "curves/curve_large");
            StraightModel = Content.Load<Model>(ContentFolder3D + "straights/straight");
            SplitModel = Content.Load<Model>(ContentFolder3D + "splits/split");
            SplitLeftModel = Content.Load<Model>(ContentFolder3D + "splits/split_left");
            SplitRightModel = Content.Load<Model>(ContentFolder3D + "splits/split_right");
            SplitDoubleModel = Content.Load<Model>(ContentFolder3D + "splits/split_double");
            SplitDoubleSidesModel = Content.Load<Model>(ContentFolder3D + "splits/split_double_sides");
            TunnelModel = Content.Load<Model>(ContentFolder3D + "extras/tunnel");
            HelixLeftModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_left");
            HelixRightModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_right");
            HelixHalfLeftModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_half_left");
            HelixHalfRightModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_half_right");
            HelixLargeHalfLeftModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_half_left");
            HelixLargeHalfRightModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_half_right");
            HelixLargeLeftModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_left");
            HelixLargeRightModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_right");
            HelixLargeQuarterLeftModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_quarter_left");
            HelixLargeQuarterRightModel = Content.Load<Model>(ContentFolder3D + "helixs/helix_large_quarter_right");
            WaveAModel = Content.Load<Model>(ContentFolder3D + "waves/wave_A");
            WaveBModel = Content.Load<Model>(ContentFolder3D + "waves/wave_B");
            WaveCModel = Content.Load<Model>(ContentFolder3D + "waves/wave_C");
            FunnelModel = Content.Load<Model>(ContentFolder3D + "funnels/funnel");
            FunnelLongModel = Content.Load<Model>(ContentFolder3D + "funnels/funnel_long");
            WallHalfModel = Content.Load<Model>(ContentFolder3D + "extras/wallHalf");
            NormalTreeModel = Content.Load<Model>(ContentFolder3D + "tree/tree");
            TallTreeModel = Content.Load<Model>(ContentFolder3D + "tree/tree_tall");
            SlantLongDModel = Content.Load<Model>(ContentFolder3D + "slants/slan_long_D");
            EndSquareModel = Content.Load<Model>(ContentFolder3D + "endHoles/end_square");
            SupportModel = Content.Load<Model>(ContentFolder3D + "supports/support_base");
            BumpDModel = Content.Load<Model>(ContentFolder3D + "bump/bump_D");
            BannerHighModel = Content.Load<Model>(ContentFolder3D + "banners/banner_high");
            BendModel = Content.Load<Model>(ContentFolder3D + "bend/bend_medium");

            // Cargo un efecto basico propio declarado en el Content pipeline.
            // En el juego no pueden usar BasicEffect de MG, deben usar siempre efectos propios.
            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");

            // Asigno los efectos para cada parte de las mesh.
            AssignEffect(BoxModel, Effect);
            AssignEffect(CurveModel, Effect);
            AssignEffect(SlantLongAModel, Effect);
            AssignEffect(SlantLongCModel, Effect);
            AssignEffect(BumpAModel, Effect);
            AssignEffect(BumpSolidBModel, Effect);
            AssignEffect(RampLongAModel, Effect);
            AssignEffect(RampLongBModel, Effect);
            AssignEffect(RampLongCModel, Effect);
            AssignEffect(RampLongDModel, Effect);
            AssignEffect(CurveLargeModel, Effect);
            AssignEffect(StraightModel, Effect);
            AssignEffect(SplitModel, Effect);
            AssignEffect(SplitLeftModel, Effect);
            AssignEffect(SplitRightModel, Effect);
            AssignEffect(SplitDoubleModel, Effect);
            AssignEffect(SplitDoubleSidesModel, Effect);
            AssignEffect(TunnelModel, Effect);
            AssignEffect(HelixLeftModel, Effect);
            AssignEffect(HelixRightModel, Effect);
            AssignEffect(HelixHalfLeftModel, Effect);
            AssignEffect(HelixHalfRightModel, Effect);
            AssignEffect(HelixLargeHalfLeftModel, Effect);
            AssignEffect(HelixLargeHalfRightModel, Effect);
            AssignEffect(HelixLargeLeftModel, Effect);
            AssignEffect(HelixLargeRightModel, Effect);
            AssignEffect(HelixLargeQuarterLeftModel, Effect);
            AssignEffect(HelixLargeQuarterRightModel, Effect);
            AssignEffect(WaveAModel, Effect);
            AssignEffect(WaveBModel, Effect);
            AssignEffect(WaveCModel, Effect);
            AssignEffect(FunnelModel, Effect);
            AssignEffect(FunnelLongModel, Effect);
            AssignEffect(WallHalfModel, Effect);
            AssignEffect(NormalTreeModel, Effect);
            AssignEffect(TallTreeModel, Effect);
            AssignEffect(SlantLongDModel, Effect);
            AssignEffect(EndSquareModel, Effect);
            AssignEffect(SupportModel, Effect);
            AssignEffect(BumpDModel, Effect);
            AssignEffect(BannerHighModel, Effect);
            AssignEffect(BendModel, Effect);

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

            // Matrices
            World = Matrix.Identity;
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
            Random = new Random(RANDOM_SEED);

            // Dibujo el escenario
            DrawLevel1();
            DrawLevel2();
            DrawLevel3();
            DrawLevel4();
            DrawLevel5();
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

        private void DrawLevel1()
        {
            // Posicion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(0f, 0f, -0f);

            float zBasePosition = 0f;
            float yBasePosition = 0f;

            Color[] colors =
            [
                Color.Peru,
                Color.Wheat
            ];

            int colorIndex = 0;

            for (int i = 0; i < 30; i++)
            {
                // Matrices locales
                Matrix translation = Matrix.CreateTranslation(0f, yBasePosition, zBasePosition);

                DrawModel(FunnelModel, translation: translation, color: colors[colorIndex], globalOffset: globalOffset);

                // Actualización de posición
                zBasePosition += 10f;
                yBasePosition += 5f;

                // Alternancia de color
                colorIndex = (colorIndex + 1) % colors.Length;
            }
        }

        private void DrawLevel2()
        {
            // Posicion y rotacion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(200f, 0f, 0f);
            Matrix globalRotation = Matrix.CreateRotationZ(0f);

            // Dibujamos rampas rectas con caida hacia abajo
            var rampDownPositions = new List<Vector3>
            {
                new Vector3(0f, 460f, 300f),
                new Vector3(0f, 410f, 270f),
                new Vector3(0f, 360f, 230f),
                new Vector3(0f, 310f, 200f),
                new Vector3(0f, 260f, 170f),
                new Vector3(0f, 210f, 130f),
                new Vector3(0f, 160f, 100f),
                new Vector3(0f, 110f, 70f),
                new Vector3(0f, 60f, 40f),
                new Vector3(0f, 10f, 10f)
            };

            foreach (var rampDownPosition in rampDownPositions)
            {
                DrawModel(SlantLongDModel,
                    translation: Matrix.CreateTranslation(rampDownPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1.5f, zScale: 2f),
                    color: Color.Yellow,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujo el "piso"
            var floorY = -40f;

            DrawModel(BoxModel,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -200f)),
                scale: Matrix.CreateScale(xScale: 50f, yScale: 1f, zScale: 500f),
                color: Color.SandyBrown,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos las rectas
            var straightPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -30f),
                new Vector3(0f, floorY, -90f),
                new Vector3(-10f, floorY, -350f),
                new Vector3(10f, floorY, -350f),
                new Vector3(0f, floorY, -450f),
                new Vector3(0f, floorY, -510f),
                new Vector3(0f, floorY, -570f),
                new Vector3(0f, floorY, -630f)
            };

            foreach (var straightPosition in straightPositions)
            {
                DrawModel(StraightModel,
                    translation: Matrix.CreateTranslation(straightPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 6f),
                    color: Color.Violet,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las divisiones
            var splitPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -140f)
            };

            foreach (var splitPosition in splitPositions)
            {
                DrawModel(SplitDoubleModel,
                    translation: Matrix.CreateTranslation(splitPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    rotation: Matrix.CreateRotationY(MathHelper.Pi),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las conjunciones
            var conjuctionPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -400f)
            };

            foreach (var conjuctionPosition in conjuctionPositions)
            {
                DrawModel(SplitDoubleModel,
                    translation: Matrix.CreateTranslation(conjuctionPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja
            var upDownRampPositions = new List<Vector3>
            {
                new Vector3(10f, floorY, -220f),
                new Vector3(10f, floorY, -260f),
                new Vector3(10f, floorY, -300f),
                new Vector3(10f, floorY, -180f)
            };

            foreach (var upDownRampPosition in upDownRampPositions)
            {
                DrawModel(BumpDModel,
                    translation: Matrix.CreateTranslation(upDownRampPosition),
                    color: Color.Black,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja leves
            var slowUpDownRampPositions = new List<Vector3>
            {
                new Vector3(-10f, floorY, -220f),
                new Vector3(-10f, floorY, -260f),
                new Vector3(-10f, floorY, -300f),
                new Vector3(-10f, floorY, -180f),
            };

            foreach (var slowUpDownRampPosition in slowUpDownRampPositions)
            {
                DrawModel(BumpAModel,
                    translation: Matrix.CreateTranslation(slowUpDownRampPosition),
                    color: Color.White,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos los tuneles
            var tunnelPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -530f),
            };

            foreach (var tunnelPosition in tunnelPositions)
            {
                DrawModel(TunnelModel,
                    translation: Matrix.CreateTranslation(tunnelPosition),
                    scale: Matrix.CreateScale(xScale: 3f, yScale: 5f, zScale: 16f),
                    color: Color.Teal,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las banderas
            var bannerPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -200f),
                new Vector3(0f, floorY, -100f),
                new Vector3(0f, floorY, -300f),
                new Vector3(0f, floorY, -400f),
                new Vector3(0f, floorY, -680f)
            };

            foreach (var bannerPosition in bannerPositions)
            {
                DrawModel(BannerHighModel,
                    translation: Matrix.CreateTranslation(bannerPosition),
                    scale: Matrix.CreateScale(xScale: 10f, yScale: 7f, zScale: 1f),
                    color: Color.DarkBlue,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos el final
            DrawModel(EndSquareModel,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -665f)),
                scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Red,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos los arboles
            var treeColors = new Color[]
            {
                Color.Green,
                Color.GreenYellow,
                Color.DarkGreen,
                Color.DarkOliveGreen,
                Color.DarkSeaGreen
            };

            var normalTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -320f),
                new Vector3(40f, floorY, -80f),
                new Vector3(40f, floorY, 220f),
                new Vector3(40f, floorY, 280f),
                new Vector3(40f, floorY, -440f),
                new Vector3(40f, floorY, -20f),
                new Vector3(40f, floorY, -560f),

                // Izquierda
                new Vector3(-40f, floorY, 220f),
                new Vector3(-40f, floorY, -560f),
                new Vector3(-40f, floorY, -320f),
                new Vector3(-40f, floorY, -80f),
                new Vector3(-40f, floorY, 280f),
                new Vector3(-40f, floorY, -440f),
                new Vector3(-40f, floorY, -20f),
                new Vector3(-40f, floorY, -180f),

                // Parte sin rampas ("random")
                new Vector3(-14.6f, floorY, 125.7f),
                new Vector3(37.4f, floorY, 185.8f),
                new Vector3(-9.3f, floorY, 240.9f),
                new Vector3(21.1f, floorY, 155.3f),
                new Vector3(-3.7f, floorY, 45.6f)
            };

            foreach (var normalTreePosition in normalTreePositions)
            {
                DrawModel(NormalTreeModel,
                    translation: Matrix.CreateTranslation(normalTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[Random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            var tallTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -500f),
                new Vector3(40f, floorY, 100f),
                new Vector3(40f, floorY, -620f),
                new Vector3(40f, floorY, -140f),
                new Vector3(40f, floorY, -260f),
                new Vector3(40f, floorY, -380f),
                new Vector3(40f, floorY, 160f),
                new Vector3(40f, floorY, 40f),
                new Vector3(40f, floorY, -670f),

                // Izquierda
                new Vector3(-40f, floorY, -260f),
                new Vector3(-40f, floorY, -500f),
                new Vector3(-40f, floorY, 100f),
                new Vector3(-40f, floorY, -620f),
                new Vector3(-40f, floorY, -140f),
                new Vector3(-40f, floorY, -380f),
                new Vector3(-40f, floorY, 160f),
                new Vector3(-40f, floorY, 40f),

                // Parte sin rampas (random)
                new Vector3(-12.8f, floorY, 56.3f),
                new Vector3(24.9f, floorY, 95.2f),
                new Vector3(8.1f, floorY, 145.6f),
                new Vector3(-35.4f, floorY, 120.4f),
                new Vector3(29.6f, floorY, 200.8f),
                new Vector3(16.3f, floorY, 215.5f),
                new Vector3(33.7f, floorY, 170.2f),
                new Vector3(-27.2f, floorY, 205.3f),
                new Vector3(3.0f, floorY, 160.1f),
                new Vector3(12.5f, floorY, 230.4f),
                new Vector3(-5.9f, floorY, 100.7f),
                new Vector3(20.2f, floorY, 130.6f),
                new Vector3(-31.3f, floorY, 190.2f),
                new Vector3(6.8f, floorY, 250.1f),
                new Vector3(11.7f, floorY, 70.3f)
            };

            foreach (var tallTreePosition in tallTreePositions)
            {
                DrawModel(TallTreeModel,
                    translation: Matrix.CreateTranslation(tallTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[Random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rocas
            var rockColors = new Color[]
            {
                Color.Gray,
                Color.DarkGray,
                Color.DimGray,
                Color.DarkSlateGray
            };

            var rockScales = new Matrix[]
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

            var rockPositions = new List<Vector3>
            {
                // Derecha
                new Vector3(27.6f, floorY, -695f),
                new Vector3(35.2f, floorY, -670f),
                new Vector3(45.1f, floorY, -660f),
                new Vector3(28.3f, floorY, -640f),
                new Vector3(37.4f, floorY, -620f),
                new Vector3(42.3f, floorY, -600f),
                new Vector3(30.1f, floorY, -580f),
                new Vector3(46.7f, floorY, -560f),
                new Vector3(29.5f, floorY, -550f),
                new Vector3(38.2f, floorY, -520f),
                new Vector3(44.5f, floorY, -500f),
                new Vector3(26.9f, floorY, -480f),
                new Vector3(41.0f, floorY, -460f),
                new Vector3(34.6f, floorY, -440f),
                new Vector3(47.0f, floorY, -420f),
                new Vector3(32.8f, floorY, 0f),
                new Vector3(39.5f, floorY, -380f),
                new Vector3(36.4f, floorY, -360f),
                new Vector3(27.2f, floorY, -340f),
                new Vector3(43.8f, floorY, -310f),
                new Vector3(33.9f, floorY, -280f),
                new Vector3(46.3f, floorY, -250f),
                new Vector3(31.3f, floorY, -230f),
                new Vector3(36.0f, floorY, -210f),
                new Vector3(28.9f, floorY, -180f),
                new Vector3(40.7f, floorY, -150f),
                new Vector3(47.2f, floorY, -120f),
                new Vector3(31.7f, floorY, -90f),
                new Vector3(42.1f, floorY, -60f),
                new Vector3(29.3f, floorY, -30f),
                new Vector3(44.1f, floorY, 0f),
                new Vector3(37.9f, floorY, 40f),
                new Vector3(32.6f, floorY, 70f),
                new Vector3(43.4f, floorY, 100f),
                new Vector3(30.2f, floorY, 140f),
                new Vector3(45.6f, floorY, 170f),
                new Vector3(33.8f, floorY, 200f),
                new Vector3(39.2f, floorY, 230f),
                new Vector3(25.7f, floorY, 260f),
                new Vector3(47.0f, floorY, 280f),

                // Izquierda
                new Vector3(-25.5f, floorY, -695f),
                new Vector3(-30.3f, floorY, -670f),
                new Vector3(-35.0f, floorY, -660f),
                new Vector3(-26.9f, floorY, -640f),
                new Vector3(-28.4f, floorY, -620f),
                new Vector3(-37.2f, floorY, -600f),
                new Vector3(-29.7f, floorY, -580f),
                new Vector3(-33.1f, floorY, -560f),
                new Vector3(-38.4f, floorY, -550f),
                new Vector3(-32.1f, floorY, -520f),
                new Vector3(-26.8f, floorY, -500f),
                new Vector3(-30.2f, floorY, -480f),
                new Vector3(-34.5f, floorY, -460f),
                new Vector3(-31.8f, floorY, -440f),
                new Vector3(-29.3f, floorY, -420f),
                new Vector3(-38.0f, floorY, 00f),
                new Vector3(-27.7f, floorY, -380f),
                new Vector3(-33.0f, floorY, -360f),
                new Vector3(-37.3f, floorY, -340f),
                new Vector3(-32.2f, floorY, -310f),
                new Vector3(-34.8f, floorY, -280f),
                new Vector3(-39.5f, floorY, -250f),
                new Vector3(-28.1f, floorY, -230f),
                new Vector3(-31.6f, floorY, -210f),
                new Vector3(-35.9f, floorY, -180f),
                new Vector3(-29.4f, floorY, -150f),
                new Vector3(-32.3f, floorY, -120f),
                new Vector3(-34.0f, floorY, -90f),
                new Vector3(-38.6f, floorY, -60f),
                new Vector3(-26.7f, floorY, -30f),
                new Vector3(-36.3f, floorY, 0f),
                new Vector3(-27.9f, floorY, 40f),
                new Vector3(-29.5f, floorY, 70f),
                new Vector3(-35.7f, floorY, 100f),
                new Vector3(-28.0f, floorY, 140f),
                new Vector3(-39.8f, floorY, 170f),
                new Vector3(-30.9f, floorY, 200f),
                new Vector3(-32.0f, floorY, 230f),
                new Vector3(-31.4f, floorY, 260f),
                new Vector3(-26.0f, floorY, 280f),

                // Parte sin rampas ("random")
                new Vector3(-38.2f, floorY, 45.7f),
                new Vector3(25.6f, floorY, 60.1f),
                new Vector3(10.3f, floorY, 120.4f),
                new Vector3(33.7f, floorY, 110.9f),
                new Vector3(-29.4f, floorY, 95.2f),
                new Vector3(-18.1f, floorY, 210.3f),
                new Vector3(39.8f, floorY, 178.5f),
                new Vector3(-4.9f, floorY, 245.6f),
                new Vector3(19.5f, floorY, 170.7f),
                new Vector3(12.6f, floorY, 250.1f),
                new Vector3(-25.3f, floorY, 185.4f),
                new Vector3(-10.8f, floorY, 150.2f),
                new Vector3(35.4f, floorY, 222.1f),
                new Vector3(-32.5f, floorY, 129.6f),
                new Vector3(2.2f, floorY, 204.8f),
                new Vector3(6.1f, floorY, 280f),
                new Vector3(20.8f, floorY, 230.9f),
                new Vector3(38.1f, floorY, 140.3f),
                new Vector3(-15.6f, floorY, 160.1f),
                new Vector3(28.4f, floorY, 80.7f),
                new Vector3(0.7f, floorY, 50.6f),
                new Vector3(-27.9f, floorY, 230.2f),
                new Vector3(33.1f, floorY, 120.3f),
                new Vector3(-1.4f, floorY, 95.4f),
                new Vector3(37.2f, floorY, 190.4f),
                new Vector3(5.0f, floorY, 115.2f),
                new Vector3(-36.9f, floorY, 120.5f),
                new Vector3(29.7f, floorY, 170.3f),
                new Vector3(-8.3f, floorY, 140.9f),
                new Vector3(1.9f, floorY, 135.6f),
                new Vector3(-19.2f, floorY, 245.9f),
                new Vector3(4.3f, floorY, 180.7f),
                new Vector3(-2.5f, floorY, 200.8f),
                new Vector3(16.0f, floorY, 220.1f),
                new Vector3(36.8f, floorY, 110.6f),
                new Vector3(-5.6f, floorY, 70.3f),
                new Vector3(3.2f, floorY, 120.6f),
                new Vector3(22.3f, floorY, 180.2f),
                new Vector3(-13.9f, floorY, 190.1f),
                new Vector3(31.2f, floorY, 100.5f),
                new Vector3(15.4f, floorY, 120.2f)
            };

            foreach (var rockPosition in rockPositions)
            {
                DrawModel(SupportModel,
                    translation: Matrix.CreateTranslation(rockPosition),
                    scale: rockScales[Random.Next(rockScales.Length)],
                    color: rockColors[Random.Next(rockColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }
        }

        private void DrawLevel3()
        {
            // Posicion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(400f, 0f, 0f);

            // --- Straight #1 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- CurveLarge #1 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(10f, 0f, 20f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #2 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(30f, 0f, 30f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #1 ---
            DrawModel(WaveAModel,
                translation: Matrix.CreateTranslation(new Vector3(55f, 0f, 28.75f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #3 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(80f, 0f, 30f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- CurveLarge #2 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(100f, 0f, 20f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveB #1 ---
            DrawModel(WaveBModel,
                translation: Matrix.CreateTranslation(112.5f, 0f, -15f),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #4 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(110, 0, -40)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- WaveC #1 ---
            DrawModel(WaveCModel,
                translation: Matrix.CreateTranslation(115f, 0f, -65f),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #5 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(110, 0, -90)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #3 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(120f, 0f, -110f)),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #6 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(140, 0, -120)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #2 ---
            DrawModel(WaveAModel,
                translation: Matrix.CreateTranslation(165f, 0f, -121.25f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #7 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(190, 0, -120)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- SplitDoubleSides #1---
            DrawModel(SplitDoubleSidesModel,
                translation: Matrix.CreateTranslation(205f, 0f, -120f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // #### CAMINO A (IZQUIERDA) ####
            // --- Straight #8 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(210, 0, -140)),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- CurveLarge #4 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(200f, 0f, -160f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #9 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(180, 0, -170)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- Straight #10 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(170, 0, -170)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Red,
               globalOffset: globalOffset);

            // --- WaveA #3 ---
            DrawModel(WaveAModel,
                translation: Matrix.CreateTranslation(new Vector3(145f, 0f, -171.25f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #11 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(120f, 0, -170f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #4 ---
            DrawModel(WaveAModel,
                translation: Matrix.CreateTranslation(new Vector3(95f, 0f, -168.75f)),
                rotation: Matrix.CreateRotationY(3 * MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #12 ---
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(new Vector3(70f, 0, -170f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #13 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(60f, 0, -170f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #5 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -160f)),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #14 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(30f, 0, -140f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // #### CAMINO B (DERECHA) ####
            // --- Straight #15 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(210f, 0, -100f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #6 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(200f, 0f, -80f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #16 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(180f, 0, -70f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- WaveB #2 ---
            DrawModel(WaveBModel,
                translation: Matrix.CreateTranslation(new Vector3(155f, 0f, -67.5f)),
                rotation: Matrix.CreateRotationY(3 * MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- WaveC #2 ---
            DrawModel(WaveCModel,
                translation: Matrix.CreateTranslation(new Vector3(85f, 0f, -75f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #17 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(60f, 0, -70f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #7 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -80f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #18 ---
            DrawModel(StraightModel,
               translation: Matrix.CreateTranslation(new Vector3(30f, 0, -100f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // #### UNION DE CAMINOS ####
            // --- SplitDoubleSides #2 ---
            DrawModel(SplitDoubleSidesModel,
                translation: Matrix.CreateTranslation(new Vector3(25f, 0f, -120f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- CurveLarge #8 ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -80f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);
        }

        private void DrawLevel4()
        {
            // Posicion y rotacion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(1200f, 0f, 0f);
            Matrix globalRotation = Matrix.CreateRotationY(MathHelper.Pi);

            // --- Straight #1
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-60f, 0f, 20f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #2
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-60f, 0f, 30f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Tunel #1
            DrawModel(TunnelModel,
                translation: Matrix.CreateTranslation(-60f, 5f, 20f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Tunel #2
            DrawModel(TunnelModel,
                translation: Matrix.CreateTranslation(-60f, 5f, 30f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #3
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-60f, 0f, 40f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Ramp Long D
            DrawModel(RampLongDModel,
                translation: Matrix.CreateTranslation(-60f, -15f, 55f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- CURVE LARGE  ---
            DrawModel(CurveLargeModel,
                translation: Matrix.CreateTranslation(-70f, -15f, 80f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #4
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-90f, -15f, 90f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Double Split Sides
            DrawModel(SplitDoubleSidesModel,
                translation: Matrix.CreateTranslation(-105f, -15f, 90f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2 * -1),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Bump A (derecha)
            DrawModel(BumpAModel,
                translation: Matrix.CreateTranslation(-110f, -15f, 55f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #4
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -15f, 30f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #5
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -15f, 20f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #6
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -15f, 10f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Helix Large Right #1
            DrawModel(HelixLargeRightModel,
                translation: Matrix.CreateTranslation(-135f, -35f, 5f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Helix Large Right #2
            DrawModel(HelixLargeRightModel,
                translation: Matrix.CreateTranslation(-135f, -55f, 5f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #6
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -80f, -30f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #7
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -80f, -40f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #8
            DrawModel(StraightModel,
                translation: Matrix.CreateTranslation(-110f, -80f, -50f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C  ---
            DrawModel(SlantLongCModel,
                translation: Matrix.CreateTranslation(-110f, -90f, -65f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C #2  ---
            DrawModel(SlantLongCModel,
                translation: Matrix.CreateTranslation(-110f, -95f, -75f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C #3  ---
            DrawModel(SlantLongCModel,
                translation: Matrix.CreateTranslation(-110f, -100f, -85f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);
        }

        private void DrawLevel5()
        {
            // Posicion y rotacion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(800f, 0f, 0f);
            Matrix globalRotation = Matrix.CreateRotationZ(0f);

            // Dibujamos rampas rectas con caida hacia abajo
            var rampDownPositions = new List<Vector3>
            {
                new Vector3(0f, 460f, 300f),
                new Vector3(0f, 410f, 270f),
                new Vector3(0f, 360f, 230f),
                new Vector3(0f, 310f, 200f),
                new Vector3(0f, 260f, 170f),
                new Vector3(0f, 210f, 130f),
                new Vector3(0f, 160f, 100f),
                new Vector3(0f, 110f, 70f),
                new Vector3(0f, 60f, 40f),
                new Vector3(0f, 10f, 10f)
            };

            foreach (var rampDownPosition in rampDownPositions)
            {
                DrawModel(SlantLongDModel,
                    translation: Matrix.CreateTranslation(rampDownPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1.5f, zScale: 2f),
                    color: Color.Yellow,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujo el "piso"
            var floorY = -40f;

            DrawModel(BoxModel,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -200f)),
                scale: Matrix.CreateScale(xScale: 50f, yScale: 1f, zScale: 500f),
                color: Color.SandyBrown,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos las rectas
            var straightPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -30f),
                new Vector3(0f, floorY, -90f),
                new Vector3(-10f, floorY, -350f),
                new Vector3(10f, floorY, -350f),
                new Vector3(0f, floorY, -450f),
                new Vector3(0f, floorY, -510f),
                new Vector3(0f, floorY, -570f),
                new Vector3(0f, floorY, -630f)
            };

            foreach (var straightPosition in straightPositions)
            {
                DrawModel(StraightModel,
                    translation: Matrix.CreateTranslation(straightPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 6f),
                    color: Color.Violet,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las divisiones
            var splitPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -140f)
            };

            foreach (var splitPosition in splitPositions)
            {
                DrawModel(SplitDoubleModel,
                    translation: Matrix.CreateTranslation(splitPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    rotation: Matrix.CreateRotationY(MathHelper.Pi),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las conjunciones
            var conjuctionPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -400f)
            };

            foreach (var conjuctionPosition in conjuctionPositions)
            {
                DrawModel(SplitDoubleModel,
                    translation: Matrix.CreateTranslation(conjuctionPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja
            var upDownRampPositions = new List<Vector3>
            {
                new Vector3(10f, floorY, -220f),
                new Vector3(10f, floorY, -260f),
                new Vector3(10f, floorY, -300f),
                new Vector3(10f, floorY, -180f)
            };

            foreach (var upDownRampPosition in upDownRampPositions)
            {
                DrawModel(BumpDModel,
                    translation: Matrix.CreateTranslation(upDownRampPosition),
                    color: Color.Black,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja leves
            var slowUpDownRampPositions = new List<Vector3>
            {
                new Vector3(-10f, floorY, -220f),
                new Vector3(-10f, floorY, -260f),
                new Vector3(-10f, floorY, -300f),
                new Vector3(-10f, floorY, -180f),
            };

            foreach (var slowUpDownRampPosition in slowUpDownRampPositions)
            {
                DrawModel(BumpAModel,
                    translation: Matrix.CreateTranslation(slowUpDownRampPosition),
                    color: Color.White,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos los tuneles
            var tunnelPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -530f),
            };

            foreach (var tunnelPosition in tunnelPositions)
            {
                DrawModel(TunnelModel,
                    translation: Matrix.CreateTranslation(tunnelPosition),
                    scale: Matrix.CreateScale(xScale: 3f, yScale: 5f, zScale: 16f),
                    color: Color.Teal,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las banderas
            var bannerPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -200f),
                new Vector3(0f, floorY, -100f),
                new Vector3(0f, floorY, -300f),
                new Vector3(0f, floorY, -400f),
                new Vector3(0f, floorY, -680f)
            };

            foreach (var bannerPosition in bannerPositions)
            {
                DrawModel(BannerHighModel,
                    translation: Matrix.CreateTranslation(bannerPosition),
                    scale: Matrix.CreateScale(xScale: 10f, yScale: 7f, zScale: 1f),
                    color: Color.DarkBlue,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos el final
            DrawModel(EndSquareModel,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -665f)),
                scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Red,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos los arboles
            var treeColors = new Color[]
            {
                Color.Green,
                Color.GreenYellow,
                Color.DarkGreen,
                Color.DarkOliveGreen,
                Color.DarkSeaGreen
            };

            var normalTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -320f),
                new Vector3(40f, floorY, -80f),
                new Vector3(40f, floorY, 220f),
                new Vector3(40f, floorY, 280f),
                new Vector3(40f, floorY, -440f),
                new Vector3(40f, floorY, -20f),
                new Vector3(40f, floorY, -560f),

                // Izquierda
                new Vector3(-40f, floorY, 220f),
                new Vector3(-40f, floorY, -560f),
                new Vector3(-40f, floorY, -320f),
                new Vector3(-40f, floorY, -80f),
                new Vector3(-40f, floorY, 280f),
                new Vector3(-40f, floorY, -440f),
                new Vector3(-40f, floorY, -20f),
                new Vector3(-40f, floorY, -180f),

                // Parte sin rampas ("random")
                new Vector3(-14.6f, floorY, 125.7f),
                new Vector3(37.4f, floorY, 185.8f),
                new Vector3(-9.3f, floorY, 240.9f),
                new Vector3(21.1f, floorY, 155.3f),
                new Vector3(-3.7f, floorY, 45.6f)
            };

            foreach (var normalTreePosition in normalTreePositions)
            {
                DrawModel(NormalTreeModel,
                    translation: Matrix.CreateTranslation(normalTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[Random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            var tallTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -500f),
                new Vector3(40f, floorY, 100f),
                new Vector3(40f, floorY, -620f),
                new Vector3(40f, floorY, -140f),
                new Vector3(40f, floorY, -260f),
                new Vector3(40f, floorY, -380f),
                new Vector3(40f, floorY, 160f),
                new Vector3(40f, floorY, 40f),
                new Vector3(40f, floorY, -670f),

                // Izquierda
                new Vector3(-40f, floorY, -260f),
                new Vector3(-40f, floorY, -500f),
                new Vector3(-40f, floorY, 100f),
                new Vector3(-40f, floorY, -620f),
                new Vector3(-40f, floorY, -140f),
                new Vector3(-40f, floorY, -380f),
                new Vector3(-40f, floorY, 160f),
                new Vector3(-40f, floorY, 40f),

                // Parte sin rampas (random)
                new Vector3(-12.8f, floorY, 56.3f),
                new Vector3(24.9f, floorY, 95.2f),
                new Vector3(8.1f, floorY, 145.6f),
                new Vector3(-35.4f, floorY, 120.4f),
                new Vector3(29.6f, floorY, 200.8f),
                new Vector3(16.3f, floorY, 215.5f),
                new Vector3(33.7f, floorY, 170.2f),
                new Vector3(-27.2f, floorY, 205.3f),
                new Vector3(3.0f, floorY, 160.1f),
                new Vector3(12.5f, floorY, 230.4f),
                new Vector3(-5.9f, floorY, 100.7f),
                new Vector3(20.2f, floorY, 130.6f),
                new Vector3(-31.3f, floorY, 190.2f),
                new Vector3(6.8f, floorY, 250.1f),
                new Vector3(11.7f, floorY, 70.3f)
            };

            foreach (var tallTreePosition in tallTreePositions)
            {
                DrawModel(TallTreeModel,
                    translation: Matrix.CreateTranslation(tallTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[Random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rocas
            var rockColors = new Color[]
            {
                Color.Gray,
                Color.DarkGray,
                Color.DimGray,
                Color.DarkSlateGray
            };

            var rockScales = new Matrix[]
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

            var rockPositions = new List<Vector3>
            {
                // Derecha
                new Vector3(27.6f, floorY, -695f),
                new Vector3(35.2f, floorY, -670f),
                new Vector3(45.1f, floorY, -660f),
                new Vector3(28.3f, floorY, -640f),
                new Vector3(37.4f, floorY, -620f),
                new Vector3(42.3f, floorY, -600f),
                new Vector3(30.1f, floorY, -580f),
                new Vector3(46.7f, floorY, -560f),
                new Vector3(29.5f, floorY, -550f),
                new Vector3(38.2f, floorY, -520f),
                new Vector3(44.5f, floorY, -500f),
                new Vector3(26.9f, floorY, -480f),
                new Vector3(41.0f, floorY, -460f),
                new Vector3(34.6f, floorY, -440f),
                new Vector3(47.0f, floorY, -420f),
                new Vector3(32.8f, floorY, 0f),
                new Vector3(39.5f, floorY, -380f),
                new Vector3(36.4f, floorY, -360f),
                new Vector3(27.2f, floorY, -340f),
                new Vector3(43.8f, floorY, -310f),
                new Vector3(33.9f, floorY, -280f),
                new Vector3(46.3f, floorY, -250f),
                new Vector3(31.3f, floorY, -230f),
                new Vector3(36.0f, floorY, -210f),
                new Vector3(28.9f, floorY, -180f),
                new Vector3(40.7f, floorY, -150f),
                new Vector3(47.2f, floorY, -120f),
                new Vector3(31.7f, floorY, -90f),
                new Vector3(42.1f, floorY, -60f),
                new Vector3(29.3f, floorY, -30f),
                new Vector3(44.1f, floorY, 0f),
                new Vector3(37.9f, floorY, 40f),
                new Vector3(32.6f, floorY, 70f),
                new Vector3(43.4f, floorY, 100f),
                new Vector3(30.2f, floorY, 140f),
                new Vector3(45.6f, floorY, 170f),
                new Vector3(33.8f, floorY, 200f),
                new Vector3(39.2f, floorY, 230f),
                new Vector3(25.7f, floorY, 260f),
                new Vector3(47.0f, floorY, 280f),

                // Izquierda
                new Vector3(-25.5f, floorY, -695f),
                new Vector3(-30.3f, floorY, -670f),
                new Vector3(-35.0f, floorY, -660f),
                new Vector3(-26.9f, floorY, -640f),
                new Vector3(-28.4f, floorY, -620f),
                new Vector3(-37.2f, floorY, -600f),
                new Vector3(-29.7f, floorY, -580f),
                new Vector3(-33.1f, floorY, -560f),
                new Vector3(-38.4f, floorY, -550f),
                new Vector3(-32.1f, floorY, -520f),
                new Vector3(-26.8f, floorY, -500f),
                new Vector3(-30.2f, floorY, -480f),
                new Vector3(-34.5f, floorY, -460f),
                new Vector3(-31.8f, floorY, -440f),
                new Vector3(-29.3f, floorY, -420f),
                new Vector3(-38.0f, floorY, 00f),
                new Vector3(-27.7f, floorY, -380f),
                new Vector3(-33.0f, floorY, -360f),
                new Vector3(-37.3f, floorY, -340f),
                new Vector3(-32.2f, floorY, -310f),
                new Vector3(-34.8f, floorY, -280f),
                new Vector3(-39.5f, floorY, -250f),
                new Vector3(-28.1f, floorY, -230f),
                new Vector3(-31.6f, floorY, -210f),
                new Vector3(-35.9f, floorY, -180f),
                new Vector3(-29.4f, floorY, -150f),
                new Vector3(-32.3f, floorY, -120f),
                new Vector3(-34.0f, floorY, -90f),
                new Vector3(-38.6f, floorY, -60f),
                new Vector3(-26.7f, floorY, -30f),
                new Vector3(-36.3f, floorY, 0f),
                new Vector3(-27.9f, floorY, 40f),
                new Vector3(-29.5f, floorY, 70f),
                new Vector3(-35.7f, floorY, 100f),
                new Vector3(-28.0f, floorY, 140f),
                new Vector3(-39.8f, floorY, 170f),
                new Vector3(-30.9f, floorY, 200f),
                new Vector3(-32.0f, floorY, 230f),
                new Vector3(-31.4f, floorY, 260f),
                new Vector3(-26.0f, floorY, 280f),

                // Parte sin rampas ("random")
                new Vector3(-38.2f, floorY, 45.7f),
                new Vector3(25.6f, floorY, 60.1f),
                new Vector3(10.3f, floorY, 120.4f),
                new Vector3(33.7f, floorY, 110.9f),
                new Vector3(-29.4f, floorY, 95.2f),
                new Vector3(-18.1f, floorY, 210.3f),
                new Vector3(39.8f, floorY, 178.5f),
                new Vector3(-4.9f, floorY, 245.6f),
                new Vector3(19.5f, floorY, 170.7f),
                new Vector3(12.6f, floorY, 250.1f),
                new Vector3(-25.3f, floorY, 185.4f),
                new Vector3(-10.8f, floorY, 150.2f),
                new Vector3(35.4f, floorY, 222.1f),
                new Vector3(-32.5f, floorY, 129.6f),
                new Vector3(2.2f, floorY, 204.8f),
                new Vector3(6.1f, floorY, 280f),
                new Vector3(20.8f, floorY, 230.9f),
                new Vector3(38.1f, floorY, 140.3f),
                new Vector3(-15.6f, floorY, 160.1f),
                new Vector3(28.4f, floorY, 80.7f),
                new Vector3(0.7f, floorY, 50.6f),
                new Vector3(-27.9f, floorY, 230.2f),
                new Vector3(33.1f, floorY, 120.3f),
                new Vector3(-1.4f, floorY, 95.4f),
                new Vector3(37.2f, floorY, 190.4f),
                new Vector3(5.0f, floorY, 115.2f),
                new Vector3(-36.9f, floorY, 120.5f),
                new Vector3(29.7f, floorY, 170.3f),
                new Vector3(-8.3f, floorY, 140.9f),
                new Vector3(1.9f, floorY, 135.6f),
                new Vector3(-19.2f, floorY, 245.9f),
                new Vector3(4.3f, floorY, 180.7f),
                new Vector3(-2.5f, floorY, 200.8f),
                new Vector3(16.0f, floorY, 220.1f),
                new Vector3(36.8f, floorY, 110.6f),
                new Vector3(-5.6f, floorY, 70.3f),
                new Vector3(3.2f, floorY, 120.6f),
                new Vector3(22.3f, floorY, 180.2f),
                new Vector3(-13.9f, floorY, 190.1f),
                new Vector3(31.2f, floorY, 100.5f),
                new Vector3(15.4f, floorY, 120.2f)
            };

            foreach (var rockPosition in rockPositions)
            {
                DrawModel(SupportModel,
                    translation: Matrix.CreateTranslation(rockPosition),
                    scale: rockScales[Random.Next(rockScales.Length)],
                    color: rockColors[Random.Next(rockColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }
        }

        private void DrawModel(
            Model model,
            Matrix? translation = null,
            Matrix? scale = null,
            Matrix? rotation = null,
            Color? color = null,
            Matrix? globalOffset = null,
            Matrix? globalRotation = null)
        {
            // Valores por defecto
            translation ??= Matrix.Identity;
            scale ??= Matrix.CreateScale(1f);
            rotation ??= Matrix.Identity;
            globalOffset ??= Matrix.Identity;
            globalRotation ??= Matrix.Identity;
            color ??= Color.Black;

            // Transformaciones del modelo base
            var baseTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(baseTransforms);

            // Transformación local (modelo)
            // Esto se lee: Escala -> Rotacion -> Traslacion
            var localTransform = scale.Value * rotation.Value * translation.Value;

            // Transformacion global (escena)
            // Esto se lee: Rotacion -> Traslacion
            var worldTransform = localTransform * globalRotation.Value * globalOffset.Value;

            foreach (var mesh in model.Meshes)
            {
                var meshTransform = baseTransforms[mesh.ParentBone.Index];

                Effect.Parameters["View"].SetValue(View);
                Effect.Parameters["Projection"].SetValue(Projection);
                Effect.Parameters["World"].SetValue(meshTransform * worldTransform);
                Effect.Parameters["DiffuseColor"].SetValue(color.Value.ToVector3());

                mesh.Draw();
            }
        }

        public static void AssignEffect(Model model, Effect effect)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect;
                }
            }
        }
    }
}