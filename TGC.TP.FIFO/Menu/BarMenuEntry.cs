using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public class BarMenuEntry : MenuEntry
{
    public SpriteFont Font { get; private set; }
    public string Label { get; private set; }
    public Action<Keys> Action { get; private set; }

    private readonly Func<int> getValueFunc;

    private const int Min = 0;
    private const int Max = 100;
    private const int BarWidth = 200;
    private const int BarHeight = 10;
    private const int Spacing = 10;

    public Vector2 Size
    {
        get
        {
            string text = $"{Label}:";
            Vector2 textSize = Font.MeasureString(text);
            float width = textSize.X + Spacing + BarWidth;
            float height = Math.Max(textSize.Y, BarHeight);
            return new Vector2(width, height);
        }
    }

    public BarMenuEntry(string label, Func<int> getValueFunc, Action<int> setValueFunc, SpriteFont font)
    {
        this.getValueFunc = getValueFunc;

        Font = font;
        Label = label;
        Action = (keyPressed) =>
        {
            if (keyPressed == Keys.A)
                setValueFunc(Math.Max(Min, getValueFunc() - 5));
            else if (keyPressed == Keys.D)
                setValueFunc(Math.Min(Max, getValueFunc() + 5));
        };
    }

    public void Draw(SpriteBatch spriteBatch, Color textColor, Vector2 position)
    {
        int currentValue = getValueFunc();
        float percentage = (float)(currentValue - Min) / (Max - Min);
        int filledWidth = (int)(percentage * BarWidth);

        string text = $"{Label}:";
        Vector2 textSize = Font.MeasureString(text);
        Vector2 barPosition = new Vector2(position.X + textSize.X + Spacing, position.Y + (textSize.Y - BarHeight) / 2);

        Texture2D rectTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        rectTexture.SetData([Color.White]);

        spriteBatch.Begin();
        spriteBatch.DrawString(Font, text.ToUpper(), position, textColor);
        spriteBatch.Draw(rectTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, BarWidth, BarHeight), Color.Gray);
        spriteBatch.Draw(rectTexture, new Rectangle((int)barPosition.X, (int)barPosition.Y, filledWidth, BarHeight), Color.Green);
        spriteBatch.End();
    }
}
