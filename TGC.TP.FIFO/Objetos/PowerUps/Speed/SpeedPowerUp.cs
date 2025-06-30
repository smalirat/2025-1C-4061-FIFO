using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos.PowerUps.Speed;

public abstract class SpeedPowerUp : IGameObject
{
    private const float xScale = 3f / 492.08f;
    private const float yScale = 3f / 859.56f;
    private const float zScale = 1f / 115.72f;

    private readonly XnaVector3 modelOffset = new(0f, 2f, 0f);

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    public float SpeedMultiplier { get; private set; }

    public SpeedPowerUp(XnaVector3 position, float speedMultiplier, Color color)
    {
        this.position = position + modelOffset;
        this.color = color;

        rotation = XnaQuaternion.Identity;
        SpeedMultiplier = speedMultiplier;

        PhysicsManager.AddStaticBox(3.3f, 3f, 3.3f, position, XnaQuaternion.Identity, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(xScale, yScale, zScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = ModelManager.ArrowModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var material = MaterialPresets.PowerUp;
        var baseColor = color.ToVector3();

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = EffectManager.BlinnPhongShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = EffectManager.BlinnPhongShader;
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

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.8f);
        rotation = XnaQuaternion.Normalize(incrementalRotation);
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (contactSpeed >= GameState.MinBallSpeedForSounds)
        {
            AudioManager.PlaySpeedPowerUpSound();
        }
    }

    public void Reset()
    {
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }
}