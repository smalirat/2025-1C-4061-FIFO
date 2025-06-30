using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos;

public class Checkpoint : IGameObject
{
    private readonly StaticHandle boundingVolume;
    private readonly XnaVector3 position;
    private readonly float scale;
    private readonly bool glow;
    private XnaQuaternion rotation;

    public XnaVector3 Position => PhysicsManager.GetPosition(boundingVolume);
    public bool Checked { get; private set; } = false;

    public Checkpoint(XnaVector3 position, bool glow, float scale = 1f)
    {
        this.scale = scale;
        this.position = position;
        this.glow = glow;

        rotation = XnaQuaternion.Identity;
        boundingVolume = PhysicsManager.AddStaticBox(2f, 10f, 2f, position + new Vector3(0f, 5f, 0f), rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(scale, scale, scale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var baseTransforms = new XnaMatrix[ModelManager.FlagModel.Bones.Count];
        ModelManager.FlagModel.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var material = MaterialPresets.Checkpoint;
        var finalColor = Checked ? Color.LimeGreen : Color.Blue;
        var baseColor = finalColor.ToVector3();

        foreach (var mesh in ModelManager.FlagModel.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                //meshPart.Effect = effectManager.BlinnPhongShader;
                //meshPart.Effect = effectManager.BasicGlowShader;
                meshPart.Effect = EffectManager.BasicGlowShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = EffectManager.BasicGlowShader;
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
            effect.Parameters["useGlow"]?.SetValue(glow);
            effect.Parameters["emissiveColor"]?.SetValue(baseColor);

            float tiempo = (float)GameState.Cronometer.Elapsed.TotalSeconds;
            effect.Parameters["emissiveStrength"]?.SetValue(0.5f + 0.3f * MathF.Sin(tiempo * 3f));

            mesh.Draw();
        }
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.5f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (!Checked && !GameState.Lost)
        {
            Checked = true;
            AudioManager.PlayCheckpointSound();
            GameState.CheckpointChecked();
        }
    }

    public void Reset()
    {
        Checked = false;
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }

    public XnaVector3 GetPlayerBallRespawnPosition()
    {
        return Position + new XnaVector3(0f, 10f, 0f);
    }
}