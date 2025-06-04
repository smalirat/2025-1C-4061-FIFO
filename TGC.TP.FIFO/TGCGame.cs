using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Skybox;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Audio;

namespace TGC.TP.FIFO;




public class TGCGame : Game
{
    private readonly GraphicsDeviceManager Graphics;
    private readonly ModelManager ModelManager;
    private readonly EffectManager EffectManager;
    private readonly PhysicsManager PhysicsManager;
    private readonly TextureManager TextureManager;
    private readonly AudioManager AudioManager;

    private TargetCamera TargetCamera;
    private PlayerBall PlayerBall;
    private List<FloorWallRamp> FloorWallRamps = new();

    private SimpleSkyBox Skybox;

    private List<DynamicBox> DynamicBoxes = new();
    private List<StaticBox> StaticBoxes = new();

    private List<KinematicWall> KinematicWalls = new();
    private List<KinematicFloor> KinematicFloors = new();

    private Random random;

    private List<SpeedPowerUp> SpeedPowerUps = new();
    private List<JumpPowerUp> JumpPowerUps = new();

    private List<Checkpoint> Checkpoints = new();

    private SpriteFont _font;
    private SpriteBatch _spriteBatch;
    private float _gameTimer;
    private bool _isGameActive;
    private Vector2 _timerPosition;
    private Color _timerColor;

    private Texture2D _ballTypeRubberTexture;
    private Texture2D _ballTypeMetalTexture;
    private Texture2D _ballTypeStoneTexture;

    private Vector2 _ballTypePosition;

    private Vector2 _speedPosition;

    private int _currentCheckpointId = 0;
    private int _totalCheckpoints = 0;
    private Vector2 _progressBarPosition;
    private Vector2 _progressBarSize;
    private Color _progressBarBackgroundColor = new Color(0, 0, 0, 128); // Negro semi-transparente
    private Color _progressBarFillColor = Color.Green;
    private Texture2D _progressBarTexture;

    private Model _arrowModel;
    private Vector3 _arrowPosition;
    private float _arrowScale = 0.5f; // Ajusta este valor según el tamaño que necesites
    private Matrix _arrowRotation = Matrix.Identity;
    private float _arrowFloatOffset = 0f;
    private const float ARROW_FLOAT_SPEED = 1f;
    private const float ARROW_FLOAT_AMPLITUDE = 0.2f;

    public TGCGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        ModelManager = new ModelManager();
        EffectManager = new EffectManager();
        PhysicsManager = new PhysicsManager();
        TextureManager = new TextureManager();
        AudioManager = new AudioManager();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _gameTimer = 0f;
        _isGameActive = true;
        _timerColor = Color.White;

        random = new Random(6814);
    }

    protected override void Initialize()
    {
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
        Graphics.ApplyChanges();

        PhysicsManager.Initialize();

        Skybox = new SimpleSkyBox(ModelManager, EffectManager, TextureManager, TiposSkybox.Roca);
        PlayerBall = new PlayerBall(ModelManager, EffectManager, PhysicsManager, TextureManager, AudioManager, GraphicsDevice, new Vector3(0, 50f, 0f), BallType.Rubber);

        InitializeLevel1();
        InitializeLevel2();
        InitializeLevel3();

        TargetCamera = new TargetCamera(MathF.PI / 3f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100000f, PlayerBall.Position, 30f, 0.01f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        EffectManager.Load(Content);
        ModelManager.Load(Content);
        TextureManager.Load(Content);
        AudioManager.Load(Content);
        AudioManager.PlayBackgroundMusic();
        base.LoadContent();

        // Cargar la fuente para el timer
        _font = Content.Load<SpriteFont>("Fonts/TimerFont");
        _ballTypeRubberTexture = Content.Load<Texture2D>("Textures/Rubber");
        _ballTypeMetalTexture = Content.Load<Texture2D>("Textures/harsh-metal/color");
        _ballTypeStoneTexture = Content.Load<Texture2D>("Textures/marble/color");


        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _timerPosition = new Vector2(
            GraphicsDevice.Viewport.Width - 150,
            50  // Ajusta este valor según donde quieras que aparezca el timer
        );

        // Cargar texturas del velocímetro

        // Inicializar posiciones
        _ballTypePosition = new Vector2(50, 100);

        // Posición del texto de velocidad
        _speedPosition = new Vector2(
            GraphicsDevice.Viewport.Width - 150,
            GraphicsDevice.Viewport.Height - 100
        );

        // Crear una textura para la barra de progreso
        _progressBarTexture = new Texture2D(GraphicsDevice, 1, 1);
        _progressBarTexture.SetData(new[] { Color.White });

        // Configurar la posición y tamaño de la barra de progreso
        _progressBarSize = new Vector2(200, 20);
        _progressBarPosition = new Vector2(
            GraphicsDevice.Viewport.Width - _progressBarSize.X - 50,
            100
        );

        // Contar el total de checkpoints
        _totalCheckpoints = Checkpoints.Count;

        // Cargar el modelo de la flecha
        _arrowModel = Content.Load<Model>("Models/hud/3D_Arrow");

        // Calcular la posición de la flecha en la esquina inferior derecha
        _arrowPosition = new Vector3(
            GraphicsDevice.Viewport.Width - 100,  // 100 píxeles desde el borde derecho
            GraphicsDevice.Viewport.Height - 100,  // 100 píxeles desde el borde inferior
            0
        );
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape)) Exit();

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        PlayerBall.Update(keyboardState, deltaTime, TargetCamera);

        PhysicsManager.Update(deltaTime);
        TargetCamera.Update(PlayerBall.Position);

        foreach (var box in DynamicBoxes)
        {
            box.Update(deltaTime, TargetCamera);
        }

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Update(deltaTime, TargetCamera);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Update(deltaTime, TargetCamera);
        }

        foreach (var jumpPowerUp in JumpPowerUps)
        {
            jumpPowerUp.Update(deltaTime);
        }

        foreach (var speedPowerUp in SpeedPowerUps)
        {
            speedPowerUp.Update(deltaTime);
        }

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Update(deltaTime);
        }

        if (_isGameActive)
        {
            _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _currentCheckpointId = Checkpoint.GetLastActivatedCheckpointId();

        _arrowFloatOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * ARROW_FLOAT_SPEED) * ARROW_FLOAT_AMPLITUDE;

        if (_currentCheckpointId < _totalCheckpoints)
        {
            var nextCheckpoint = Checkpoints.FirstOrDefault(c => c.Id == _currentCheckpointId + 1);
            if (nextCheckpoint != null)
            {
                Vector3 direction = nextCheckpoint.Position - TargetCamera.TargetPosition;
                direction.Normalize();

                // Calculamos la rotación en el plano XZ (horizontal)
                float yaw = (float)Math.Atan2(direction.X, direction.Z);

                // Calculamos la rotación en el plano YZ (vertical)
                float pitch = (float)Math.Asin(direction.Y);

                // Aplicamos ambas rotaciones
                Matrix rotationMatrix = Matrix.CreateRotationY(yaw) * Matrix.CreateRotationX(pitch);
                _arrowRotation = rotationMatrix;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        Skybox.Draw(TargetCamera.View, TargetCamera.Projection, PlayerBall.Position, GraphicsDevice);

        foreach (var item in FloorWallRamps)
        {
            item.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var box in DynamicBoxes)
        {
            box.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var staticBox in StaticBoxes)
        {
            staticBox.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var powerUp in SpeedPowerUps)
        {
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var powerUp in JumpPowerUps)
        {
            powerUp.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var kinematicWall in KinematicWalls)
        {
            kinematicWall.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var kinematicFloor in KinematicFloors)
        {
            kinematicFloor.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Draw(TargetCamera.View, TargetCamera.Projection);
        }

        PlayerBall.Draw(TargetCamera.View, TargetCamera.Projection);

        // Guardar estados
        var originalDepthStencilState = GraphicsDevice.DepthStencilState;
        var originalBlendState = GraphicsDevice.BlendState;
        var originalSamplerState = GraphicsDevice.SamplerStates[0];
        var originalRasterizerState = GraphicsDevice.RasterizerState;

        // Primero dibujamos el texto y la textura normal
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullNone);

        string ballType = PlayerBall.ballType.ToString();
        _spriteBatch.DrawString(_font, $"Tipo: {ballType}", _ballTypePosition, Color.White);

        // Timer existente
        int minutes = (int)(_gameTimer / 60);
        int seconds = (int)(_gameTimer % 60);
        string timerText = $"Tiempo: {minutes:00}:{seconds:00}";
        _spriteBatch.DrawString(_font, timerText, _timerPosition, _timerColor);


        // Luego dibujamos la textura circular con el shader
        Texture2D ballTexture;
        switch (PlayerBall.ballType)
        {
            case BallType.Rubber:
                ballTexture = _ballTypeRubberTexture;
                break;
            case BallType.Metal:
                ballTexture = _ballTypeMetalTexture;
                break;
            case BallType.Stone:
                ballTexture = _ballTypeStoneTexture;
                break;
            default:
                ballTexture = _ballTypeRubberTexture;
                break;
        }

        // Posición y tamaño del círculo
        int circleSize = 32; // Tamaño del círculo en píxeles
        Vector2 texturePosition = new Vector2(_ballTypePosition.X, _ballTypePosition.Y + 30);
        Rectangle destinationRect = new Rectangle((int)texturePosition.X, (int)texturePosition.Y, circleSize, circleSize);

        // Cargar y aplicar el shader circular
        var circleShader = Content.Load<Effect>("Effects/CircleShader");
        circleShader.Parameters["ModelTexture"].SetValue(ballTexture);

        _spriteBatch.Draw(ballTexture, destinationRect, Color.White);


        float currentSpeedText = PlayerBall.GetCurrentSpeed();
        string speedTextText = $"Velocidad: {currentSpeedText:F1}";
        _spriteBatch.DrawString(_font, speedTextText, _speedPosition, Color.White);

        // Dibujar la barra de progreso
        float progress = _currentCheckpointId / (float)_totalCheckpoints;

        // Dibujar el fondo de la barra
        _spriteBatch.Draw(_progressBarTexture,
            new Rectangle((int)_progressBarPosition.X, (int)_progressBarPosition.Y,
                         (int)_progressBarSize.X, (int)_progressBarSize.Y),
            _progressBarBackgroundColor);

        // Dibujar el progreso
        _spriteBatch.Draw(_progressBarTexture,
            new Rectangle((int)_progressBarPosition.X, (int)_progressBarPosition.Y,
                         (int)(_progressBarSize.X * progress), (int)_progressBarSize.Y),
            _progressBarFillColor);

        // Dibujar el texto del progreso
        string progressText = $"Checkpoint: {_currentCheckpointId}/{_totalCheckpoints}";
        _spriteBatch.DrawString(_font, progressText,
            new Vector2(_progressBarPosition.X, _progressBarPosition.Y - 25),
            Color.White);

        _spriteBatch.End();


        // ### MINIMAPA ###
        int minimapSize = 150;
        Vector2 minimapPosition = new Vector2(20, GraphicsDevice.Viewport.Height - minimapSize - 20);


        // Dibuja el fondo del minimapa
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _spriteBatch.Draw(_progressBarTexture, new Rectangle((int)minimapPosition.X, (int)minimapPosition.Y, minimapSize, minimapSize), Color.Black * 0.7f);

        
    
        // Checkpoints en el minimapa
        float minX = -75f, maxX = 75f, minZ = -75f, maxZ = 900f;

        foreach (var checkpoint in Checkpoints)
        {
            Vector3 check = checkpoint.Position;
            float checkNormX = (check.X - minX) / (maxX - minX);
            float checkNormZ = (check.Z - minZ) / (maxZ - minZ);
            Vector2 checkMinimapPos = minimapPosition + new Vector2(checkNormX * minimapSize, minimapSize - checkNormZ * minimapSize);

            // Si el checkpoint está activado, lo dibuja en verde, si no en amarillo
            Color color = checkpoint.Id <= _currentCheckpointId ? Color.LimeGreen : Color.Yellow;

            _spriteBatch.Draw(_progressBarTexture, new Rectangle((int)(checkMinimapPos.X - 4), (int)(checkMinimapPos.Y - 4), 8, 8), color);
        }

        // Pelota en el minimapa
        Vector3 ballPos = PlayerBall.Position;
        float normX = (ballPos.X - minX) / (maxX - minX);
        float normZ = (ballPos.Z - minZ) / (maxZ - minZ);
        Vector2 ballMinimapPos = minimapPosition + new Vector2((1 - normX) * minimapSize, minimapSize - normZ * minimapSize);        int ballDotSize = 6;
        _spriteBatch.Draw(_progressBarTexture, new Rectangle((int)(ballMinimapPos.X - ballDotSize / 2), (int)(ballMinimapPos.Y - ballDotSize / 2), ballDotSize, ballDotSize), Color.Red);

        _spriteBatch.End();


        GraphicsDevice.DepthStencilState = originalDepthStencilState;
        GraphicsDevice.BlendState = originalBlendState;
        GraphicsDevice.SamplerStates[0] = originalSamplerState;
        GraphicsDevice.RasterizerState = originalRasterizerState;

        var checkpointPosition = Checkpoints[_currentCheckpointId].Position;
        var toCheckpoint = checkpointPosition - TargetCamera.TargetPosition;
        toCheckpoint.Y = 0; // ignoramos altura
        toCheckpoint.Normalize();

        // La flecha en reposo "mira" hacia adelante (-Z), así que rotamos desde -Z a toCheckpoint
        var forward = -Vector3.UnitZ;
        var rotationAngle = MathF.Atan2(toCheckpoint.X, toCheckpoint.Z);

        // Posición relativa a la cámara en el mundo (como HUD)
        Vector3 hudOffset =
            TargetCamera.RightXZ * 10f -    // Derecha
            Vector3.Up +               // Abajo
            TargetCamera.ForwardXZ * 15f;   // Un poco atrás

        Vector3 hudPosition = TargetCamera.TargetPosition + hudOffset;

        Matrix worldArrow =
            Matrix.CreateScale(_arrowScale) *
            _arrowRotation * // Usamos la matriz de rotación completa
            Matrix.CreateTranslation(hudPosition);

        foreach (var mesh in _arrowModel.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                effect.World = worldArrow;
                effect.View = TargetCamera.View;
                effect.Projection = TargetCamera.Projection;
            }
            mesh.Draw();
        }
    }

    private void InitializeLevel1()
    {
        // Checkpoint
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0f, 0f, 0f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue, 1));

        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, 0f, 0f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -0.1f, 150f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -23.5f, 296f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 10f), 150f, 150f, true, RampWallTextureType.Dirt));

        // Paredes
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, 75f, -75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, 75f, 0f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, 75f, 150f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));

        // Obstaculos
        KinematicWalls.Add(new KinematicWall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(0f, 11f, 225f), 40f, 20f, 1f, 1f, false, 50f));

        // Cajas dinamicas en piramide
        float boxSize = 5f;
        float spacing = 5f;
        XnaVector3 baseCenter = new XnaVector3(0f, 5f, 180f);

        for (int i = -3; i <= 3; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, 0f, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        for (int i = -2; i <= 2; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        for (int i = -1; i <= 1; i++)
            DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                baseCenter + new XnaVector3(i * spacing, 2 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            baseCenter + new XnaVector3(0f, 3 * spacing, 0f), XnaQuaternion.Identity, boxSize, 1f, 1f));

        // PowerUps
        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(-15f, 2f, 30f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.1f, Color.Yellow));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(15f, 2f, 30f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.2f, Color.Orange));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(-15f, 2f, 70f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.3f, Color.Red));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(25f, 2f, 80f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.3f, Color.Red));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(-47f, 2f, 92f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.2f, Color.Orange));

        JumpPowerUps.Add(new JumpPowerUp(ModelManager, EffectManager, PhysicsManager, AudioManager, GraphicsDevice,
            new XnaVector3(-5f, 2f, 140f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f), 1f, 5f, 3f, 0.1f, Color.Yellow));

        // Cajas estáticas
        List<(XnaVector3 pos, XnaQuaternion rot, float size)> cajas = new();

        float[] staticBoxSizes = { 3f, 6f, 9f };

        List<XnaVector2> staticBoxPositions = new();

        for (int i = 0; i < 30; i++)
        {
            XnaVector2 position;
            bool valid;

            int attempts = 0;
            do
            {
                float x = (float)(random.NextDouble() * 115.0 - 55.0);
                float z = (float)(random.NextDouble() * 260.0 - 60.0);
                position = new XnaVector2(x, z);

                // Verificar que no esté cerca de otra posición existente
                valid = staticBoxPositions.All(p => XnaVector2.Distance(p, position) >= 18f);
                attempts++;

                if (attempts > 1000)
                {
                    throw new Exception("No se pudieron generar suficientes posiciones únicas");
                }
            } while (!valid);

            staticBoxPositions.Add(position);
        }

        for (int i = 0; i < staticBoxPositions.Count; i++)
        {
            float x = staticBoxPositions[i].X;
            float z = staticBoxPositions[i].Y;

            // Una caja sola tamaño random
            if (i % 3 == 0)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
            // Dos cajas del mismo tamaño apiladas
            else if (i % 3 == 1)
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
                cajas.Add((new XnaVector3(x, size + size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
            else
            {
                float randomAngle = (float)(random.NextDouble() * 2 * MathF.PI);
                float size = staticBoxSizes[random.Next(staticBoxSizes.Length)];
                cajas.Add((new XnaVector3(x, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
                cajas.Add((new XnaVector3(x + size + 2f, size / 2f + 0.75f, z), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, randomAngle), size));
            }
        }

        foreach (var (pos, rot, size) in cajas)
        {
            StaticBoxes.Add(new StaticBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, pos, rot, size));
        }
    }

    private void InitializeLevel2()
    {
        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.5f, 442f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.6f, 442f + 150f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        // PowerUps
        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(-44f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(45f, -46.5f + 2f, 442f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(-44f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(45f, -46.5f + 2f, 394f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(-44f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 15f, Color.Yellow));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 60f, Color.Red));

        SpeedPowerUps.Add(new SpeedPowerUp(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(45f, -46.5f + 2f, 515f), XnaQuaternion.Identity, 3f, 3f, 1f, 30f, Color.Orange));

        // Checkpoint
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0, -46.5f + 2f, 380f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue, 2));

        // Obstaculos
        for (int i = 0; i < 5; i++)
        {
            KinematicWalls.Add(new KinematicWall(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(0, -46.5f + 10f, 410f + 21f * i), 20f, 20f, 1f, 1f, false, 50f - i * 4));
        }

        // Cajas dinamicas
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-55f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 5, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-30f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(-10, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 6, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(22f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 2, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(30f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 8, 1f, 0.1f));
        DynamicBoxes.Add(new DynamicBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice, new XnaVector3(52f, -46.5f + 10f, 550f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, (float)(random.NextDouble() * 2 * MathF.PI)), 3, 1f, 0.1f));
    }

    private void InitializeLevel3()
    {
        // Pisos
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.7f, 742f), XnaQuaternion.Identity, 150f, 150f, true, RampWallTextureType.Dirt));

        // Paredes
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(0f, -46.7f + 75f, 742f + 75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));
        FloorWallRamps.Add(new FloorWallRamp(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(75f, -46.7f + 75f, 742f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, -MathF.PI / 2f), 150f, 150f, false, RampWallTextureType.Stones1));

        // Subi baja
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 2f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 22f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 42f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 62f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 82f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 102f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 122f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 142f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(10f, -46.7f + 162f, 712f), XnaVector3.Left, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(20f, -46.7f + 182f, 712f), XnaVector3.Forward, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-10f, -46.7f + 222f, 712f), XnaVector3.Right, 15f, 15f, 1f, 0.2f, true));
        KinematicFloors.Add(new KinematicFloor(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
            new XnaVector3(-20f, -46.7f + 242f, 712f), XnaVector3.Backward, 15f, 15f, 1f, 0.2f, true));

        // Cajas estaticas
        float size = 10f;
        float spacingX = 12f;
        float spacingY = size + 0.1f;
        float baseFloorY = -46.5f;
        float z = 800f;

        int cantidadMaxima = 11;
        int cantidadActual = cantidadMaxima;
        int filaY = 0;

        while (cantidadActual >= 1)
        {
            for (int subFila = 0; subFila < 2; subFila++) // 3 filas por nivel
            {
                // Centrar la fila en X
                float totalAncho = (cantidadActual - 1) * spacingX;
                float startX = -totalAncho / 2f;

                for (int i = 0; i < cantidadActual; i++)
                {
                    float x = startX + i * spacingX;
                    float y = baseFloorY + filaY * spacingY + size / 2f;

                    StaticBoxes.Add(new StaticBox(ModelManager, EffectManager, PhysicsManager, TextureManager, GraphicsDevice,
                        new XnaVector3(x, y, z), XnaQuaternion.Identity, size));
                }

                filaY++;
            }

            cantidadActual--;
        }

        // Checkpoints
        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0, -46.5f + 2f, 580f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue, 3));

        Checkpoints.Add(new Checkpoint(ModelManager, EffectManager, PhysicsManager, GraphicsDevice, AudioManager,
            new XnaVector3(0f, 175f, 800f), XnaQuaternion.Identity, 1f, 1f, 1f, Color.Blue, 4));
    }

    protected override void UnloadContent()
    {
        Content.Unload();
        base.UnloadContent();
    }

    // Método para pausar/reanudar el timer
    public void ToggleTimer()
    {
        _isGameActive = !_isGameActive;
    }

    // Método para reiniciar el timer
    public void ResetTimer()
    {
        _gameTimer = 0f;
    }
}