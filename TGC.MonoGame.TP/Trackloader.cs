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
    }
}