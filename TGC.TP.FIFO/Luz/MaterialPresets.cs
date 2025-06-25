using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Luz
{
    public static class MaterialPresets
    {
        public static readonly BlinnPhongMaterial Madera = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.4f, 0.25f, 0.15f),
            DiffuseColor = new Vector3(0.85f, 0.55f, 0.3f),
            SpecularColor = new Vector3(0.4f, 0.3f, 0.2f),
            KAmbient = 0.6f,
            KDiffuse = 1.0f,
            KSpecular = 0.3f,
            Shininess = 16.0f
        };

        public static readonly BlinnPhongMaterial Piedra = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.35f, 0.35f, 0.35f),
            DiffuseColor = new Vector3(0.65f, 0.65f, 0.65f),
            SpecularColor = new Vector3(0.25f, 0.25f, 0.25f),
            KAmbient = 0.5f,
            KDiffuse = 0.9f,
            KSpecular = 0.15f,
            Shininess = 8.0f
        };

        public static readonly BlinnPhongMaterial Tierra = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.3f, 0.22f, 0.15f),
            DiffuseColor = new Vector3(0.6f, 0.45f, 0.3f),
            SpecularColor = new Vector3(0.2f, 0.2f, 0.15f),
            KAmbient = 0.6f,
            KDiffuse = 0.9f,
            KSpecular = 0.15f,
            Shininess = 6.0f
        };

        public static readonly BlinnPhongMaterial Goma = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.5f, 0.5f, 0.5f),
            DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f),
            SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.7f,
            KDiffuse = 1.2f,
            KSpecular = 1.0f,
            Shininess = 64.0f
        };

        public static readonly BlinnPhongMaterial Metal = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.4f, 0.4f, 0.4f),
            DiffuseColor = new Vector3(0.75f, 0.75f, 0.75f),
            SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.6f,
            KDiffuse = 1.0f,
            KSpecular = 1.0f,
            Shininess = 128.0f
        };

        public static readonly BlinnPhongMaterial PowerUp = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.5f, 0.5f, 0.3f),
            DiffuseColor = new Vector3(1.0f, 1.0f, 0.7f),
            SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.5f,
            KDiffuse = 1.0f,
            KSpecular = 1.0f,
            Shininess = 96.0f
        };

        public static readonly BlinnPhongMaterial Checkpoint = new BlinnPhongMaterial
        {
            AmbientColor = new Vector3(0.5f, 0.5f, 0.5f),
            DiffuseColor = new Vector3(0.9f, 1.0f, 0.9f),
            SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.5f,
            KDiffuse = 1.0f,
            KSpecular = 1.0f,
            Shininess = 96.0f
        };
    }

}