using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos;

public class JumpPowerUp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;


    private readonly StaticHandle boundingVolume;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    private const float ModelHeight = 3.72f;
    private const float ModelWidth = 0.78f;
    private const float ModelLength = 5.66f;

    private readonly float width;
    private readonly float height;
    private readonly float depth;

    private float XScale => width / ModelWidth;
    private float YScale => height / ModelHeight;
    private float ZScale => depth / ModelLength;

    public BodyType BodyType => BodyType.JumpPowerUp;
    public XnaVector3 Position => physicsManager.GetPosition(boundingVolume);

    public float JumpMultiplier { get; private set; }
    public bool CanPlayerBallJumpOnIt => false;

    public JumpPowerUp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 position,
        XnaQuaternion rotation,
        float width,
        float height,
        float depth,
        float jumpMultiplier,
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

        JumpMultiplier = jumpMultiplier;

        boundingVolume = this.physicsManager.AddStaticBox(width * 2, height * 2, depth * 2, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(XScale, YScale, ZScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = modelManager.ArrowModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effectManager.BlinnPhongShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = effectManager.BlinnPhongShader;
            effect.Parameters["WorldViewProjection"]?.SetValue(meshTransform * localTransform * view * projection);
            effect.Parameters["World"]?.SetValue(meshTransform * localTransform);
            effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(meshTransform * localTransform)));

            var baseColor = color.ToVector3();
            effect.Parameters["ambientColor"]?.SetValue(baseColor * 0.3f);
            effect.Parameters["diffuseColor"]?.SetValue(baseColor);
            effect.Parameters["specularColor"]?.SetValue(Vector3.One);
            effect.Parameters["KAmbient"]?.SetValue(0.3f);
            effect.Parameters["KDiffuse"]?.SetValue(0.7f);
            effect.Parameters["KSpecular"]?.SetValue(0.8f);
            effect.Parameters["shininess"]?.SetValue(64.0f);

            effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            effect.Parameters["eyePosition"]?.SetValue(eyePosition);

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
        audioManager.PlayJumpPowerUpSound();
    }

    public void Reset()
    {
    }
}
