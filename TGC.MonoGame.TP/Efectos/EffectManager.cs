using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Efectos;

public class EffectManager
{
    public const string ContentFolderEffects = "Effects/";

    public Effect BasicShader { get; private set; }
    public Effect PelotaShader { get; private set; }

    public void Load(ContentManager content)
    {
        BasicShader = content.Load<Effect>(ContentFolderEffects + "BasicShader");
        PelotaShader = content.Load<Effect>(ContentFolderEffects + "PelotaShader");
    }
}
