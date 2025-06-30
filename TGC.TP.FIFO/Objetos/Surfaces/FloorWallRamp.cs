using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Luz;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public abstract class FloorWallRamp : IGameObject
{
    private const float Height = 1.25f;

    private readonly StaticHandle boundingVolume;
    private readonly BoxPrimitive model;
    private readonly RampWallTextureType rampWallTextureType;
    private readonly XnaMatrix world;
    private readonly FloorWallRampType floorWallRampType;

    public FloorWallRamp(XnaVector3 position, XnaQuaternion rotation, float width, float length, FloorWallRampType floorWallRampType, RampWallTextureType rampWallTextureType)
    {
        this.rampWallTextureType = rampWallTextureType;
        this.floorWallRampType = floorWallRampType;

        model = ModelManager.CreateBox(Height, width, length);
        boundingVolume = PhysicsManager.AddStaticBox(width, Height, length, position, rotation, this);
        world = XnaMatrix.CreateFromQuaternion(PhysicsManager.GetOrientation(boundingVolume)) * XnaMatrix.CreateTranslation(PhysicsManager.GetPosition(boundingVolume));
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var effect = EffectManager.BlinnPhongShader;
        Texture2D texture = null;
        BlinnPhongMaterial material;

        switch (rampWallTextureType)
        {
            case RampWallTextureType.Dirt:
                texture = TextureManager.DirtTexture;
                material = MaterialPresets.Tierra;
                effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"
                break;

            case RampWallTextureType.Stones:
                texture = TextureManager.StonesTexture;
                material = MaterialPresets.Piedra;
                effect.CurrentTechnique = effect.Techniques["NormalMapping"]; // Opciones: "Default", "Gouraud", "NormalMapping"
                break;

            default:
                texture = TextureManager.DirtTexture;
                material = MaterialPresets.Tierra;
                effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"
                break;
        }

        effect.Parameters["WorldViewProjection"]?.SetValue(world * view * projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["ambientColor"]?.SetValue(material.AmbientColor);
        effect.Parameters["diffuseColor"]?.SetValue(material.DiffuseColor);
        effect.Parameters["specularColor"]?.SetValue(material.SpecularColor);
        effect.Parameters["KAmbient"]?.SetValue(material.KAmbient);
        effect.Parameters["KDiffuse"]?.SetValue(material.KDiffuse);
        effect.Parameters["KSpecular"]?.SetValue(material.KSpecular);
        effect.Parameters["shininess"]?.SetValue(material.Shininess);

        effect.Parameters["lightPosition"]?.SetValue(lightPosition);
        effect.Parameters["eyePosition"]?.SetValue(eyePosition);
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(1.0f, 1.0f));

        effect.Parameters["baseTexture"]?.SetValue(texture);

        // Solo establecer la textura de normales si estamos usando NormalMapping
        if (effect.CurrentTechnique.Name == "NormalMapping")
        {
            effect.Parameters["NormalTexture"]?.SetValue(TextureManager.StonesNormalTexture);
        }

        model.Draw(effect);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        if (!(playerBall as PlayerBall).IsDummy && floorWallRampType == FloorWallRampType.Wall)
        {
            AudioManager.PlayWallHitSound(GameState.BallType, contactSpeed);
        }
    }

    public void Reset()
    {
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return floorWallRampType != FloorWallRampType.Wall;
    }
}