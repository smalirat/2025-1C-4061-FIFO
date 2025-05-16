using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class Checkpoint : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private const float ModelHeight = 10f;
    private const float ModelWidth = 9.25f;
    private const float ModelLength = 0.75f;

    private readonly StaticHandle boundingVolume1;
    private readonly StaticHandle boundingVolume2;
    private readonly StaticHandle boundingVolume3;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    private float PostHeight => height;
    private float PostWidth => 0.3f * XScale;
    private float PostDepth => depth;

    private float CrossbarHeight => 0.3f * YScale;
    private float CrossbarWidth => width;
    private float CrossbarDepth => depth;

    public BodyType BodyType => BodyType.Checkpoint;

    public Checkpoint(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float depth,
        float height,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;

        this.width = width;
        this.height = height;
        this.depth = depth;
        this.color = color;
        this.rotation = rotation;
        this.position = position;

        var halfWidth = (ModelWidth / 2f) * XScale;

        var offsetX = Vector3.Transform(Vector3.Right, rotation) * halfWidth * 0.8f;
        var offsetY = Vector3.Transform(Vector3.Up, rotation) * (PostHeight / 2f);
        var offsetTop = Vector3.Transform(Vector3.Up, rotation) * (PostHeight - CrossbarHeight / 2f);

        var posLeft = position - offsetX + offsetY;
        var posRight = position + offsetX + offsetY;
        var posTop = position + offsetTop;

        boundingVolume1 = physicsManager.AddStaticBox(PostWidth, PostHeight, PostDepth, posLeft, rotation, this);
        boundingVolume2 = physicsManager.AddStaticBox(PostWidth, PostHeight, PostDepth, posRight, rotation, this);
        boundingVolume3 = physicsManager.AddStaticBox(CrossbarWidth, CrossbarHeight, CrossbarDepth, posTop, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.BannerHighModel,
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