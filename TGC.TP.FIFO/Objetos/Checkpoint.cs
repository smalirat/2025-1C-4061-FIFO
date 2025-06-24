using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos;

public class Checkpoint : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;

    private readonly StaticHandle boundingVolume;

    public bool Checked { get; private set; } = false;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private const float ModelHeight = 1f;
    private const float ModelWidth = 1f;
    private const float ModelLength = 1f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.Checkpoint;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public bool CanPlayerBallJumpOnIt => false;

    public Checkpoint(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        GraphicsDevice graphicsDevice,
        AudioManager audioManager,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
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

        boundingVolume = this.physicsManager.AddStaticBox(width * 2, height * 4, depth * 2, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(XScale, YScale, ZScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var baseTransforms = new XnaMatrix[modelManager.FlagModel.Bones.Count];
        modelManager.FlagModel.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var finalColor = Checked ? Color.LimeGreen : color;

        foreach (var mesh in modelManager.FlagModel.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effectManager.BasicShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            effectManager.BasicShader.Parameters["World"]?.SetValue(meshTransform * localTransform);
            effectManager.BasicShader.Parameters["View"]?.SetValue(view);
            effectManager.BasicShader.Parameters["Projection"]?.SetValue(projection);
            effectManager.BasicShader.Parameters["DiffuseColor"]?.SetValue(finalColor.ToVector3());

            mesh.Draw();
        }
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.5f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollition(IColisionable with)
    {
        if (with.BodyType == BodyType.PlayerBall && !Checked && !GameState.Lost)
        {
            Checked = true;
            audioManager.PlayCheckpointSound();
            GameState.CheckpointChecked();
        }
    }

    public XnaVector3 GetPlayerBallRespawnPosition()
    {
        return Position + new XnaVector3(0f, 10f, 0f);
    }

    public void Reset()
    {
        Checked = false;
    }
}