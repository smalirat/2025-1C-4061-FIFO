using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Efectos;

public static class EffectManager
{
    public const string ContentFolderEffects = "Effects/";

    public static Effect BasicShader { get; private set; }
    public static Effect BasicTextureShader { get; private set; }
    public static Effect TextureTilingShader { get; private set; }
    public static Effect SkyBoxShader { get; private set; }
    public static Effect BlinnPhongShader { get; private set; }
    public static Effect BasicGlowShader { get; private set; }
    public static XnaVector3 LightPosition { get; set; }

    public static void Load(ContentManager content)
    {
        BasicShader = content.Load<Effect>(ContentFolderEffects + "BasicShader");
        BasicTextureShader = content.Load<Effect>(ContentFolderEffects + "BasicTextureShader");
        TextureTilingShader = content.Load<Effect>(ContentFolderEffects + "TextureTilingShader");
        SkyBoxShader = content.Load<Effect>(ContentFolderEffects + "SkyBoxShader");
        BlinnPhongShader = content.Load<Effect>(ContentFolderEffects + "BlinnPhongShader");
        BasicGlowShader = content.Load<Effect>(ContentFolderEffects + "BasicGlowShader");

        // Se puede refactorizar en una clase, o un metodo con otro nombre,
        LightPosition = new XnaVector3(0f, 100f, 480f);
    }
}