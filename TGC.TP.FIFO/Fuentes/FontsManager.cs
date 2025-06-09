using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Fuentes;

public class FontsManager
{
    public const string ContentFolderFonts = "Fonts/";

    public SpriteFont LucidaConsole14 { get; private set; }
    public SpriteFont ComicSans24 { get; private set; }

    public void Load(ContentManager content)
    {
        LucidaConsole14 = LoadFont(content, "lucida-console-14");
        ComicSans24 = LoadFont(content, "comic-sans-24");
    }

    private SpriteFont LoadFont(ContentManager content, string path)
    {
        return content.Load<SpriteFont>(ContentFolderFonts + path);
    }
}