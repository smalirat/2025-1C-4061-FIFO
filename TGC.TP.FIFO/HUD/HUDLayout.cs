using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.HUD;

public class HUDLayout
{
    private readonly FontsManager fontsManager;
    private readonly GraphicsDevice graphicsDevice;
    private readonly SpriteBatch spriteBatch;

    private Texture2D _progressBarTexture;
    private Vector2 _timerPosition;
    private Vector2 _progressBarPosition;
    private Vector2 _progressBarSize = new Vector2(350, 40);
    private Vector2 _minimapPosition;

    private const int MINIMAP_SIZE = 150;
    private static readonly Color BarBackground = new Color(0, 0, 0, 128);
    private static readonly Color BarFill = Color.Green;
    private static readonly Color MinimapBackground = Color.Black * 0.7f;

    private const float MIN_X = -75f, MAX_X = 75f, MIN_Z = -75f, MAX_Z = 900f;

    public HUDLayout(FontsManager fontsManager, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        this.fontsManager = fontsManager;
        this.spriteBatch = spriteBatch;
        this.graphicsDevice = graphicsDevice;

        _timerPosition = new Vector2(graphicsDevice.Viewport.Width / 2f, 50);
        _progressBarPosition = new Vector2(graphicsDevice.Viewport.Width - _progressBarSize.X - 20, graphicsDevice.Viewport.Height - _progressBarSize.Y - 20);
        _minimapPosition = new Vector2(20, graphicsDevice.Viewport.Height - MINIMAP_SIZE - 20);
    }

    public void LoadContent(ContentManager content)
    {
        _progressBarTexture = new Texture2D(graphicsDevice, 1, 1);
        _progressBarTexture.SetData(new[] { Color.White });
    }

    public void Draw(PlayerBall playerBall, Matrix view, Matrix projection, List<Checkpoint> checkpoints)
    {
        var originalDepthStencilState = graphicsDevice.DepthStencilState;
        var originalBlendState = graphicsDevice.BlendState;

        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.BlendState = BlendState.AlphaBlend;

        DrawTimer();
        DrawProgressBar();
        DrawMinimap(playerBall, checkpoints);
        DrawEndGameMessage();

        graphicsDevice.DepthStencilState = originalDepthStencilState;
        graphicsDevice.BlendState = originalBlendState;
    }

    private void DrawTimer()
    {
        spriteBatch.Begin();

        string time = GameState.Cronometer.Elapsed.ToString("mm\\:ss\\.ff");
        var font = fontsManager.LucidaConsole14;
        Vector2 size = font.MeasureString(time);
        Vector2 pos = new Vector2(_timerPosition.X - size.X / 2f, _timerPosition.Y);

        Color timerColor = Color.White;

        if (GameState.Won)
        {
            timerColor = Color.LimeGreen;
        }
        else if (GameState.Lost)
        {
            timerColor = Color.Red;
        }
        else
        {
            double elapsed = GameState.Cronometer.Elapsed.TotalSeconds;
            double total = GameState.TotalSecondsBeforeLosing;
            double threshold = GameState.TotalSecondsBeforeAboutToLose;

            if (total - elapsed <= threshold)
            {
                bool flash = ((int)(elapsed * 2)) % 2 == 0;
                timerColor = flash ? Color.Red : Color.White;
            }
        }

        spriteBatch.Draw(_progressBarTexture, new Rectangle((int)(pos.X - 60), (int)(pos.Y - 20), (int)(size.X + 120), (int)(size.Y + 30)), BarBackground);
        spriteBatch.DrawString(font, time, pos + size / 2f, timerColor, 0f, size / 2f, 2f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private void DrawProgressBar()
    {
        spriteBatch.Begin();

        float progress = (float)GameState.TotalCheckpointsChecked / GameState.TotalCheckpoints;
        Color fillColor = GameState.Lost ? Color.Red : BarFill;

        spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, BarBackground, 0f, Vector2.Zero, _progressBarSize, SpriteEffects.None, 0f);
        spriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, fillColor, 0f, Vector2.Zero, new Vector2(_progressBarSize.X * progress, _progressBarSize.Y), SpriteEffects.None, 0f);

        var font = fontsManager.LucidaConsole14;
        string resultText;
        Color resultColor;

        resultText = $"CHECKPOINTS {GameState.TotalCheckpointsChecked}/{GameState.TotalCheckpoints}";
        resultColor = Color.White;

        Vector2 textSize = font.MeasureString(resultText);
        Vector2 textPosition = new Vector2(_progressBarPosition.X + _progressBarSize.X / 2f - textSize.X / 2f, _progressBarPosition.Y - textSize.Y - 5);
        spriteBatch.DrawString(font, resultText, textPosition, resultColor);

        spriteBatch.End();
    }

    private void DrawMinimap(PlayerBall playerBall, List<Checkpoint> checkpoints)
    {
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        spriteBatch.Draw(_progressBarTexture, new Rectangle((int)_minimapPosition.X, (int)_minimapPosition.Y, MINIMAP_SIZE, MINIMAP_SIZE), MinimapBackground);

        foreach (var checkpoint in checkpoints)
        {
            Vector2 pos = GetMinimapPosition(checkpoint.Position);
            Color color = checkpoint.Checked ? Color.LightGreen : Color.Yellow;
            spriteBatch.Draw(_progressBarTexture, new Rectangle((int)(pos.X - 4), (int)(pos.Y - 4), 8, 8), color);
        }

        Vector2 ballPos = GetMinimapPosition(playerBall.Position);
        spriteBatch.Draw(_progressBarTexture, new Rectangle((int)(ballPos.X - 3), (int)(ballPos.Y - 3), 6, 6), Color.Red);

        spriteBatch.End();
    }

    private void DrawEndGameMessage()
    {
        if (!GameState.Won && !GameState.Lost)
            return;

        spriteBatch.Begin();

        string message = GameState.Won ? "GANASTE" : "PERDISTE";
        var font = fontsManager.LucidaConsole14;
        Vector2 size = font.MeasureString(message) * 4f;
        Vector2 center = new Vector2(graphicsDevice.Viewport.Width / 2f, graphicsDevice.Viewport.Height / 2f);
        Vector2 pos = center - size / 2f;

        Rectangle backgroundRect = new Rectangle((int)pos.X - 20, (int)pos.Y - 20, (int)size.X + 40, (int)size.Y + 40);
        spriteBatch.Draw(_progressBarTexture, backgroundRect, MinimapBackground);

        Color color = GameState.Won ? Color.Gold : Color.Red;
        spriteBatch.DrawString(font, message, center, color, 0f, font.MeasureString(message) / 2f, 4f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private Vector2 GetMinimapPosition(Vector3 worldPosition)
    {
        float normX = (worldPosition.X - MIN_X) / (MAX_X - MIN_X);
        float normZ = (worldPosition.Z - MIN_Z) / (MAX_Z - MIN_Z);
        return _minimapPosition + new Vector2(normX * MINIMAP_SIZE, MINIMAP_SIZE - normZ * MINIMAP_SIZE);
    }
}