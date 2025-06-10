using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Fuentes;

public class FontsManager
{
    public const string ContentFolderFonts = "Fonts/";

    public SpriteFont LucidaConsole14 { get; private set; }
    public SpriteFont LucidaConsole20 { get; private set; }
    public SpriteFont LucidaConsole40 { get; private set; }
    public SpriteFont LucidaConsole60 { get; private set; }

    public void Load(ContentManager content)
    {
        LucidaConsole14 = LoadFont(content, "lucida-console-14");
        LucidaConsole20 = LoadFont(content, "lucida-console-20");
        LucidaConsole40 = LoadFont(content, "lucida-console-40");
        LucidaConsole60 = LoadFont(content, "lucida-console-60");
    }

    private SpriteFont LoadFont(ContentManager content, string path)
    {
        return content.Load<SpriteFont>(ContentFolderFonts + path);
    }
}