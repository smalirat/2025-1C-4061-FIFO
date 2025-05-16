using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class StaticBox : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private const float ModelHeight = 2f;
    private const float ModelWidth = 2f;
    private const float ModelLength = 2f;

    private readonly StaticHandle boundingVolume;

    private readonly float width;
    private readonly float height;
    private readonly float length;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => length / ModelLength;

    public BodyType BodyType => BodyType.Other;

    public StaticBox(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float length,
        float height,
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

        boundingVolume = this.physicsManager.AddStaticBox(width, height, length, position, rotation, this);
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



    public void NotifyCollition(IColisionable with)
    {

    }
}
