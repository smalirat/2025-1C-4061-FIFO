using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Skybox;

public class SimpleSkyBox
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly TextureManager textureManager;

    private readonly BoxPrimitive model;

    private const float Size = 2500f;

    public SimpleSkyBox(ModelManager modelManager,
        EffectManager effectManager,
        TextureManager textureManager,
        GraphicsDevice graphicsDevice)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.textureManager = textureManager;

        model = this.modelManager.CreateBox(graphicsDevice, Size, Size, Size);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 cameraPosition, GraphicsDevice graphicsDevice)
    {
        var originalState = graphicsDevice.RasterizerState;
        graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };

        var effect = effectManager.SkyBoxShader;
        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(XnaMatrix.CreateTranslation(cameraPosition));
        effect.Parameters["ModelTexture"].SetValue(textureManager.MountainSkyBoxTexture);
        model.Draw(effect);

        graphicsDevice.RasterizerState = originalState;
    }
}