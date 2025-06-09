using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public class BarMenuEntry : MenuEntry
{
    private readonly string label;
    public Action<Keys> Action { get; private set; }
    private readonly Func<int> getValueFunc;

    private const int Min = 0;
    private const int Max = 100;

    public BarMenuEntry(string label, Func<int> getValueFunc, Action<int> setValueFunc)
    {
        this.label = label;
        this.getValueFunc = getValueFunc;
        this.Action = (keyPressed) =>
        {
            if (keyPressed == Keys.A)
            {
                setValueFunc(Math.Max(Min, getValueFunc() - 1));
                return;
            }

            if (keyPressed == Keys.D)
            {
                setValueFunc(Math.Min(Max, getValueFunc() + 1));
                return;
            }
        };
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont textFont, Color textColor, Vector2 position)
    {
        const int barWidth = 200;
        const int barHeight = 10;
        const int spacing = 10;

        int currentValue = getValueFunc();
        float percentage = (float)(currentValue - Min) / (Max - Min);
        int filledWidth = (int)(percentage * barWidth);

        string text = $"{label}:";
        Vector2 textSize = textFont.MeasureString(text);
        Vector2 barPosition = new Vector2(position.X + textSize.X + spacing, position.Y + (textSize.Y - barHeight) / 2);

        Texture2D rectTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        rectTexture.SetData(new[] { Color.White });

        spriteBatch.Begin();
        spriteBatch.DrawString(textFont, text, position, textColor);
        spriteBatch.Draw(rectTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight), Color.Gray);
        spriteBatch.Draw(rectTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, filledWidth, barHeight), Color.Green);
        spriteBatch.End();
    }
}
