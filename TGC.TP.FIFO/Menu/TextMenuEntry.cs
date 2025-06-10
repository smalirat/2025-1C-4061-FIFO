using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public class TextMenuEntry : MenuEntry
{
    public string Label { get; private set; }
    public Action<Keys> Action { get; private set; }
    public SpriteFont Font { get; private set; }
    public XnaVector2 Size => Font.MeasureString(Label);

    public TextMenuEntry(string label, Action action, SpriteFont font)
    {
        Font = font;
        Label = label;
        Action = (keyPressed) =>
        {
            if (keyPressed != Keys.Enter) return;
            action.Invoke();
        };
    }

    public void Draw(SpriteBatch spriteBatch, Color textColor, XnaVector2 screenTextPosition)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(Font, Label.ToUpper(), screenTextPosition, textColor);
        spriteBatch.End();
    }
}