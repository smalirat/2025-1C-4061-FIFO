using Microsoft.Xna.Framework;

public static class MaterialPresets
{
    public static readonly BlinnPhongMaterial Madera = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.25f, 0.18f, 0.1f),
        DiffuseColor = new Vector3(0.6f, 0.4f, 0.2f),
        SpecularColor = new Vector3(0.2f, 0.2f, 0.1f),
        KAmbient = 0.4f,
        KDiffuse = 0.7f,
        KSpecular = 0.2f,
        Shininess = 12.0f
    };

    public static readonly BlinnPhongMaterial Piedra = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.3f, 0.2f, 0.2f),
        DiffuseColor = new Vector3(0.6f, 0.3f, 0.3f),
        SpecularColor = new Vector3(0.2f, 0.2f, 0.2f),
        KAmbient = 0.4f,
        KDiffuse = 0.7f,
        KSpecular = 0.1f,
        Shininess = 8.0f
    };

    public static readonly BlinnPhongMaterial Tierra = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.2f, 0.15f, 0.1f),
        DiffuseColor = new Vector3(0.4f, 0.3f, 0.2f),
        SpecularColor = new Vector3(0.1f, 0.1f, 0.1f),
        KAmbient = 0.5f,
        KDiffuse = 0.6f,
        KSpecular = 0.05f,
        Shininess = 4.0f
    };

    public static readonly BlinnPhongMaterial Goma = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.3f, 0.3f, 0.3f),   // subido desde 0.1
        DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f),   // subido desde 0.3
        SpecularColor = new Vector3(0.8f, 0.8f, 0.8f),  // mantenido
        KAmbient = 0.6f,    // subido desde 0.4
        KDiffuse = 1.0f,    // subido desde 0.5 (máxima difusión)
        KSpecular = 0.9f,   // mantenido
        Shininess = 64.0f   // mantenido
    };

    public static readonly BlinnPhongMaterial Metal = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.25f, 0.25f, 0.25f),
        DiffuseColor = new Vector3(0.4f, 0.4f, 0.4f),
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
        KAmbient = 0.3f,
        KDiffuse = 0.6f,
        KSpecular = 1.0f,
        Shininess = 128.0f
    };

    public static readonly BlinnPhongMaterial PowerUp = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.3f, 0.3f, 0.3f),
        DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f),
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
        KAmbient = 0.3f,
        KDiffuse = 0.7f,
        KSpecular = 0.8f,
        Shininess = 64.0f
    };

    public static readonly BlinnPhongMaterial Checkpoint = new BlinnPhongMaterial
    {
        AmbientColor = new Vector3(0.3f, 0.3f, 0.3f),
        DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f),
        SpecularColor = new Vector3(1.0f, 1.0f, 1.0f),
        KAmbient = 0.3f,
        KDiffuse = 0.7f,
        KSpecular = 0.8f,
        Shininess = 64.0f
    };
}
