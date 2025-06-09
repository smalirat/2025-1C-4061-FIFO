using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TGC.TP.FIFO.Menu;

public class SelectMenuEntry<T> : MenuEntry
{
    private readonly string text;
    private readonly List<T> options;
    private int selectedIndex;

    public Action<Keys> Action { get; private set; }

    public SelectMenuEntry(string text, List<T> options, Action<T> onOptionChanged)
    {
        this.text = text;
        this.options = options;
        this.selectedIndex = 0 % options.Count;

        this.Action = (keyPressed) =>
        {
            if (keyPressed == Keys.A)
            {
                selectedIndex = (selectedIndex - 1 + options.Count) % options.Count;
                onOptionChanged(options[selectedIndex]);
            }
            else if (keyPressed == Keys.D)
            {
                selectedIndex = (selectedIndex + 1) % options.Count;
                onOptionChanged(options[selectedIndex]);
            }
        };
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont textFont, Color textColor, Vector2 position)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(textFont, $"{text}: < {options[selectedIndex]} >", position, textColor);
        spriteBatch.End();
    }
}
