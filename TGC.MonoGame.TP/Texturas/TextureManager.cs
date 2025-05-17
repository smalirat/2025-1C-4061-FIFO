using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Texturas;

public class TextureManager
{
    public const string ContentFolderTextures = "Textures/";

    public Texture2D RubberTexture { get; private set; }
    public Texture2D WoodBox1Texture { get; private set; }
    public Texture2D WoodBox2Texture { get; private set; }
    public Texture2D WoodBox3Texture { get; private set; }
    public Texture2D WoodBox4Texture { get; private set; }
    public Texture2D Stones1Texture { get; private set; }
    public Texture2D Stones2Texture { get; private set; }
    public Texture2D Stones3Texture { get; private set; }
    public Texture2D Rock1Texture { get; private set; }
    public Texture2D Rock2Texture { get; private set; }
    public Texture2D DirtTexture { get; private set; }
    public Texture2D GroundTexture { get; private set; }
    public Texture2D GrassTexture { get; private set; }
    public Texture2D SphereMetalTexture { get; private set; }
    public Texture2D SphereMarbleTexture { get; private set; }
    public TextureCube AlpsSkyBoxTexture { get; private set; }
    public TextureCube DarlingSkyBoxTexture { get; private set; }
    public TextureCube IceSkyBoxTexture { get; private set; }
    public TextureCube MountainSkyBoxTexture { get; private set; }

    public void Load(ContentManager content)
    {
        RubberTexture = LoadTexture2D(content, "rubber");
        WoodBox1Texture = LoadTexture2D(content, "caja-madera-1");
        WoodBox2Texture = LoadTexture2D(content, "caja-madera-2");
        WoodBox3Texture = LoadTexture2D(content, "caja-madera-3");
        WoodBox4Texture = LoadTexture2D(content, "caja-madera-4");
        Stones1Texture = LoadTexture2D(content, "stones");
        Stones2Texture = LoadTexture2D(content, "adoquin");
        Stones3Texture = LoadTexture2D(content, "adoquin-2");
        Rock1Texture = LoadTexture2D(content, "rock1");
        Rock2Texture = LoadTexture2D(content, "rock2");
        DirtTexture = LoadTexture2D(content, "tierra");
        GroundTexture = LoadTexture2D(content, "ground");
        GrassTexture = LoadTexture2D(content, "grass");
        SphereMetalTexture = LoadTexture2D(content, "harsh-metal/color");
        SphereMarbleTexture = LoadTexture2D(content, "marble/color");
        AlpsSkyBoxTexture = LoadTextureCube(content, "alpsSkybox");
        DarlingSkyBoxTexture = LoadTextureCube(content, "darlingSkybox");
        IceSkyBoxTexture = LoadTextureCube(content, "iceSkybox");
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