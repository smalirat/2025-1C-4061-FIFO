using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Modelos
{
    public class Nivel2
    {
        private readonly ModelManager modelManager;
        private readonly EffectManager effectManager;

        // Semilla para lograr aleatoreidad deterministica
        // Es decir, random, pero siempre el mismo random
        private const int RANDOM_SEED = 0;
        private Random random;

        private Color[] treeColors =
        [
            Color.Green,
            Color.GreenYellow,
            Color.DarkGreen,
            Color.DarkOliveGreen,
            Color.DarkSeaGreen,
            Color.ForestGreen,
            Color.OliveDrab,
            Color.SeaGreen,
            Color.LawnGreen,
            Color.MediumSeaGreen
        ];

        private Color[] rockColors =
        [
            Color.Gray,
            Color.DarkGray,
            Color.DimGray,
            Color.DarkSlateGray,
            Color.Black,
            Color.SlateGray,
            Color.LightSlateGray
        ];

        private Matrix[] rockScales =
        [
            Matrix.CreateScale(1.2f, 0.8f, 1.0f),
            Matrix.CreateScale(0.9f, 1.0f, 1.1f),
            Matrix.CreateScale(1.5f, 1.3f, 1.4f),
            Matrix.CreateScale(0.8f, 0.7f, 0.9f),
            Matrix.CreateScale(1.3f, 1.0f, 0.9f),
            Matrix.CreateScale(1.8f, 1.6f, 1.5f),
            Matrix.CreateScale(0.6f, 0.5f, 0.7f),
            Matrix.CreateScale(1.1f, 0.9f, 1.6f),
            Matrix.CreateScale(1.4f, 1.2f, 1.2f),
            Matrix.CreateScale(0.75f, 0.8f, 0.6f),
            Matrix.CreateScale(2.0f, 1.7f, 1.9f),
            Matrix.CreateScale(1.0f, 1.0f, 1.0f),
            Matrix.CreateScale(1.6f, 1.2f, 1.3f),
            Matrix.CreateScale(0.7f, 0.9f, 1.2f),
            Matrix.CreateScale(1.9f, 1.4f, 1.6f)
        ];

        public Nivel2(ModelManager modelManager, EffectManager effectManager)
        {
            this.modelManager = modelManager;
            this.effectManager = effectManager;
        }

        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            random = new Random(RANDOM_SEED);

            // Posicion y rotacion global del nivel
            Matrix globalOffset = Matrix.CreateTranslation(200f, 0f, 0f);
            Matrix globalRotation = Matrix.CreateRotationZ(0f);

            // Dibujamos rampas rectas con caida hacia abajo
            var rampDownPositions = new List<Vector3>
            {
                new Vector3(0f, 460f, 300f),
                new Vector3(0f, 410f, 270f),
                new Vector3(0f, 360f, 230f),
                new Vector3(0f, 310f, 200f),
                new Vector3(0f, 260f, 170f),
                new Vector3(0f, 210f, 130f),
                new Vector3(0f, 160f, 100f),
                new Vector3(0f, 110f, 70f),
                new Vector3(0f, 60f, 40f),
                new Vector3(0f, 10f, 10f)
            };

            foreach (var rampDownPosition in rampDownPositions)
            {
                DrawUtilities.DrawModel(modelManager.SlantLongDModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(rampDownPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1.5f, zScale: 2f),
                    color: Color.Yellow,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujo el "piso"
            var floorY = -40f;

            DrawUtilities.DrawModel(modelManager.BoxModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -200f)),
                scale: Matrix.CreateScale(xScale: 50f, yScale: 1f, zScale: 500f),
                color: Color.SandyBrown,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos las rectas
            var straightPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -30f),
                new Vector3(0f, floorY, -90f),
                new Vector3(-10f, floorY, -350f),
                new Vector3(10f, floorY, -350f),
                new Vector3(0f, floorY, -450f),
                new Vector3(0f, floorY, -510f),
                new Vector3(0f, floorY, -570f),
                new Vector3(0f, floorY, -630f)
            };

            foreach (var straightPosition in straightPositions)
            {
                DrawUtilities.DrawModel(modelManager.StraightModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(straightPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 6f),
                    color: Color.Violet,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las divisiones
            var splitPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -140f)
            };

            foreach (var splitPosition in splitPositions)
            {
                DrawUtilities.DrawModel(modelManager.SplitDoubleModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(splitPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    rotation: Matrix.CreateRotationY(MathHelper.Pi),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las conjunciones
            var conjuctionPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -400f)
            };

            foreach (var conjuctionPosition in conjuctionPositions)
            {
                DrawUtilities.DrawModel(modelManager.SplitDoubleModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(conjuctionPosition),
                    scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 2f),
                    color: Color.Red,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja
            var upDownRampPositions = new List<Vector3>
            {
                new Vector3(10f, floorY, -220f),
                new Vector3(10f, floorY, -260f),
                new Vector3(10f, floorY, -300f),
                new Vector3(10f, floorY, -180f)
            };

            foreach (var upDownRampPosition in upDownRampPositions)
            {
                DrawUtilities.DrawModel(modelManager.BumpDModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(upDownRampPosition),
                    color: Color.Black,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rampas subi baja leves
            var slowUpDownRampPositions = new List<Vector3>
            {
                new Vector3(-10f, floorY, -220f),
                new Vector3(-10f, floorY, -260f),
                new Vector3(-10f, floorY, -300f),
                new Vector3(-10f, floorY, -180f),
            };

            foreach (var slowUpDownRampPosition in slowUpDownRampPositions)
            {
                DrawUtilities.DrawModel(modelManager.BumpAModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(slowUpDownRampPosition),
                    color: Color.White,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos los tuneles
            var tunnelPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -530f),
            };

            foreach (var tunnelPosition in tunnelPositions)
            {
                DrawUtilities.DrawModel(modelManager.TunnelModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(tunnelPosition),
                    scale: Matrix.CreateScale(xScale: 3f, yScale: 5f, zScale: 16f),
                    color: Color.Teal,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las banderas
            var bannerPositions = new List<Vector3>
            {
                new Vector3(0f, floorY, -200f),
                new Vector3(0f, floorY, -100f),
                new Vector3(0f, floorY, -300f),
                new Vector3(0f, floorY, -400f),
                new Vector3(0f, floorY, -680f)
            };

            foreach (var bannerPosition in bannerPositions)
            {
                DrawUtilities.DrawModel(modelManager.BannerHighModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(bannerPosition),
                    scale: Matrix.CreateScale(xScale: 10f, yScale: 7f, zScale: 1f),
                    color: Color.DarkBlue,
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos el final
            DrawUtilities.DrawModel(modelManager.EndSquareModel,
                effectManager.BasicShader,
                view,
                projection,
                translation: Matrix.CreateTranslation(new Vector3(0f, floorY, -665f)),
                scale: Matrix.CreateScale(xScale: 1f, yScale: 1f, zScale: 1f),
                rotation: Matrix.CreateRotationY(MathHelper.Pi),
                color: Color.Red,
                globalOffset: globalOffset,
                globalRotation: globalRotation);

            // Dibujamos los arboles


            var normalTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -320f),
                new Vector3(40f, floorY, -80f),
                new Vector3(40f, floorY, 220f),
                new Vector3(40f, floorY, 280f),
                new Vector3(40f, floorY, -440f),
                new Vector3(40f, floorY, -20f),
                new Vector3(40f, floorY, -560f),

                // Izquierda
                new Vector3(-40f, floorY, 220f),
                new Vector3(-40f, floorY, -560f),
                new Vector3(-40f, floorY, -320f),
                new Vector3(-40f, floorY, -80f),
                new Vector3(-40f, floorY, 280f),
                new Vector3(-40f, floorY, -440f),
                new Vector3(-40f, floorY, -20f),
                new Vector3(-40f, floorY, -180f),

                // Parte sin rampas ("random")
                new Vector3(-14.6f, floorY, 125.7f),
                new Vector3(37.4f, floorY, 185.8f),
                new Vector3(-9.3f, floorY, 240.9f),
                new Vector3(21.1f, floorY, 155.3f),
                new Vector3(-3.7f, floorY, 45.6f)
            };

            foreach (var normalTreePosition in normalTreePositions)
            {
                DrawUtilities.DrawModel(modelManager.NormalTreeModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(normalTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            var tallTreePositions = new List<Vector3>
            {
                // Derecha
                new Vector3(40f, floorY, -500f),
                new Vector3(40f, floorY, 100f),
                new Vector3(40f, floorY, -620f),
                new Vector3(40f, floorY, -140f),
                new Vector3(40f, floorY, -260f),
                new Vector3(40f, floorY, -380f),
                new Vector3(40f, floorY, 160f),
                new Vector3(40f, floorY, 40f),
                new Vector3(40f, floorY, -670f),

                // Izquierda
                new Vector3(-40f, floorY, -260f),
                new Vector3(-40f, floorY, -500f),
                new Vector3(-40f, floorY, 100f),
                new Vector3(-40f, floorY, -620f),
                new Vector3(-40f, floorY, -140f),
                new Vector3(-40f, floorY, -380f),
                new Vector3(-40f, floorY, 160f),
                new Vector3(-40f, floorY, 40f),

                // Parte sin rampas (random)
                new Vector3(-12.8f, floorY, 56.3f),
                new Vector3(24.9f, floorY, 95.2f),
                new Vector3(8.1f, floorY, 145.6f),
                new Vector3(-35.4f, floorY, 120.4f),
                new Vector3(29.6f, floorY, 200.8f),
                new Vector3(16.3f, floorY, 215.5f),
                new Vector3(33.7f, floorY, 170.2f),
                new Vector3(-27.2f, floorY, 205.3f),
                new Vector3(3.0f, floorY, 160.1f),
                new Vector3(12.5f, floorY, 230.4f),
                new Vector3(-5.9f, floorY, 100.7f),
                new Vector3(20.2f, floorY, 130.6f),
                new Vector3(-31.3f, floorY, 190.2f),
                new Vector3(6.8f, floorY, 250.1f),
                new Vector3(11.7f, floorY, 70.3f)
            };

            foreach (var tallTreePosition in tallTreePositions)
            {
                DrawUtilities.DrawModel(modelManager.TallTreeModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(tallTreePosition),
                    scale: Matrix.CreateScale(2f),
                    color: treeColors[random.Next(treeColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }

            // Dibujamos las rocas
            var rockPositions = new List<Vector3>
            {
                // Derecha
                new Vector3(27.6f, floorY, -695f),
                new Vector3(35.2f, floorY, -670f),
                new Vector3(45.1f, floorY, -660f),
                new Vector3(28.3f, floorY, -640f),
                new Vector3(37.4f, floorY, -620f),
                new Vector3(42.3f, floorY, -600f),
                new Vector3(30.1f, floorY, -580f),
                new Vector3(46.7f, floorY, -560f),
                new Vector3(29.5f, floorY, -550f),
                new Vector3(38.2f, floorY, -520f),
                new Vector3(44.5f, floorY, -500f),
                new Vector3(26.9f, floorY, -480f),
                new Vector3(41.0f, floorY, -460f),
                new Vector3(34.6f, floorY, -440f),
                new Vector3(47.0f, floorY, -420f),
                new Vector3(32.8f, floorY, 0f),
                new Vector3(39.5f, floorY, -380f),
                new Vector3(36.4f, floorY, -360f),
                new Vector3(27.2f, floorY, -340f),
                new Vector3(43.8f, floorY, -310f),
                new Vector3(33.9f, floorY, -280f),
                new Vector3(46.3f, floorY, -250f),
                new Vector3(31.3f, floorY, -230f),
                new Vector3(36.0f, floorY, -210f),
                new Vector3(28.9f, floorY, -180f),
                new Vector3(40.7f, floorY, -150f),
                new Vector3(47.2f, floorY, -120f),
                new Vector3(31.7f, floorY, -90f),
                new Vector3(42.1f, floorY, -60f),
                new Vector3(29.3f, floorY, -30f),
                new Vector3(44.1f, floorY, 0f),
                new Vector3(37.9f, floorY, 40f),
                new Vector3(32.6f, floorY, 70f),
                new Vector3(43.4f, floorY, 100f),
                new Vector3(30.2f, floorY, 140f),
                new Vector3(45.6f, floorY, 170f),
                new Vector3(33.8f, floorY, 200f),
                new Vector3(39.2f, floorY, 230f),
                new Vector3(25.7f, floorY, 260f),
                new Vector3(47.0f, floorY, 280f),

                // Izquierda
                new Vector3(-25.5f, floorY, -695f),
                new Vector3(-30.3f, floorY, -670f),
                new Vector3(-35.0f, floorY, -660f),
                new Vector3(-26.9f, floorY, -640f),
                new Vector3(-28.4f, floorY, -620f),
                new Vector3(-37.2f, floorY, -600f),
                new Vector3(-29.7f, floorY, -580f),
                new Vector3(-33.1f, floorY, -560f),
                new Vector3(-38.4f, floorY, -550f),
                new Vector3(-32.1f, floorY, -520f),
                new Vector3(-26.8f, floorY, -500f),
                new Vector3(-30.2f, floorY, -480f),
                new Vector3(-34.5f, floorY, -460f),
                new Vector3(-31.8f, floorY, -440f),
                new Vector3(-29.3f, floorY, -420f),
                new Vector3(-38.0f, floorY, 00f),
                new Vector3(-27.7f, floorY, -380f),
                new Vector3(-33.0f, floorY, -360f),
                new Vector3(-37.3f, floorY, -340f),
                new Vector3(-32.2f, floorY, -310f),
                new Vector3(-34.8f, floorY, -280f),
                new Vector3(-39.5f, floorY, -250f),
                new Vector3(-28.1f, floorY, -230f),
                new Vector3(-31.6f, floorY, -210f),
                new Vector3(-35.9f, floorY, -180f),
                new Vector3(-29.4f, floorY, -150f),
                new Vector3(-32.3f, floorY, -120f),
                new Vector3(-34.0f, floorY, -90f),
                new Vector3(-38.6f, floorY, -60f),
                new Vector3(-26.7f, floorY, -30f),
                new Vector3(-36.3f, floorY, 0f),
                new Vector3(-27.9f, floorY, 40f),
                new Vector3(-29.5f, floorY, 70f),
                new Vector3(-35.7f, floorY, 100f),
                new Vector3(-28.0f, floorY, 140f),
                new Vector3(-39.8f, floorY, 170f),
                new Vector3(-30.9f, floorY, 200f),
                new Vector3(-32.0f, floorY, 230f),
                new Vector3(-31.4f, floorY, 260f),
                new Vector3(-26.0f, floorY, 280f),

                // Parte sin rampas ("random")
                new Vector3(-38.2f, floorY, 45.7f),
                new Vector3(25.6f, floorY, 60.1f),
                new Vector3(10.3f, floorY, 120.4f),
                new Vector3(33.7f, floorY, 110.9f),
                new Vector3(-29.4f, floorY, 95.2f),
                new Vector3(-18.1f, floorY, 210.3f),
                new Vector3(39.8f, floorY, 178.5f),
                new Vector3(-4.9f, floorY, 245.6f),
                new Vector3(19.5f, floorY, 170.7f),
                new Vector3(12.6f, floorY, 250.1f),
                new Vector3(-25.3f, floorY, 185.4f),
                new Vector3(-10.8f, floorY, 150.2f),
                new Vector3(35.4f, floorY, 222.1f),
                new Vector3(-32.5f, floorY, 129.6f),
                new Vector3(2.2f, floorY, 204.8f),
                new Vector3(6.1f, floorY, 280f),
                new Vector3(20.8f, floorY, 230.9f),
                new Vector3(38.1f, floorY, 140.3f),
                new Vector3(-15.6f, floorY, 160.1f),
                new Vector3(28.4f, floorY, 80.7f),
                new Vector3(0.7f, floorY, 50.6f),
                new Vector3(-27.9f, floorY, 230.2f),
                new Vector3(33.1f, floorY, 120.3f),
                new Vector3(-1.4f, floorY, 95.4f),
                new Vector3(37.2f, floorY, 190.4f),
                new Vector3(5.0f, floorY, 115.2f),
                new Vector3(-36.9f, floorY, 120.5f),
                new Vector3(29.7f, floorY, 170.3f),
                new Vector3(-8.3f, floorY, 140.9f),
                new Vector3(1.9f, floorY, 135.6f),
                new Vector3(-19.2f, floorY, 245.9f),
                new Vector3(4.3f, floorY, 180.7f),
                new Vector3(-2.5f, floorY, 200.8f),
                new Vector3(16.0f, floorY, 220.1f),
                new Vector3(36.8f, floorY, 110.6f),
                new Vector3(-5.6f, floorY, 70.3f),
                new Vector3(3.2f, floorY, 120.6f),
                new Vector3(22.3f, floorY, 180.2f),
                new Vector3(-13.9f, floorY, 190.1f),
                new Vector3(31.2f, floorY, 100.5f),
                new Vector3(15.4f, floorY, 120.2f)
            };

            foreach (var rockPosition in rockPositions)
            {
                DrawUtilities.DrawModel(modelManager.SupportModel,
                    effectManager.BasicShader,
                    view,
                    projection,
                    translation: Matrix.CreateTranslation(rockPosition),
                    scale: rockScales[random.Next(rockScales.Length)],
                    color: rockColors[random.Next(rockColors.Length)],
                    globalOffset: globalOffset,
                    globalRotation: globalRotation);
            }
        }
    }
}
