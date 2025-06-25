using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Efectos;

public class EffectManager
{
    public const string ContentFolderEffects = "Effects/";

    public Effect BasicShader { get; private set; }
    public Effect BasicTextureShader { get; private set; }
    public Effect TextureTilingShader { get; private set; }
    public Effect SkyBoxShader { get; private set; }
    public Effect BlinnPhongShader { get; private set; }
    public XnaVector3 LightPosition { get; set; }

    public void Load(ContentManager content)
    {
        BasicShader = content.Load<Effect>(ContentFolderEffects + "BasicShader");
        BasicTextureShader = content.Load<Effect>(ContentFolderEffects + "BasicTextureShader");
        TextureTilingShader = content.Load<Effect>(ContentFolderEffects + "TextureTilingShader");
        SkyBoxShader = content.Load<Effect>(ContentFolderEffects + "SkyBoxShader");
        BlinnPhongShader = content.Load<Effect>(ContentFolderEffects + "BlinnPhongShader");

        //Se puede refactorizar en una clase, o un metodo con otro nombre,
        LightPosition = new XnaVector3(3050f, 3100f, 3050f);

    }
}