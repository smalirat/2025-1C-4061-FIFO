using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Skybox;

public class SimpleSkyBox
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly TextureManager textureManager;

    private const float Size = 2500f;

    private readonly RasterizerState NoCullState = new RasterizerState { CullMode = CullMode.None };

    private readonly TiposSkybox TipoSkybox;

    public SimpleSkyBox(ModelManager modelManager,
        EffectManager effectManager,
        TextureManager textureManager,
        TiposSkybox tipoSkybox)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.textureManager = textureManager;

        TipoSkybox = tipoSkybox;
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 cameraPosition, GraphicsDevice graphicsDevice)
    {
        TextureCube texture = null;

        switch (TipoSkybox)
        {
            case TiposSkybox.Pasto:
                texture = textureManager.AlpsSkyBoxTexture;
                break;

            case TiposSkybox.Mar:
                texture = textureManager.DarlingSkyBoxTexture;
                break;

            case TiposSkybox.Nieve:
                texture = textureManager.IceSkyBoxTexture;
                break;

            case TiposSkybox.Roca:
                texture = textureManager.MountainSkyBoxTexture;
                break;
        }

        var originalState = graphicsDevice.RasterizerState;

        graphicsDevice.RasterizerState = NoCullState;

        foreach (var pass in effectManager.SkyBoxShader.CurrentTechnique.Passes)
        {
            pass.Apply();

            foreach (var mesh in modelManager.SkyBoxCubeModel.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    part.Effect = effectManager.SkyBoxShader;
                    part.Effect.Parameters["World"].SetValue(XnaMatrix.CreateScale(Size) * XnaMatrix.CreateTranslation(cameraPosition));
                    part.Effect.Parameters["View"].SetValue(view);
                    part.Effect.Parameters["Projection"].SetValue(projection);
                    part.Effect.Parameters["SkyBoxTexture"].SetValue(texture);
                    part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                }

                mesh.Draw();
            }
        }

        graphicsDevice.RasterizerState = originalState;
    }
}