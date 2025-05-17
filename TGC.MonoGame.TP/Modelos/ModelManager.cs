using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Modelos.Primitivas;

namespace TGC.MonoGame.TP.Modelos;

public class ModelManager
{
    public const string ContentFolderModels = "Models/";

    public Model SphereModel { get; private set; }
    public Model SkyBoxCubeModel { get; private set; }
    public Model LigthingModel { get; private set; }
    public Model ArrowModel { get; private set; }
    public Model FlagModel { get; private set; }

    public void Load(ContentManager content)
    {
        SphereModel = LoadModel(content, "sphere");
        SkyBoxCubeModel = LoadModel(content, "cube");
        LigthingModel = LoadModel(content, "ligthing");
        ArrowModel = LoadModel(content, "arrow");
        FlagModel = LoadModel(content, "flag");
    }

    private Model LoadModel(ContentManager content, string path)
    {
        return content.Load<Model>(ContentFolderModels + path);
    }

    public BoxPrimitive CreateBox(GraphicsDevice graphicsDevice, float height, float width, float length)
    {
        return new BoxPrimitive(graphicsDevice, new XnaVector3(width, height, length));
    }

    public SpherePrimitive CreateSphere(GraphicsDevice graphicsDevice, float diameter)
    {
        return new SpherePrimitive(graphicsDevice, diameter);
    }

    public QuadPrimitive CreateQuad(GraphicsDevice graphicsDevice)
    {
        return new QuadPrimitive(graphicsDevice);
    }
    public CylinderPrimitive CreateCylinder(GraphicsDevice graphicsDevice, float height, float radius, int tessellation)
    {
        return new CylinderPrimitive(graphicsDevice, height, radius, tessellation);
    }
}