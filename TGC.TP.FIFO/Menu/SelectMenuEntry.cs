using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TGC.TP.FIFO.Menu;

public class SelectMenuEntry<T> : MenuEntry
{
    public string Label { get; private set; }
    public Action<Keys> Action { get; private set; }
    public SpriteFont Font { get; private set; }

    public XnaVector2 Size => Font.MeasureString(TextToPrint());

    private readonly List<T> options;
    private int selectedIndex;

    public SelectMenuEntry(string label, List<T> options, Action<T> onOptionChanged, SpriteFont font)
    {
        Font = font;
        Label = label;
        this.options = options;
        selectedIndex = 0 % options.Count;

        Action = (keyPressed) =>
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

    public void Draw(SpriteBatch spriteBatch, Color textColor, Vector2 position)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(Font, TextToPrint().ToUpper(), position, textColor);
        spriteBatch.End();
    }

    private string TextToPrint()
    {
        return $"{Label}: < {options[selectedIndex]} >";
    }
}