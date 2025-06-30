using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Skybox;

public class SkyBox
{
    private readonly BoxPrimitive model;
    private const float Size = 2500f;

    public SkyBox()
    {
        model = ModelManager.CreateBox(Size, Size, Size);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 cameraPosition, GraphicsDevice graphicsDevice)
    {
        var originalState = graphicsDevice.RasterizerState;
        graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        var effect = EffectManager.SkyBoxShader;
        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(XnaMatrix.CreateTranslation(cameraPosition));
        effect.Parameters["ModelTexture"].SetValue(TextureManager.MountainSkyBoxTexture);
        model.Draw(effect);

        graphicsDevice.RasterizerState = originalState;
    }
}