using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Texturas;

namespace TGC.MonoGame.TP.Skybox;

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

        this.TipoSkybox = tipoSkybox;
    }

    public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition, GraphicsDevice graphicsDevice)
    {
        TextureCube texture = null;

        switch (TipoSkybox)
        {
            case TiposSkybox.Pasto:
                texture = this.textureManager.AlpsSkyBoxTexture;
                break;

            case TiposSkybox.Mar:
                texture = this.textureManager.DarlingSkyBoxTexture;
                break;

            case TiposSkybox.Nieve:
                texture = this.textureManager.IceSkyBoxTexture;
                break;

            case TiposSkybox.Roca:
                texture = this.textureManager.MountainSkyBoxTexture;
                break;
        }

        var originalState = graphicsDevice.RasterizerState;

        graphicsDevice.RasterizerState = NoCullState;

        foreach (var pass in this.effectManager.SkyBoxShader.CurrentTechnique.Passes)
        {
            pass.Apply();

            foreach (var mesh in this.modelManager.SkyBoxCubeModel.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    part.Effect = this.effectManager.SkyBoxShader;
                    part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(Size) * Matrix.CreateTranslation(cameraPosition));
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