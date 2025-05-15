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

    private const float ModelHeight = 10f;
    private const float ModelWidth = 9.25f;
    private const float ModelLength = 0.75f;

    private readonly StaticHandle boundingVolume1;
    private readonly StaticHandle boundingVolume2;
    private readonly StaticHandle boundingVolume3;

    private readonly float width;
    private readonly float height;
    private readonly float length;
    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    // Escala real aplicada al modelo
    private float xScale => width / ModelWidth;

    private float yScale => height / ModelHeight;
    private float zScale => length / ModelLength;

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

        // Separación entre los postes: la mitad del ancho escalado
        var halfWidth = (ModelWidth / 2f) * xScale; // = 4.625 * xScale
        var postHeight = height;
        var postWidth = 0.3f * xScale; // Grosor visual estimado del poste
        var postDepth = length;

        // Altura del travesaño: muy fina (como un poste acostado)
        var crossbarHeight = 0.3f * yScale;
        var crossbarWidth = width;
        var crossbarDepth = length;

        // Poste izquierdo (desplazado en -X)
        var posLeft = position + Vector3.Left * halfWidth * 0.8f + Vector3.Up * (postHeight / 2f);
        boundingVolume1 = physicsManager.AddStaticBox(postWidth, postHeight, postDepth, posLeft, rotation);

        // Poste derecho (en +X)
        var posRight = position + Vector3.Right * halfWidth * 0.8f + Vector3.Up * (postHeight / 2f);
        boundingVolume2 = physicsManager.AddStaticBox(postWidth, postHeight, postDepth, posRight, rotation);

        // Travesaño horizontal (arriba del centro)
        var posTop = position + Vector3.Up * (postHeight - crossbarHeight / 2f);
        boundingVolume3 = physicsManager.AddStaticBox(crossbarWidth, crossbarHeight, crossbarDepth, posTop, rotation);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        DrawUtilities.DrawCustomModel(modelManager.BannerHighModel,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: XnaMatrix.CreateScale(xScale, yScale, zScale),
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color);
    }
}