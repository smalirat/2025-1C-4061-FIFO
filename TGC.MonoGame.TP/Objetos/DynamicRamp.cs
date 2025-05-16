using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class DynamicRamp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private const float ModelHeight = 2f;
    private const float ModelWidth = 2f;
    private const float ModelLength = 2f;

    private readonly BodyHandle boundingVolume;

    private readonly float width;
    private const float Height = 1.25f;
    private readonly float length;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private float XScale => width / ModelWidth;
    private float YScale => Height / ModelHeight;
    private float ZScale => length / ModelLength;

    public BodyType BodyType => BodyType.Other;

    public BodyHandle Handle => boundingVolume;

    public DynamicRamp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float length,
        Color color,
        float mass = 10f,      // por defecto 10, podés ajustar
        float friction = 1f)   // por defecto 1
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.width = width;
        this.length = length;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        boundingVolume = this.physicsManager.AddDynamicBox(width, Height, length, mass, friction, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.BoxModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(XScale, YScale, ZScale),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }

    public void Update(float deltaTime, TargetCamera camera)
    {
        // Sincronizar la posición y rotación con la física
        position = physicsManager.GetPosition(boundingVolume);
        rotation = physicsManager.GetOrientation(boundingVolume);
    }

    public void NotifyCollition(IColisionable with)
    {
        // Opcional: lógica de colisiones
    }
}
