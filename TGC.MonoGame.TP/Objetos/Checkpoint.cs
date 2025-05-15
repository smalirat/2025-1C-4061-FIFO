using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos;

public class Checkpoint
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;

    private readonly StaticHandle boundingVolume1;
    private readonly StaticHandle boundingVolume2;
    private readonly StaticHandle boundingVolume3;


    private float width;
    private float height;
    private float length;
    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    public Checkpoint(ModelManager modelManager,
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

        boundingVolume1 = this.physicsManager.AddStaticBox(width, height, length, position, rotation); // Poste izquierda
        boundingVolume2 = this.physicsManager.AddStaticBox(width, height, length, position, rotation); // Poste derecho
        boundingVolume3 = this.physicsManager.AddStaticBox(width, height, length, position, rotation); // Poste superior
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.BannerHighModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(width / 2f, height / 2f, length / 2f),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }
}
