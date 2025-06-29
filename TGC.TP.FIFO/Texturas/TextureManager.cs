﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Texturas;

public static class TextureManager
{
    public const string ContentFolderTextures = "Textures/";

    public static Texture2D RubberTexture { get; private set; }
    public static Texture2D WoodBox1Texture { get; private set; }
    public static Texture2D WoodBox2Texture { get; private set; }
    public static Texture2D WoodBox3Texture { get; private set; }
    public static Texture2D StonesTexture { get; private set; }
    public static Texture2D StonesNormalTexture { get; private set; }
    public static Texture2D DirtTexture { get; private set; }
    public static Texture2D SphereMetalTexture { get; private set; }
    public static Texture2D SphereMarbleTexture { get; private set; }
    public static TextureCube MountainSkyBoxTexture { get; private set; }

    public static void Load(ContentManager content)
    {
        RubberTexture = LoadTexture2D(content, "rubber");
        WoodBox2Texture = LoadTexture2D(content, "caja-madera-2");
        WoodBox3Texture = LoadTexture2D(content, "caja-madera-3");
        WoodBox1Texture = LoadTexture2D(content, "caja-madera-4");
        StonesTexture = LoadTexture2D(content, "stones");
        StonesNormalTexture = LoadTexture2D(content, "stones-normal");
        DirtTexture = LoadTexture2D(content, "tierra");
        SphereMetalTexture = LoadTexture2D(content, "harsh-metal/color");
        SphereMarbleTexture = LoadTexture2D(content, "marble/color");
        MountainSkyBoxTexture = LoadTextureCube(content, "mountainSkybox");
    }

    private static Texture2D LoadTexture2D(ContentManager content, string path)
    {
        return content.Load<Texture2D>(ContentFolderTextures + path);
    }

    private static TextureCube LoadTextureCube(ContentManager content, string path)
    {
        return content.Load<TextureCube>(ContentFolderTextures + path);
    }
}