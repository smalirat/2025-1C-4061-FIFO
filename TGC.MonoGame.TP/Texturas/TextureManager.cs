using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Texturas;

public class TextureManager
{
    public const string ContentFolderTextures = "Textures/";

    public Texture2D RubberTexture { get; private set; }

    public void Load(ContentManager content)
    {
        RubberTexture = LoadTexture(content, "rubber");
    }

    private Texture2D LoadTexture(ContentManager content, string path)
    {
        return content.Load<Texture2D>(ContentFolderTextures + path);
    }
}
