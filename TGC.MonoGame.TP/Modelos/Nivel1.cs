using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Modelos
{
    public class Nivel1
    {
        private readonly ModelManager modelManager;
        private readonly EffectManager effectManager;

        public Nivel1(ModelManager modelManager, EffectManager effectManager)
        {
            this.modelManager = modelManager;
            this.effectManager = effectManager;
        }

        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            // Posicion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(0f, 0f, -0f);

            float zBasePosition = 0f;
            float yBasePosition = 0f;

            Color[] colors =
            [
                Color.Peru,
                Color.Wheat
            ];

            int colorIndex = 0;

            for (int i = 0; i < 30; i++)
            {
                // Matrices locales
                Matrix translation = Matrix.CreateTranslation(0f, yBasePosition, zBasePosition);

                DrawUtilities.DrawModel(modelManager.FunnelModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: translation,
                    color: colors[colorIndex],
                    globalOffset: globalOffset);

                // Actualización de posición
                zBasePosition += 10f;
                yBasePosition += 5f;

                // Alternancia de color
                colorIndex = (colorIndex + 1) % colors.Length;
            }
        }
    }
}
