using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class StaticTree
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly StaticHandle boundingVolume;

    private float width;
    private float height;
    private Color color;
    private Quaternion rotation;
    private Vector3 position;

    public StaticTree(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        Vector3 position,
        Quaternion rotation,
        float height,
        float width,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.height = height;
        this.width = width;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        boundingVolume = this.physicsManager.AddStaticCylinder(height * 2f, width * 2f, position, rotation);
    }

    public void Draw(Matrix view, Matrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.NormalTreeModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: Matrix.CreateScale(width / 2f, height / 2f, width / 2f),
            rotation: Matrix.CreateFromQuaternion(rotation), 
            color: color);
    }
}
