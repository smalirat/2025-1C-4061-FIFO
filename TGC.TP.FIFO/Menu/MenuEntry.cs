using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.TP.FIFO.Menu;

public interface MenuEntry
{
    Action<Keys> Action { get; }

    void Draw(SpriteBatch spriteBatch, SpriteFont textFont, Color textColor, Vector2 screenTextPosition);
}