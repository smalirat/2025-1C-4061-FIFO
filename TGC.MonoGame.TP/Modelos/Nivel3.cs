using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Modelos
{
    public class Nivel3
    {
        private readonly ModelManager modelManager;
        private readonly EffectManager effectManager;

        public Nivel3(ModelManager modelManager, EffectManager effectManager)
        {
            this.modelManager = modelManager;
            this.effectManager = effectManager;
        }

        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            // Posicion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(400f, 0f, 0f);

            // --- Straight #1 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- CurveLarge #1 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(10f, 0f, 20f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #2 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(30f, 0f, 30f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #1 ---
            DrawUtilities.DrawModel(modelManager.WaveAModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(55f, 0f, 28.75f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #3 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(80f, 0f, 30f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- CurveLarge #2 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(100f, 0f, 20f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveB #1 ---
            DrawUtilities.DrawModel(modelManager.WaveBModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(112.5f, 0f, -15f),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #4 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(110, 0, -40)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- WaveC #1 ---
            DrawUtilities.DrawModel(modelManager.WaveCModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(115f, 0f, -65f),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #5 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(110, 0, -90)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #3 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(120f, 0f, -110f)),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #6 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(140, 0, -120)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #2 ---
            DrawUtilities.DrawModel(modelManager.WaveAModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(165f, 0f, -121.25f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #7 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(190, 0, -120)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- SplitDoubleSides #1---
            DrawUtilities.DrawModel(modelManager.SplitDoubleSidesModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(205f, 0f, -120f),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // #### CAMINO A (IZQUIERDA) ####
            // --- Straight #8 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(210, 0, -140)),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- CurveLarge #4 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(200f, 0f, -160f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #9 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(180, 0, -170)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- Straight #10 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(170, 0, -170)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Red,
               globalOffset: globalOffset);

            // --- WaveA #3 ---
            DrawUtilities.DrawModel(modelManager.WaveAModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(145f, 0f, -171.25f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #11 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(120f, 0, -170f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- WaveA #4 ---
            DrawUtilities.DrawModel(modelManager.WaveAModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(95f, 0f, -168.75f)),
                rotation: Matrix.CreateRotationY(3 * MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #12 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(70f, 0, -170f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #13 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(60f, 0, -170f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #5 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -160f)),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #14 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(30f, 0, -140f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // #### CAMINO B (DERECHA) ####
            // --- Straight #15 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(210f, 0, -100f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #6 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(200f, 0f, -80f)),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #16 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(180f, 0, -70f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- WaveB #2 ---
            DrawUtilities.DrawModel(modelManager.WaveBModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(155f, 0f, -67.5f)),
                rotation: Matrix.CreateRotationY(3 * MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- WaveC #2 ---
            DrawUtilities.DrawModel(modelManager.WaveCModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(85f, 0f, -75f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Purple,
                globalOffset: globalOffset);

            // --- Straight #17 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(60f, 0, -70f)),
               rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
               color: Color.Blue,
               globalOffset: globalOffset);

            // --- CurveLarge #7 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -80f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Blue,
                globalOffset: globalOffset);

            // --- Straight #18 ---
            DrawUtilities.DrawModel(modelManager.StraightModel,
               effectManager.BasicShader,
               view,
               projection,
               translation: Matrix.CreateTranslation(new Vector3(30f, 0, -100f)),
               color: Color.Blue,
               globalOffset: globalOffset);

            // #### UNION DE CAMINOS ####
            // --- SplitDoubleSides #2 ---
            DrawUtilities.DrawModel(modelManager.SplitDoubleSidesModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(25f, 0f, -120f)),
                rotation: Matrix.CreateRotationY(MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);

            // --- CurveLarge #8 ---
            DrawUtilities.DrawModel(modelManager.CurveLargeModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(40f, 0f, -80f)),
                rotation: Matrix.CreateRotationY(-MathHelper.PiOver2),
                color: Color.Red,
                globalOffset: globalOffset);
        }
    }
}