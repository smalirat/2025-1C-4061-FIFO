using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Efectos;

public class EffectManager
{
    public const string ContentFolderEffects = "Effects/";

    public Effect BasicShader { get; private set; }
    public Effect BasicTextureShader { get; private set; }
    public Effect SphereShader { get; private set; }
    public Effect TextureTilingShader { get; private set; }
    public Effect SkyBoxShader { get; private set; }
    public Effect CircleShader { get; private set; }


    public void Load(ContentManager content)
    {
        BasicShader = content.Load<Effect>(ContentFolderEffects + "BasicShader");
        SphereShader = content.Load<Effect>(ContentFolderEffects + "SphereShader");
        BasicTextureShader = content.Load<Effect>(ContentFolderEffects + "BasicTextureShader");
        TextureTilingShader = content.Load<Effect>(ContentFolderEffects + "TextureTilingShader");
        SkyBoxShader = content.Load<Effect>(ContentFolderEffects + "SkyBoxShader");
        CircleShader = content.Load<Effect>("Effects/CircleShader");

    }
}
