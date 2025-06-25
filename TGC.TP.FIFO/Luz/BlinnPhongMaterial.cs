using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Luz
{
    public class BlinnPhongMaterial
    {
        public Vector3 AmbientColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }
        public float Shininess { get; set; }
        public float KAmbient { get; set; }
        public float KDiffuse { get; set; }
        public float KSpecular { get; set; }
    }
}
