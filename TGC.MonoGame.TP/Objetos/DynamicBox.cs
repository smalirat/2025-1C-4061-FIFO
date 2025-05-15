using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class DynamicBox
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly BodyHandle boundingVolume;

    private float width;
    private float height;
    private float length;
    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    public DynamicBox(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float length,
        float height,
        float friction,
        float mass,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.width = width;
        this.height = height;
        this.length = length;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        boundingVolume = this.physicsManager.AddDynamicBox(width, height, length, mass, friction, position, rotation);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.BoxModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(width / 2f, height / 2f, length / 2f),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        // Actualizo matriz mundo
        position = physicsManager.GetPosition(boundingVolume);
        rotation = physicsManager.GetOrientation(boundingVolume);
    }
}
