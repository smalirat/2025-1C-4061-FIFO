using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Objetos;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.HUD;

public class HUDLayout
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly EffectManager _effectManager;
    private readonly TextureManager _textureManager;

    // Componentes del HUD
    private SpriteFont _font;
    private SpriteBatch _spriteBatch;
    private float _gameTimer;
    private bool _isGameActive;
    private Vector2 _timerPosition;
    private Color _timerColor;

    // Texturas de tipos de bola
    private Texture2D _ballTypeRubberTexture;
    private Texture2D _ballTypeMetalTexture;
    private Texture2D _ballTypeStoneTexture;
    private Vector2 _ballTypePosition;

    // Componentes de velocidad
    private Vector2 _speedPosition;
    private Texture2D _speedometerTexture;
    private Texture2D _speedArrowTexture;
    private const float MAX_SPEED = 100f; // Velocidad máxima para la rotación de la flecha
    private const float MIN_SPEED_ANGLE = -120f; // Ángulo mínimo de la flecha (en grados)
    private const float MAX_SPEED_ANGLE = 120f; // Ángulo máximo de la flecha (en grados)

    // Componentes de progreso
    private int _currentCheckpointId;
    private int _totalCheckpoints;
    private Vector2 _progressBarPosition;
    private Vector2 _progressBarSize;
    private Color _progressBarBackgroundColor;
    private Color _progressBarFillColor;
    private Texture2D _progressBarTexture;

    // Componentes de la flecha
    private Model _arrowModel;
    private Vector3 _arrowPosition;
    private float _arrowScale;
    private Matrix _arrowRotation;
    private float _arrowFloatOffset;
    private const float ARROW_FLOAT_SPEED = 1f;
    private const float ARROW_FLOAT_AMPLITUDE = 0.2f;

    // Componentes del minimapa
    private const int MINIMAP_SIZE = 150;
    private Vector2 _minimapPosition;
    private const float MIN_X = -75f;
    private const float MAX_X = 75f;
    private const float MIN_Z = -75f;
    private const float MAX_Z = 900f;

    public HUDLayout(GraphicsDevice graphicsDevice, EffectManager effectManager, TextureManager textureManager)
    {
        _graphicsDevice = graphicsDevice;
        _effectManager = effectManager;
        _textureManager = textureManager;
        _spriteBatch = new SpriteBatch(graphicsDevice);

        InitializeHUD();
    }

    private void InitializeHUD()
    {
        _gameTimer = 0f;
        _isGameActive = true;
        _timerColor = Color.White;
        _progressBarBackgroundColor = new Color(0, 0, 0, 128);
        _progressBarFillColor = Color.Green;
        _arrowScale = 0.5f;
        _arrowRotation = Matrix.Identity;
        _arrowFloatOffset = 0f;

        // Inicializar posiciones
        _timerPosition = new Vector2(
            _graphicsDevice.Viewport.Width - 150,
            50
        );

        _ballTypePosition = new Vector2(50, 100);
        _speedPosition = new Vector2(
            _graphicsDevice.Viewport.Width - 120,
            _graphicsDevice.Viewport.Height - 80
        );

        _progressBarSize = new Vector2(200, 20);
        _progressBarPosition = new Vector2(
            _graphicsDevice.Viewport.Width - _progressBarSize.X - 50,
            100
        );

        _arrowPosition = new Vector3(
            _graphicsDevice.Viewport.Width - 100,
            _graphicsDevice.Viewport.Height - 100,
            0
        );

        // Inicializar posición del minimapa
        _minimapPosition = new Vector2(20, _graphicsDevice.Viewport.Height - MINIMAP_SIZE - 20);
    }

    public void LoadContent(ContentManager content)
    {
        _font = content.Load<SpriteFont>("Fonts/TimerFont");
        _ballTypeRubberTexture = content.Load<Texture2D>("Textures/Rubber");
        _ballTypeMetalTexture = content.Load<Texture2D>("Textures/harsh-metal/color");
        _ballTypeStoneTexture = content.Load<Texture2D>("Textures/marble/color");
        _arrowModel = content.Load<Model>("Models/hud/3D_Arrow");
        _speedometerTexture = content.Load<Texture2D>("Textures/speedometer");
        _speedArrowTexture = content.Load<Texture2D>("Textures/speedArrow");

        // Crear textura para la barra de progreso
        _progressBarTexture = new Texture2D(_graphicsDevice, 1, 1);
        _progressBarTexture.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime, int currentCheckpointId, int totalCheckpoints, Vector3 nextCheckpointPosition, Vector3 cameraPosition)
    {
        if (_isGameActive)
        {
            _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _currentCheckpointId = currentCheckpointId;
        _totalCheckpoints = totalCheckpoints;

        _arrowFloatOffset = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * ARROW_FLOAT_SPEED) * ARROW_FLOAT_AMPLITUDE;

        if (_currentCheckpointId < _totalCheckpoints)
        {
            Vector3 direction = nextCheckpointPosition - cameraPosition;
            direction.Normalize();

            float yaw = (float)Math.Atan2(direction.X, direction.Z);
            float pitch = (float)Math.Asin(direction.Y);

            Matrix rotationMatrix = Matrix.CreateRotationY(yaw) * Matrix.CreateRotationX(pitch);
            _arrowRotation = rotationMatrix;
        }
    }

    public void Draw(PlayerBall playerBall, Matrix view, Matrix projection, List<Checkpoint> checkpoints)
    {
        // Guardar el estado original
        var originalDepthStencilState = _graphicsDevice.DepthStencilState;
        var originalBlendState = _graphicsDevice.BlendState;

        // Configurar el estado para el HUD 2D
        _graphicsDevice.DepthStencilState = DepthStencilState.None;
        _graphicsDevice.BlendState = BlendState.AlphaBlend;

        // Dibujar el tipo de bola
        DrawBallType(playerBall);

        // Dibujar el timer
        DrawTimer();

        // Dibujar la velocidad
        DrawSpeed(playerBall);

        // Dibujar la barra de progreso
        DrawProgressBar();

        // Dibujar el minimapa
        DrawMinimap(playerBall, checkpoints);

        // Restaurar el estado original
        _graphicsDevice.DepthStencilState = originalDepthStencilState;
        _graphicsDevice.BlendState = originalBlendState;

        // Dibujar la flecha
        //DrawArrow(view, projection);
    }

    private void DrawBallType(PlayerBall playerBall)
    {
        Texture2D ballTexture = playerBall.BallType switch
        {
            BallType.Rubber => _ballTypeRubberTexture,
            BallType.Metal => _ballTypeMetalTexture,
            BallType.Stone => _ballTypeStoneTexture,
            _ => _ballTypeRubberTexture
        };

        // Posición y tamaño del círculo
        int circleSize = 32;
        Vector2 texturePosition = new Vector2(_ballTypePosition.X, _ballTypePosition.Y + 30);
        Rectangle destinationRect = new Rectangle((int)texturePosition.X, (int)texturePosition.Y, circleSize, circleSize);

        _spriteBatch.Begin(effect: _effectManager.CircleShader);
        _spriteBatch.Draw(ballTexture, destinationRect, Color.White);
        _spriteBatch.End();
    }

    private void DrawTimer()
    {
        _spriteBatch.Begin();
        string timerText = $"Tiempo: {_gameTimer:F2}";
        _spriteBatch.DrawString(_font, timerText, _timerPosition, _timerColor);
        _spriteBatch.End();
    }

    private void DrawSpeed(PlayerBall playerBall)
    {
        _spriteBatch.Begin();

        /* // Dibujar el velocímetro
        float speedometerScale = 0.04f;
        Vector2 speedometerOrigin = new Vector2(_speedometerTexture.Width / 2f, _speedometerTexture.Height / 2f);
        _spriteBatch.Draw(_speedometerTexture, _speedPosition, null, Color.White, 0f, speedometerOrigin, speedometerScale, SpriteEffects.None, 0f);

        // Dibujar la flecha fija
        Vector2 arrowOrigin = new Vector2(_speedArrowTexture.Width / 2f, _speedArrowTexture.Height);
        Vector2 arrowPosition = _speedPosition + new Vector2(0, -2);
        _spriteBatch.Draw(_speedArrowTexture, arrowPosition, null, Color.White, 0f, arrowOrigin, speedometerScale, SpriteEffects.None, 0f); */

        // Dibujar el texto de la velocidad
        string speedText = $"{playerBall.GetCurrentSpeed():F1}";
        Vector2 textPosition = _speedPosition + new Vector2(0, 15);
        _spriteBatch.DrawString(_font, speedText, textPosition, Color.White);

        _spriteBatch.End();
    }

    private void DrawProgressBar()
    {
        _spriteBatch.Begin();
        float progress = (float)_currentCheckpointId / _totalCheckpoints;
        _spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, _progressBarBackgroundColor, 0f, Vector2.Zero, _progressBarSize, SpriteEffects.None, 0f);
        _spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, _progressBarFillColor, 0f, Vector2.Zero, new Vector2(_progressBarSize.X * progress, _progressBarSize.Y), SpriteEffects.None, 0f);
        _spriteBatch.End();
    }

    private void DrawArrow(Matrix view, Matrix projection)
    {
        if (_currentCheckpointId < _totalCheckpoints)
        {
            var translation = Matrix.CreateTranslation(_arrowPosition + new Vector3(0, _arrowFloatOffset, 0));
            var scale = Matrix.CreateScale(_arrowScale);
            var rotation = _arrowRotation;

            // Obtener la textura del modelo
            Texture2D arrowTexture = null;
            if (_arrowModel.Meshes.Count > 0 &&
                _arrowModel.Meshes[0].MeshParts.Count > 0 &&
                _arrowModel.Meshes[0].MeshParts[0].Effect is BasicEffect basicEffect)
            {
                arrowTexture = basicEffect.Texture;
            }

            DrawUtilities.DrawCustomModel(
                model: _arrowModel,
                effect: _effectManager.BasicShader,
                view: view,
                projection: projection,
                translation: translation,
                scale: scale,
                rotation: rotation,
                color: Color.White,
                texture: arrowTexture
            );
        }
    }

    private void DrawMinimap(PlayerBall playerBall, List<Checkpoint> checkpoints)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Dibuja el fondo del minimapa
        _spriteBatch.Draw(_progressBarTexture,
            new Rectangle((int)_minimapPosition.X, (int)_minimapPosition.Y, MINIMAP_SIZE, MINIMAP_SIZE),
            Color.Black * 0.7f);

        // Checkpoints en el minimapa
        foreach (var checkpoint in checkpoints)
        {
            Vector3 check = checkpoint.Position;
            float checkNormX = (check.X - MIN_X) / (MAX_X - MIN_X);
            float checkNormZ = (check.Z - MIN_Z) / (MAX_Z - MIN_Z);
            Vector2 checkMinimapPos = _minimapPosition + new Vector2(checkNormX * MINIMAP_SIZE, MINIMAP_SIZE - checkNormZ * MINIMAP_SIZE);

            // Si el checkpoint está activado, lo dibuja en verde, si no en amarillo
            Color color = checkpoint.Id <= _currentCheckpointId ? Color.LimeGreen : Color.Yellow;

            _spriteBatch.Draw(_progressBarTexture,
                new Rectangle((int)(checkMinimapPos.X - 4), (int)(checkMinimapPos.Y - 4), 8, 8),
                color);
        }

        // Pelota en el minimapa
        Vector3 ballPos = playerBall.Position;
        float normX = (ballPos.X - MIN_X) / (MAX_X - MIN_X);
        float normZ = (ballPos.Z - MIN_Z) / (MAX_Z - MIN_Z);
        Vector2 ballMinimapPos = _minimapPosition + new Vector2((1 - normX) * MINIMAP_SIZE, MINIMAP_SIZE - normZ * MINIMAP_SIZE);
        int ballDotSize = 6;
        _spriteBatch.Draw(_progressBarTexture,
            new Rectangle((int)(ballMinimapPos.X - ballDotSize / 2), (int)(ballMinimapPos.Y - ballDotSize / 2), ballDotSize, ballDotSize),
            Color.Red);

        _spriteBatch.End();
    }

    public void ToggleTimer()
    {
        _isGameActive = !_isGameActive;
    }

    public void ResetTimer()
    {
        _gameTimer = 0f;
    }
}