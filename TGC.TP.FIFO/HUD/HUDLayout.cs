using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.HUD;

public class HUDLayout
{
    private readonly FontsManager fontsManager;
    private readonly GraphicsDevice graphicsDevice;
    private readonly EffectManager effectManager;
    private readonly TextureManager textureManager;
    private readonly SpriteBatch spriteBatch;

    // Componentes del HUD
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

    // Variables para el mensaje de checkpoint
    private bool _showCheckpointMessage;
    private float _checkpointMessageTimer;
    private const float CHECKPOINT_MESSAGE_DURATION = 2f; // Duración en segundos
    private int _lastCheckpointId;

    public HUDLayout(FontsManager fontsManager, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, EffectManager effectManager, TextureManager textureManager)
    {
        this.fontsManager = fontsManager;
        this.spriteBatch = spriteBatch;
        this.graphicsDevice = graphicsDevice;
        this.effectManager = effectManager;
        this.textureManager = textureManager;

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
        _showCheckpointMessage = false;
        _checkpointMessageTimer = 0f;
        _lastCheckpointId = 0;

        // Inicializar posiciones
        _timerPosition = new Vector2(
            graphicsDevice.Viewport.Width / 2f,
            50
        );

        _ballTypePosition = new Vector2(50, 100);
        _speedPosition = new Vector2(
            graphicsDevice.Viewport.Width - 120,
            graphicsDevice.Viewport.Height - 80
        );

        _progressBarSize = new Vector2(200, 20);
        _progressBarPosition = new Vector2(
            graphicsDevice.Viewport.Width - _progressBarSize.X - 50,
            100
        );

        _arrowPosition = new Vector3(
            graphicsDevice.Viewport.Width - 100,
            graphicsDevice.Viewport.Height - 100,
            0
        );

        // Inicializar posición del minimapa
        _minimapPosition = new Vector2(20, graphicsDevice.Viewport.Height - MINIMAP_SIZE - 20);
    }

    public void LoadContent(ContentManager content)
    {
        _ballTypeRubberTexture = content.Load<Texture2D>("Textures/Rubber");
        _ballTypeMetalTexture = content.Load<Texture2D>("Textures/harsh-metal/color");
        _ballTypeStoneTexture = content.Load<Texture2D>("Textures/marble/color");
        _arrowModel = content.Load<Model>("Models/hud/3D_Arrow");
        _speedometerTexture = content.Load<Texture2D>("Textures/speedometer");
        _speedArrowTexture = content.Load<Texture2D>("Textures/speedArrow");


        // Crear textura para la barra de progreso
        _progressBarTexture = new Texture2D(graphicsDevice, 1, 1);
        _progressBarTexture.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime, int currentCheckpointId, int totalCheckpoints, Vector3 nextCheckpointPosition, Vector3 cameraPosition)
    {
        if (_isGameActive)
        {
            _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Verificar si se alcanzó un nuevo checkpoint
        if (currentCheckpointId > _lastCheckpointId)
        {
            _showCheckpointMessage = true;
            _checkpointMessageTimer = 0f;
            _lastCheckpointId = currentCheckpointId;
        }

        // Actualizar el timer del mensaje
        if (_showCheckpointMessage)
        {
            _checkpointMessageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_checkpointMessageTimer >= CHECKPOINT_MESSAGE_DURATION)
            {
                _showCheckpointMessage = false;
            }
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
        var originalDepthStencilState = graphicsDevice.DepthStencilState;
        var originalBlendState = graphicsDevice.BlendState;

        // Configurar el estado para el HUD 2D
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.BlendState = BlendState.AlphaBlend;

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

        // Dibujar el mensaje de checkpoint si está activo
        if (_showCheckpointMessage)
        {
            DrawCheckpointMessage();
        }

        // Restaurar el estado original
        graphicsDevice.DepthStencilState = originalDepthStencilState;
        graphicsDevice.BlendState = originalBlendState;

        // Dibujar la flecha
        //DrawArrow(view, projection);
    }

    private void DrawBallType(PlayerBall playerBall)
    {
        Texture2D ballTexture = playerBall.BallType switch
        {
            BallType.Goma => _ballTypeRubberTexture,
            BallType.Metal => _ballTypeMetalTexture,
            BallType.Piedra => _ballTypeStoneTexture,
            _ => _ballTypeRubberTexture
        };

        // Posición y tamaño del círculo
        int circleSize = 64; // Aumentado el tamaño para mejor visibilidad
        Vector2 texturePosition = new Vector2(_ballTypePosition.X, _ballTypePosition.Y);
        Rectangle destinationRect = new Rectangle((int)texturePosition.X, (int)texturePosition.Y, circleSize, circleSize);

        // Dibujar el fondo del círculo
        spriteBatch.Begin(effect: effectManager.BasicShader);
        spriteBatch.Draw(_progressBarTexture, destinationRect, new Color(0, 0, 0, 128));
        spriteBatch.End();

        // Dibujar la textura de la bola
        spriteBatch.Begin(effect: effectManager.CircleShader, blendState: BlendState.AlphaBlend);
        spriteBatch.Draw(ballTexture, destinationRect, Color.White);
        spriteBatch.End();
    }

    private void DrawTimer()
    {
        spriteBatch.Begin();

        int minutes = (int)(_gameTimer / 60);
        int seconds = (int)(_gameTimer % 60);
        int milliseconds = (int)((_gameTimer * 1000) % 1000);
        string timerText = $"{minutes}:{seconds:D2}:{milliseconds:D3}";

        Vector2 textSize = fontsManager.LucidaConsole14.MeasureString(timerText);

        Vector2 textPosition = new Vector2(
            _timerPosition.X - textSize.X / 2f,
            _timerPosition.Y
        );

        Rectangle backgroundRect = new Rectangle(
            (int)(textPosition.X - 60),
            (int)(textPosition.Y - 20),
            (int)(textSize.X + 120),
            (int)(textSize.Y + 30)
        );

        spriteBatch.Draw(_progressBarTexture, backgroundRect, new Color(0, 0, 0, 128));

        // Dibujar el texto con escala más grande
        float scale = 2.0f; // Hacer el texto 2 veces más grande
        Vector2 origin = textSize / 2f;
        spriteBatch.DrawString(fontsManager.LucidaConsole14, timerText, textPosition + origin, _timerColor, 0f, origin, scale, SpriteEffects.None, 0f);
        spriteBatch.End();
    }

    private void DrawSpeed(PlayerBall playerBall)
    {
        spriteBatch.Begin();

        /* // Dibujar el velocímetro
        float speedometerScale = 0.04f;
        Vector2 speedometerOrigin = new Vector2(_speedometerTexture.Width / 2f, _speedometerTexture.Height / 2f);
        spriteBatch.Draw(_speedometerTexture, _speedPosition, null, Color.White, 0f, speedometerOrigin, speedometerScale, SpriteEffects.None, 0f);

        // Dibujar la flecha fija
        Vector2 arrowOrigin = new Vector2(_speedArrowTexture.Width / 2f, _speedArrowTexture.Height);
        Vector2 arrowPosition = _speedPosition + new Vector2(0, -2);
        spriteBatch.Draw(_speedArrowTexture, arrowPosition, null, Color.White, 0f, arrowOrigin, speedometerScale, SpriteEffects.None, 0f); */

        // Dibujar el texto de la velocidad
        string speedText = $"{playerBall.GetCurrentSpeed():F1}";
        Vector2 textPosition = _speedPosition + new Vector2(0, 15);
        spriteBatch.DrawString(fontsManager.LucidaConsole14, speedText, textPosition, Color.White);

        spriteBatch.End();
    }

    private void DrawProgressBar()
    {
        spriteBatch.Begin();
        float progress = (float)_currentCheckpointId / _totalCheckpoints;
        spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, _progressBarBackgroundColor, 0f, Vector2.Zero, _progressBarSize, SpriteEffects.None, 0f);
        spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, _progressBarFillColor, 0f, Vector2.Zero, new Vector2(_progressBarSize.X * progress, _progressBarSize.Y), SpriteEffects.None, 0f);
        spriteBatch.End();
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
                effect: effectManager.BasicShader,
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
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Dibuja el fondo del minimapa
        spriteBatch.Draw(_progressBarTexture,
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

            spriteBatch.Draw(_progressBarTexture,
                new Rectangle((int)(checkMinimapPos.X - 4), (int)(checkMinimapPos.Y - 4), 8, 8),
                color);
        }

        // Pelota en el minimapa
        Vector3 ballPos = playerBall.Position;
        float normX = (ballPos.X - MIN_X) / (MAX_X - MIN_X);
        float normZ = (ballPos.Z - MIN_Z) / (MAX_Z - MIN_Z);
        Vector2 ballMinimapPos = _minimapPosition + new Vector2((1 - normX) * MINIMAP_SIZE, MINIMAP_SIZE - normZ * MINIMAP_SIZE);
        int ballDotSize = 6;
        spriteBatch.Draw(_progressBarTexture,
            new Rectangle((int)(ballMinimapPos.X - ballDotSize / 2), (int)(ballMinimapPos.Y - ballDotSize / 2), ballDotSize, ballDotSize),
            Color.Red);

        spriteBatch.End();
    }

    private void DrawCheckpointMessage()
    {
        spriteBatch.Begin();
        string message = "Checkpoint!";
        Vector2 messageSize = fontsManager.ComicSans24.MeasureString(message);
        Vector2 messagePosition = new Vector2(
            graphicsDevice.Viewport.Width / 2f - messageSize.X / 2f,
            graphicsDevice.Viewport.Height / 3f
        );

        // Calcular la opacidad basada en el tiempo restante
        float opacity = 1f - (_checkpointMessageTimer / CHECKPOINT_MESSAGE_DURATION);
        Color messageColor = new Color(1f, 1f, 1f, opacity);

        spriteBatch.DrawString(fontsManager.ComicSans24, message, messagePosition, messageColor);
        spriteBatch.End();
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