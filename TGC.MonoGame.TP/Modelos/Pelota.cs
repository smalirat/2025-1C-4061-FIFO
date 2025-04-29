using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Content.Models
{
    class Pelota
    {
        public const string ContentFolder3D = "Models/";
        public const string ContentFolderEffects = "Effects/";

        private Model Model { get; set; }
        private Effect Effect { get; set; }
        private float rotationSpeed = 0.8f;
        private float Rotacion { get; set; }
        private float speed = 50f;
        private Vector3 Posicion { get; set; } = Vector3.Zero;
        
        public Pelota(ContentManager content)
        {
            Model = content.Load<Model>(ContentFolder3D + "marble/marble_high");
            Effect = content.Load<Effect>(ContentFolderEffects + "BasicShader");

            foreach (var mesh in Model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                    meshPart.Effect = Effect;
            }
            
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.J))
                Rotacion += rotationSpeed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.L))
                Rotacion -= rotationSpeed * deltaTime;

            var forward = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(Rotacion));

            if (keyboardState.IsKeyDown(Keys.I))
                Posicion -= forward * speed * deltaTime;
            if (keyboardState.IsKeyDown(Keys.K))
                Posicion += forward * speed * deltaTime;
        }

        public void Draw(Matrix world, Matrix view, Matrix projection)
        {            
            Matrix matrizPelotita = Matrix.CreateRotationY(Rotacion) * Matrix.CreateTranslation(Posicion);
            Matrix origen = Matrix.Identity;

            Effect.Parameters["View"].SetValue(view);
            Effect.Parameters["Projection"].SetValue(projection);
            Effect.Parameters["DiffuseColor"].SetValue(Color.Red.ToVector3());

            var modelMeshesBaseTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);

            foreach (var mesh in Model.Meshes)
            {

                var relativeTransform = modelMeshesBaseTransforms[mesh.ParentBone.Index];
                Effect.Parameters["World"].SetValue(relativeTransform * matrizPelotita * origen * Matrix.CreateTranslation(8,-16,72));
                mesh.Draw();
            }
        }
    }
}