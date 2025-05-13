using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace TGC.MonoGame.TP.Skybox
{

    public class SimpleSkyBox
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";
        public const string ContentFolderTextures = "Textures/";

        public Model Model { get; set; }
        private TextureCube Texture { get; set; }
        private Effect Effect { get; set; }

        private float _size = 2500f;

        private readonly RasterizerState NoCullState = new RasterizerState { CullMode = CullMode.None };

        public SimpleSkyBox()
        {
            //_cameraTarget = Vector3.Zero;
            //_view = Matrix.CreateLookAt(Vector3.UnitX * 20, _cameraTarget, Vector3.UnitY);
            //_projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 100f);
        }

        public void LoadContent(ContentManager content, TiposSkybox tipoSkybox)
        {
            Model = content.Load<Model>(ContentFolder3D + "skybox/cube");

            switch (tipoSkybox)
            {
                case TiposSkybox.Pasto:
                    Texture = content.Load<TextureCube>(ContentFolderTextures + "alpsSkybox");
                    break;

                case TiposSkybox.Mar:
                    Texture = content.Load<TextureCube>(ContentFolderTextures + "darlingSkybox");
                    break;

                case TiposSkybox.Nieve:
                    Texture = content.Load<TextureCube>(ContentFolderTextures + "iceSkybox");
                    break;

                case TiposSkybox.Roca:
                    Texture = content.Load<TextureCube>(ContentFolderTextures + "mountainSkybox");
                    break;
            }

            Effect = content.Load<Effect>(ContentFolderEffects + "SkyBox");
        }

        public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition, GraphicsDevice graphicsDevice)
        {
            var originalState = graphicsDevice.RasterizerState;

            graphicsDevice.RasterizerState = NoCullState;

            // Go through each pass in the effect, but we know there is only one...
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                foreach (var mesh in Model.Meshes)
                {

                    foreach (var part in mesh.MeshParts)
                    {
                        part.Effect = Effect;
                        part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(_size) * Matrix.CreateTranslation(cameraPosition));
                        part.Effect.Parameters["View"].SetValue(view);
                        part.Effect.Parameters["Projection"].SetValue(projection);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(Texture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                    }

                    mesh.Draw();
                }
            }

            graphicsDevice.RasterizerState = originalState;
        }

    }
}