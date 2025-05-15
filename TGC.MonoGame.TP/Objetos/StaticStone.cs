using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class StaticStone
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private const float ModelHeight = 5f;
    private const float ModelRadius = 3f;

    private readonly StaticHandle boundingVolume;

    private readonly float height;
    private readonly float radius;

    private Color color;
    private Quaternion rotation;
    private Vector3 position;

    private float XScale => radius / ModelRadius;
    private float YScale => height / ModelHeight;
    private float ZScale => radius / ModelRadius;

    public StaticStone(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        Vector3 position,
        Quaternion rotation,
        float height,
        float radius,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.height = height;
        this.radius = radius;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        boundingVolume = this.physicsManager.AddStaticCylinder(height, radius, position, rotation);
    }

    public void Draw(Matrix view, Matrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.SupportModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }
}
