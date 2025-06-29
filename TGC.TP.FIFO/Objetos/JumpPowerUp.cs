using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Objetos.Ball;

namespace TGC.TP.FIFO.Objetos;

public class JumpPowerUp : IColisionable
{
    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly AudioManager audioManager;
    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;
    private const float xScale = 1f / 0.78f;
    private const float yScale = 5f / 3.72f;
    private const float zScale = 3f / 5.66f;

    public BodyType BodyType => BodyType.JumpPowerUp;
    public float JumpMultiplier { get; private set; }
    public bool CanPlayerBallJumpOnIt => false;

    public JumpPowerUp(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        AudioManager audioManager,
        XnaVector3 position,
        XnaQuaternion rotation,
        float jumpMultiplier,
        Color color)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.audioManager = audioManager;
        this.position = position;
        this.rotation = rotation;
        this.color = color;

        JumpMultiplier = jumpMultiplier;

        this.physicsManager.AddStaticBox(4.4f, 4.4f, 3.5f, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(xScale, yScale, zScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = modelManager.ArrowModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var material = MaterialPresets.PowerUp;
        var baseColor = color.ToVector3();

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effectManager.BlinnPhongShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = effectManager.BlinnPhongShader;
            effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"

            effect.Parameters["WorldViewProjection"]?.SetValue(meshTransform * localTransform * view * projection);
            effect.Parameters["World"]?.SetValue(meshTransform * localTransform);
            effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(meshTransform * localTransform)));

            effect.Parameters["ambientColor"]?.SetValue(baseColor * material.AmbientColor);
            effect.Parameters["diffuseColor"]?.SetValue(baseColor * material.DiffuseColor);
            effect.Parameters["specularColor"]?.SetValue(material.SpecularColor);
            effect.Parameters["KAmbient"]?.SetValue(material.KAmbient);
            effect.Parameters["KDiffuse"]?.SetValue(material.KDiffuse);
            effect.Parameters["KSpecular"]?.SetValue(material.KSpecular);
            effect.Parameters["shininess"]?.SetValue(material.Shininess);

            effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            effect.Parameters["eyePosition"]?.SetValue(eyePosition);
            effect.Parameters["Tiling"]?.SetValue(new XnaVector2(1.0f, 1.0f));

            // Solo establecer la textura de normales si estamos usando NormalMapping
            // if (effect.CurrentTechnique.Name == "NormalMapping")
            // {
            //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture);
            // }

            mesh.Draw();
        }
    }

    public void Update(float deltaTime)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.5f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollitionWithPlayerBall(PlayerBall playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= GameState.MinBallSpeedForSounds)
        {
            audioManager.PlayJumpPowerUpSound();
        }
    }

    public void NotifyCollition(IColisionable with) { }
}