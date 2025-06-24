using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Texturas;

public class TextureManager
{
    public const string ContentFolderTextures = "Textures/";

    public Texture2D RubberTexture { get; private set; }
    public Texture2D WoodBox1Texture { get; private set; }
    public Texture2D WoodBox2Texture { get; private set; }
    public Texture2D WoodBox3Texture { get; private set; }
    public Texture2D StonesTexture { get; private set; }
    public Texture2D DirtTexture { get; private set; }
    public Texture2D SphereMetalTexture { get; private set; }
    public Texture2D SphereMarbleTexture { get; private set; }
    public TextureCube MountainSkyBoxTexture { get; private set; }

    public void Load(ContentManager content)
    {
        RubberTexture = LoadTexture2D(content, "rubber");
        WoodBox2Texture = LoadTexture2D(content, "caja-madera-2");
        WoodBox3Texture = LoadTexture2D(content, "caja-madera-3");
        WoodBox1Texture = LoadTexture2D(content, "caja-madera-4");
        StonesTexture = LoadTexture2D(content, "stones");
        DirtTexture = LoadTexture2D(content, "tierra");
        SphereMetalTexture = LoadTexture2D(content, "harsh-metal/color");
        SphereMarbleTexture = LoadTexture2D(content, "marble/color");
        MountainSkyBoxTexture = LoadTextureCube(content, "mountainSkybox");
    }

    private Texture2D LoadTexture2D(ContentManager content, string path)
    {
        return content.Load<Texture2D>(ContentFolderTextures + path);
    }

    private TextureCube LoadTextureCube(ContentManager content, string path)
    {
        return content.Load<TextureCube>(ContentFolderTextures + path);
    }
}