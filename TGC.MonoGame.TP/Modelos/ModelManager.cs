using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Modelos.Primitivas;

namespace TGC.MonoGame.TP.Modelos;

public class ModelManager
{
    public const string ContentFolderModels = "Models/";

    public Model SphereModel { get; private set; }
    public Model Sphere2Model { get; private set; }
    public Model NormalTreeModel { get; private set; }
    public Model TallTreeModel { get; private set; }
    public Model RockModel { get; private set; }
    public Model CheckpointModel { get; private set; }
    public Model MarbleModel { get; set; }
    public Model PowerUp1Model { get; set; }
    public Model PowerUP2Model { get; set; }
    public Model SkyBoxCubeModel { get; set; }

    public void Load(ContentManager content)
    {
        SphereModel = LoadModel(content, "sphere");
        Sphere2Model = LoadModel(content, "sphere2");
        NormalTreeModel = LoadModel(content, "tree");
        TallTreeModel = LoadModel(content, "tree_tall");
        RockModel = LoadModel(content, "rock");
        CheckpointModel = LoadModel(content, "checkpoint");
        PowerUp1Model = LoadModel(content, "powerUp1");
        PowerUP2Model = LoadModel(content, "powerUp2");
        SkyBoxCubeModel = LoadModel(content, "cube");
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