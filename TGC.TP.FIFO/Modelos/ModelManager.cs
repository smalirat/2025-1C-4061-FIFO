using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Modelos.Primitivas;

namespace TGC.TP.FIFO.Modelos;

public static class ModelManager
{
    public const string ContentFolderModels = "Models/";

    public static Model SphereModel { get; private set; }
    public static Model LigthingModel { get; private set; }
    public static Model ArrowModel { get; private set; }
    public static Model FlagModel { get; private set; }

    public static void Load(ContentManager content)
    {
        SphereModel = LoadModel(content, "sphere");
        LigthingModel = LoadModel(content, "ligthing");
        ArrowModel = LoadModel(content, "arrow");
        FlagModel = LoadModel(content, "flag");
    }

    private static Model LoadModel(ContentManager content, string path)
    {
        return content.Load<Model>(ContentFolderModels + path);
    }

    public static BoxPrimitive CreateBox(float height, float width, float length)
    {
        return new BoxPrimitive(new XnaVector3(width, height, length));
    }
}