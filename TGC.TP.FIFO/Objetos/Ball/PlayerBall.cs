using BepuPhysics;
using Microsoft.Xna.Framework;
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
using TGC.TP.FIFO.Objetos.PowerUps.Jump;
using TGC.TP.FIFO.Objetos.PowerUps.Speed;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.Objetos.Ball;

public class PlayerBall : IGameObject
{
    private const float InactivityThreshold = 10f;
    private const float CanJumpThreshold = 0.5f;
    private const float ModelRadius = 1.00f;

    private float inactivityTimer = 0f;
    private float canJumpTimer = 0f;
    private XnaVector3 respawnPosition;
    private XnaVector3 initialPosition;
    private BodyHandle boundingVolume;
    private XnaMatrix world;
    private BallProperties ballProperties;
    private bool jumpMultiplierApplied = false;
    private bool speedMultiplierApplied = false;
    private bool isRolling = false;
    private float speedMultiplier = 0f;
    private float jumpMultiplier = 0f;

    private float XScale => ballProperties.Radius / ModelRadius;
    private float YScale => ballProperties.Radius / ModelRadius;
    private float ZScale => ballProperties.Radius / ModelRadius;

    public XnaVector3 Position => world.Translation.ToBepuVector3();
    public bool CanJump = false;
    public bool IsDummy;

    public PlayerBall(XnaVector3 initialPosition, bool isDummy = false)
    {
        this.initialPosition = initialPosition;

        IsDummy = isDummy;
        respawnPosition = initialPosition;
        ballProperties = BallPresets.Presets[GameState.BallType];
        world = XnaMatrix.CreateTranslation(respawnPosition);

        boundingVolume = PhysicsManager.AddDynamicSphere(
            radius: ballProperties.Radius,
            mass: ballProperties.Mass,
            friction: ballProperties.Friction,
            dampingRatio: ballProperties.DampingRatio,
            springFrequency: ballProperties.SpringFrequency,
            maximumRecoveryVelocity: ballProperties.MaximumRecoveryVelocity,
            initialPosition: respawnPosition,
            this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var model = ModelManager.SphereModel;
        var effect = EffectManager.BlinnPhongShader;
        Texture2D texture;
        BlinnPhongMaterial material;

        effect.CurrentTechnique = effect.Techniques["Default"]; // Opciones: "Default", "Gouraud", "NormalMapping"

        // Selección del material y textura según el tipo de bola
        if (BallType.Goma == GameState.BallType)
        {
            texture = TextureManager.RubberTexture;
            material = MaterialPresets.Goma;
        }
        else if (BallType.Metal == GameState.BallType)
        {
            texture = TextureManager.SphereMetalTexture;
            material = MaterialPresets.Metal;
        }
        else
        {
            texture = TextureManager.SphereMarbleTexture;
            material = MaterialPresets.Madera; // o cualquier otro por defecto
        }

        // Calculo de matrices
        var worldViewProjection = world * view * projection;
        var inverseTransposeWorld = Matrix.Transpose(Matrix.Invert(world));

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effect;
            }

            // Envío de matrices al shader
            effect.Parameters["WorldViewProjection"]?.SetValue(worldViewProjection);
            effect.Parameters["World"]?.SetValue(world);
            effect.Parameters["InverseTransposeWorld"]?.SetValue(inverseTransposeWorld);

            // Envío del material al shader
            effect.Parameters["ambientColor"]?.SetValue(material.AmbientColor);
            effect.Parameters["diffuseColor"]?.SetValue(material.DiffuseColor);
            effect.Parameters["specularColor"]?.SetValue(material.SpecularColor);

            effect.Parameters["KAmbient"]?.SetValue(material.KAmbient);
            effect.Parameters["KDiffuse"]?.SetValue(material.KDiffuse);
            effect.Parameters["KSpecular"]?.SetValue(material.KSpecular);
            effect.Parameters["shininess"]?.SetValue(material.Shininess);

            // Luz y cámara
            effect.Parameters["lightPosition"]?.SetValue(lightPosition);
            effect.Parameters["eyePosition"]?.SetValue(eyePosition);
            effect.Parameters["Tiling"]?.SetValue(new XnaVector2(1.0f, 1.0f));

            // Textura base
            effect.Parameters["baseTexture"]?.SetValue(texture);

            // Solo establecer la textura de normales si estamos usando NormalMapping
            // if (effect.CurrentTechnique.Name == "NormalMapping")
            // {
            //     effect.Parameters["NormalTexture"]?.SetValue(textureManager.WoodBox1Texture);
            // }

            // Dibujo
            mesh.Draw();
        }
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var keyPressed = false;
        var impulseDirection = BepuVector3.Zero;

        // La pelota siempre esta activa en el mundo física
        PhysicsManager.Awake(boundingVolume);

        if (keyboardState.IsKeyDown(Keys.R))
        {
            PhysicsManager.SetPosition(boundingVolume, respawnPosition);
            return;
        }

        if (jumpMultiplierApplied)
        {
            PhysicsManager.ApplyImpulse(boundingVolume,
                XnaVector3.Up,
                ballProperties.JumpForce * jumpMultiplier,
                deltaTime);
        }
        else if (speedMultiplierApplied)
        {
            PhysicsManager.ApplyImpulse(boundingVolume,
                -camera.ForwardXZ.ToBepuVector3(),
                ballProperties.ImpulseForce * speedMultiplier,
                deltaTime);
        }
        else
        {
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                keyPressed = true;
                impulseDirection -= camera.ForwardXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                keyPressed = true;
                impulseDirection += camera.ForwardXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                keyPressed = true;
                impulseDirection -= camera.RightXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                keyPressed = true;
                impulseDirection += camera.RightXZ.ToBepuVector3();
            }

            if (keyPressed)
            {
                PhysicsManager.ApplyImpulse(boundingVolume,
                    impulseDirection,
                    ballProperties.ImpulseForce,
                    deltaTime);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && CanJump && canJumpTimer >= CanJumpThreshold)
            {
                canJumpTimer = 0f;
                AudioManager.PlayJumpSound(GameState.BallType);

                PhysicsManager.ApplyImpulse(boundingVolume,
                    XnaVector3.Up,
                    ballProperties.JumpForce,
                    deltaTime);
            }
        }

        float currentSpeed = PhysicsManager.GetLinearVelocity(boundingVolume).Length();

        if (currentSpeed > 0.1f && CanJump)
        {
            if (!isRolling)
            {
                isRolling = true;
                AudioManager.PlayRollingSound();
            }
            AudioManager.UpdateRollingSound(GameState.BallType, currentSpeed);
        }
        else
        {
            if (isRolling)
            {
                isRolling = false;
                AudioManager.StopRollingSound();
            }
        }

        // Actualizo matriz mundo
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(PhysicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(PhysicsManager.GetPosition(boundingVolume));

        CanJump = false;
        speedMultiplierApplied = false;
        jumpMultiplierApplied = false;

        inactivityTimer += deltaTime;
        canJumpTimer += deltaTime;

        if (inactivityTimer >= InactivityThreshold)
        {
            PhysicsManager.SetPosition(boundingVolume, respawnPosition);
            inactivityTimer = 0f;
        }
    }

    public void NotifyCollition(ICollisionable with, XnaVector3? contactNormal, float contactSpeed)
    {
        if (with.CanPlayerBallJumpOnIt() && contactNormal?.Y != 0)
        {
            CanJump = true;
        }

        if (with is Checkpoint)
        {
            var checkpoint = with as Checkpoint;
            respawnPosition = checkpoint.GetPlayerBallRespawnPosition();
        }

        if (with is SpeedPowerUp)
        {
            var speedPowerUp = with as SpeedPowerUp;
            speedMultiplierApplied = true;
            speedMultiplier = speedPowerUp.SpeedMultiplier;
        }

        if (with is JumpPowerUp)
        {
            var jumpPowerUp = with as JumpPowerUp;
            jumpMultiplierApplied = true;
            jumpMultiplier = jumpPowerUp.JumpMultiplier;
        }

        inactivityTimer = 0f;
    }

    public void Reset()
    {
        PhysicsManager.RemoveBoundingVolume(boundingVolume);

        ballProperties = BallPresets.Presets[GameState.BallType];

        boundingVolume = PhysicsManager.AddDynamicSphere(
            radius: ballProperties.Radius,
            mass: ballProperties.Mass,
            friction: ballProperties.Friction,
            dampingRatio: ballProperties.DampingRatio,
            springFrequency: ballProperties.SpringFrequency,
            maximumRecoveryVelocity: ballProperties.MaximumRecoveryVelocity,
            initialPosition: initialPosition,
            this);

        world = XnaMatrix.CreateTranslation(initialPosition);

        CanJump = false;
        jumpMultiplierApplied = false;
        speedMultiplierApplied = false;
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }

    public float GetCurrentSpeed()
    {
        return PhysicsManager.GetLinearVelocity(boundingVolume).Length();
    }

    public void UpdatePositionAndRotation(XnaVector3 position, XnaQuaternion rotation)
    {
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(rotation) *
                XnaMatrix.CreateTranslation(position);
    }

    public float GetLinearSpeed()
    {
        return PhysicsManager.GetLinearSpeed(boundingVolume);
    }
}