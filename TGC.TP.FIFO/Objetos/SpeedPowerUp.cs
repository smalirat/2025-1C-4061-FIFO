using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos;

public class SpeedPowerUp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;


    private readonly StaticHandle boundingVolume;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private const float ModelHeight = 859.56f;
    private const float ModelWidth = 492.08f;
    private const float ModelLength = 115.72f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.SpeedPowerUp;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public float SpeedMultiplier { get; private set; }
    public bool CanPlayerBallJumpOnIt => false;

    public SpeedPowerUp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        AudioManager audioManager,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
        float speedMultiplier,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.audioManager = audioManager;

        this.width = width;
        this.height = height;
        this.depth = depth;

        this.color = color;
        this.rotation = rotation;
        this.position = position;

        SpeedMultiplier = speedMultiplier;

        boundingVolume = this.physicsManager.AddStaticBox(width * 2, height * 2, depth * 2, position, rotation, this);
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.8f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(XScale, YScale, ZScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var baseTransforms = new XnaMatrix[modelManager.LigthingModel.Bones.Count];
        modelManager.LigthingModel.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        foreach (var mesh in modelManager.LigthingModel.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effectManager.BasicShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            effectManager.BasicShader.Parameters["World"]?.SetValue(meshTransform * localTransform);
            effectManager.BasicShader.Parameters["View"]?.SetValue(view);
            effectManager.BasicShader.Parameters["Projection"]?.SetValue(projection);
            effectManager.BasicShader.Parameters["DiffuseColor"]?.SetValue(color.ToVector3());

            mesh.Draw();
        }
    }

    public void NotifyCollition(IColisionable with)
    {
        audioManager.PlaySpeedPowerUpSound();
    }

    public void Reset()
    {
    }
}