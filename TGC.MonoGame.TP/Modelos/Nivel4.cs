using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Modelos
{
    public class Nivel4
    {
        private readonly ModelManager modelManager;
        private readonly EffectManager effectManager;

        public Nivel4(ModelManager modelManager, EffectManager effectManager)
        {
            this.modelManager = modelManager;
            this.effectManager = effectManager;
        }

        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            // Posicion y rotacion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(1200f, 0f, 0f);
            Matrix globalRotation = Matrix.CreateRotationY(MathHelper.Pi);

            // --- Straight #1
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, 0f, 20f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #2
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, 0f, 30f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Tunel #1
            DrawUtilities.DrawModel(modelManager.TunnelModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, 5f, 20f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Tunel #2
            DrawUtilities.DrawModel(modelManager.TunnelModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, 5f, 30f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #3
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, 0f, 40f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Ramp Long D
            DrawUtilities.DrawModel(modelManager.RampLongDModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-60f, -15f, 55f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- CURVE LARGE  ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-70f, -15f, 80f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #4
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-90f, -15f, 90f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Double Split Sides
            DrawUtilities.DrawModel(modelManager.SplitDoubleSidesModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-105f, -15f, 90f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2 * -1),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Bump A (derecha)
            DrawUtilities.DrawModel(modelManager.BumpAModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -15f, 55f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #4
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -15f, 30f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #5
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -15f, 20f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #6
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -15f, 10f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Helix Large Right #1
            DrawUtilities.DrawModel(modelManager.HelixLargeRightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-135f, -35f, 5f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Helix Large Right #2
            DrawUtilities.DrawModel(modelManager.HelixLargeRightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-135f, -55f, 5f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #6
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -80f, -30f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #7
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -80f, -40f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- Straight #8
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -80f, -50f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C  ---
            DrawUtilities.DrawModel(modelManager.SlantLongCModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -90f, -65f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C #2  ---
            DrawUtilities.DrawModel(modelManager.SlantLongCModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -95f, -75f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // --- SLANT LONG C #3  ---
            DrawUtilities.DrawModel(modelManager.SlantLongCModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(-110f, -100f, -85f),
                color: Color.Purple,
                globalOffset: globalOffset,
                globalRotation: globalRotation);
        }
    }
}