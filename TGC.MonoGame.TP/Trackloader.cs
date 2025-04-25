using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP
{

    public static class TrackLoader
    {

        public static void AsignarEfecto(Model model, Effect effect)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect;
                }
            }
        }


        /*public static void CargarModelos(){
            ModelBox = Content.Load<Model>(ContentFolder3D + "skybox/cube");
            ModelMarble = Content.Load<Model>(ContentFolder3D + "marble/marble_high");
            ModelCurve = Content.Load<Model>(ContentFolder3D + "curves/curve");
            SlantLongA = Content.Load<Model>(ContentFolder3D + "slants/slant_long_A");
            BumpSolidB = Content.Load<Model>(ContentFolder3D + "bump/bump_solid_B");
            RampLongA = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_A");
            RampLongD = Content.Load<Model>(ContentFolder3D + "ramps/ramp_long_D");
            CurveLarge = Content.Load<Model>(ContentFolder3D + "curves/curve_large");
            Straight = Content.Load<Model>(ContentFolder3D + "straights/straight");
            Tunnel = Content.Load<Model>(ContentFolder3D + "extras/tunnel");

            Effect = Content.Load<Effect>(ContentFolderEffects + "BasicShader");
            } */


    }

}