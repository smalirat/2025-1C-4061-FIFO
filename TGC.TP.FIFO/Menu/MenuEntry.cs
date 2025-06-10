using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public interface MenuEntry
{
    Vector2 Size { get; }
    string Label { get; }
    Action<Keys> Action { get; }
    SpriteFont Font { get; }

    void Draw(SpriteBatch spriteBatch, Color textColor, Vector2 screenTextPosition);
}