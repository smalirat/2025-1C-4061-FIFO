using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.HUDS;

public class HUD
{
    private Texture2D _progressBarTexture;
    private XnaVector2 _timerPosition;
    private XnaVector2 _progressBarPosition;
    private XnaVector2 _progressBarSize = new XnaVector2(350, 40);
    private XnaVector2 _minimapPosition;

    private const int MINIMAP_SIZE = 150;
    private static readonly Color BarBackground = new Color(0, 0, 0, 128);
    private static readonly Color BarFill = Color.Green;
    private static readonly Color MinimapBackground = Color.Black * 0.7f;

    private const float MIN_X = -75f, MAX_X = 75f, MIN_Z = -75f, MAX_Z = 900f;

    private List<Checkpoint> checkpoints = [];

    public HUD()
    {
        _timerPosition = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width / 2f, 50);
        _progressBarPosition = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width - _progressBarSize.X - 20, GameGlobals.GraphicsDevice.Viewport.Height - _progressBarSize.Y - 20);
        _minimapPosition = new XnaVector2(20, GameGlobals.GraphicsDevice.Viewport.Height - MINIMAP_SIZE - 20);
    }

    public void LoadContent(ContentManager content)
    {
        _progressBarTexture = new Texture2D(GameGlobals.GraphicsDevice, 1, 1);
        _progressBarTexture.SetData(new[] { Color.White });
    }

    public void Draw(PlayerBall playerBall)
    {
        var originalDepthStencilState = GameGlobals.GraphicsDevice.DepthStencilState;
        var originalBlendState = GameGlobals.GraphicsDevice.BlendState;

        GameGlobals.GraphicsDevice.DepthStencilState = DepthStencilState.None;
        GameGlobals.GraphicsDevice.BlendState = BlendState.AlphaBlend;

        DrawTimer();
        DrawProgressBar();
        DrawMinimap(playerBall);
        DrawEndGameMessage();

        GameGlobals.GraphicsDevice.DepthStencilState = originalDepthStencilState;
        GameGlobals.GraphicsDevice.BlendState = originalBlendState;
    }

    private void DrawTimer()
    {
        GameGlobals.SpriteBatch.Begin();

        string time = GameState.Cronometer.Elapsed.ToString("mm\\:ss\\.ff");
        var font = FontsManager.LucidaConsole40;
        XnaVector2 size = font.MeasureString(time);
        XnaVector2 pos = new XnaVector2(_timerPosition.X - size.X / 2f, _timerPosition.Y);

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
                bool flash = (int)(elapsed * 2) % 2 == 0;
                timerColor = flash ? Color.Red : Color.White;
            }
        }

        int paddingX = 10;
        int paddingY = 10;
        int rectX = (int)(pos.X - paddingX);
        int rectY = (int)(pos.Y - paddingY);
        int rectWidth = (int)(size.X + 2 * paddingX);
        int rectHeight = (int)(size.Y + 2 * paddingY);

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle(rectX, rectY, rectWidth, rectHeight), BarBackground);

        GameGlobals.SpriteBatch.DrawString(font, time, pos, timerColor);

        GameGlobals.SpriteBatch.End();
    }

    private void DrawProgressBar()
    {
        GameGlobals.SpriteBatch.Begin();

        float progress = (float)GameState.TotalCheckpointsChecked / GameState.TotalCheckpoints;
        Color fillColor = GameState.Lost ? Color.Red : BarFill;

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, BarBackground, 0f, XnaVector2.Zero, _progressBarSize, SpriteEffects.None, 0f);
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, fillColor, 0f, XnaVector2.Zero, new XnaVector2(_progressBarSize.X * progress, _progressBarSize.Y), SpriteEffects.None, 0f);

        var font = FontsManager.LucidaConsole20;
        string resultText;
        Color resultColor;

        resultText = $"CHECKPOINTS {GameState.TotalCheckpointsChecked}/{GameState.TotalCheckpoints}";
        resultColor = Color.White;

        XnaVector2 textSize = font.MeasureString(resultText);
        XnaVector2 textPosition = new XnaVector2(_progressBarPosition.X + _progressBarSize.X / 2f - textSize.X / 2f, _progressBarPosition.Y - textSize.Y - 5);
        GameGlobals.SpriteBatch.DrawString(font, resultText, textPosition, resultColor);

        GameGlobals.SpriteBatch.End();
    }

    private void DrawMinimap(PlayerBall playerBall)
    {
        GameGlobals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)_minimapPosition.X, (int)_minimapPosition.Y, MINIMAP_SIZE, MINIMAP_SIZE), MinimapBackground);

        foreach (var checkpoint in checkpoints)
        {
            XnaVector2 pos = GetMinimapPosition(checkpoint.Position);
            Color color = checkpoint.Checked ? Color.LightGreen : Color.Yellow;
            GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)(pos.X - 4), (int)(pos.Y - 4), 8, 8), color);
        }

        XnaVector2 ballPos = GetMinimapPosition(playerBall.Position);
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)(ballPos.X - 3), (int)(ballPos.Y - 3), 6, 6), Color.Red);

        GameGlobals.SpriteBatch.End();
    }

    private void DrawEndGameMessage()
    {
        if (!GameState.Won && !GameState.Lost)
            return;

        GameGlobals.SpriteBatch.Begin();

        string message = GameState.Won ? "GANASTE" : "PERDISTE";
        var font = FontsManager.LucidaConsole60;

        XnaVector2 textSize = font.MeasureString(message);
        XnaVector2 screenCenter = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width / 2f, GameGlobals.GraphicsDevice.Viewport.Height / 2f);
        XnaVector2 textOrigin = textSize / 2f;

        int paddingX = 30;
        int paddingY = 20;

        Rectangle backgroundRect = new Rectangle(
            (int)(screenCenter.X - textSize.X / 2f) - paddingX,
            (int)(screenCenter.Y - textSize.Y / 2f) - paddingY,
            (int)textSize.X + 2 * paddingX,
            (int)textSize.Y + 2 * paddingY
        );

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, backgroundRect, MinimapBackground);

        Color color = GameState.Won ? Color.LimeGreen : Color.Red;
        GameGlobals.SpriteBatch.DrawString(font, message, screenCenter, color, 0f, textOrigin, 1f, SpriteEffects.None, 0f);

        GameGlobals.SpriteBatch.End();
    }

    private XnaVector2 GetMinimapPosition(XnaVector3 worldPosition)
    {
        float normX = 1 - (worldPosition.X - MIN_X) / (MAX_X - MIN_X);
        float normZ = (worldPosition.Z - MIN_Z) / (MAX_Z - MIN_Z);

        var posX = normX * MINIMAP_SIZE;
        var posZ = MINIMAP_SIZE - normZ * MINIMAP_SIZE;

        return _minimapPosition + new XnaVector2(posX , posZ);
    }

    public void SetCheckpoints(List<Checkpoint> checkpoints)
    {
        this.checkpoints = checkpoints;
    } 
}