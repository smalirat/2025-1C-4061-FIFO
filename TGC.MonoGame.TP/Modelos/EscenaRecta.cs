using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Modelos
{
    internal class EscenaRecta
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model Model { get; set; }
        private Effect Effect { get; set; }

        private Color[] Colors { get; set; }

        public EscenaRecta(ContentManager content)
        {
            Model = content.Load<Model>(ContentFolder3D + "skybox/cube");
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");

            Colors = new Color[20] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange,
                                     Color.Purple, Color.Pink, Color.Gold, Color.Magenta, Color.Turquoise,
                                     Color.Brown, Color.Gray, Color.DarkGray, Color.LightGray, Color.White,
                                     Color.Black, Color.CornflowerBlue, Color.Silver, Color.Indigo, Color.Maroon};

            foreach (var mesh in Model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                    meshPart.Effect = Effect;
            }
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
        }

        public void Draw(Matrix world, Matrix view, Matrix projection, Vector3 offset)
        {
            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            //Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            for (int largo = 0; largo < Colors.Length; largo++)
            {
                if (largo != 0)
                    offset.Z += 4;

                DrawModelBoxes(Model, modelMeshesBaseTransforms, 1, 9, 2, offset, Colors[largo]);
            }
        }

        private void DrawModelBoxes(Model model, Matrix[] baseTransforms, int rows, int cols, float spacing, Vector3 offset, Color color)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Posición ordenada en filas
                    var position = new Vector3(col * spacing, row * spacing, 0) + offset;
                    var worldMatrix = Matrix.CreateScale(2) * Matrix.CreateTranslation(position);

                    // Color aleatorio
                    Effect.Parameters["DiffuseColor"].SetValue(color.ToVector3());

                    foreach (var mesh in model.Meshes)
                    {
                        var relativeTransform = baseTransforms[mesh.ParentBone.Index];
                        Effect.Parameters["World"].SetValue(relativeTransform * worldMatrix);
                        mesh.Draw();
                    }
                }
            }
        }
    }
}