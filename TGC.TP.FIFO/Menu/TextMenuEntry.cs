using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public class TextMenuEntry : MenuEntry
{
    private readonly string text;
    public Action<Keys> Action { get; private set; }

    public TextMenuEntry(string text, Action action)
    {
        this.text = text;
        this.Action = (keyPressed) =>
        {
            if (keyPressed != Keys.Enter) return;
            action.Invoke();
        };
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont textFont, Color textColor, XnaVector2 screenTextPosition)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(textFont, text, screenTextPosition, textColor);
        spriteBatch.End();
    }
}