using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Utilidades;

namespace TGC.TP.FIFO.Objetos.Ball;

public class PlayerBall : IColisionable
{
    private float inactivityTimer = 0f;
    private float canJumpTimer = 0f;

    private XnaVector3 respawnPosition;
    private const float InactivityThreshold = 10f;
    private const float CanJumpThreshold = 0.5f;

    public XnaVector3 Position => world.Translation.ToBepuVector3();
    private XnaVector3 InitialPosition;

    public BodyType BodyType => BodyType.PlayerBall;

    public bool CanPlayerBallJumpOnIt => false;

    private const float ModelRadius = 1.00f;

    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;
    private readonly AudioManager audioManager;

    private BodyHandle boundingVolume;

    private XnaMatrix world;

    private BallProperties ballProperties;

    public bool canJump;
    private bool jumpMultiplierApplied;
    private bool speedMultiplierApplied;
    private bool isRolling = false;
    private float speedMultiplier;
    private float jumpMultiplier;

    private float XScale => ballProperties.Radius / ModelRadius;
    private float YScale => ballProperties.Radius / ModelRadius;
    private float ZScale => ballProperties.Radius / ModelRadius;

    public PlayerBall(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;
        this.audioManager = audioManager;

        InitialPosition = initialPosition;
        respawnPosition = initialPosition;

        ballProperties = BallPresets.Presets[GameState.BallType];

        boundingVolume = this.physicsManager.AddDynamicSphere(
            radius: ballProperties.Radius,
            mass: ballProperties.Mass,
            friction: ballProperties.Friction,
            dampingRatio: ballProperties.DampingRatio,
            springFrequency: ballProperties.SpringFrequency,
            maximumRecoveryVelocity: ballProperties.MaximumRecoveryVelocity,
            initialPosition: respawnPosition,
            this);

        world = XnaMatrix.CreateTranslation(respawnPosition);

        canJump = false;
        jumpMultiplierApplied = false;
        speedMultiplierApplied = false;
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var keyPressed = false;
        var impulseDirection = BepuVector3.Zero;

        // La pelota siempre esta activa en el mundo física
        physicsManager.Awake(boundingVolume);

        if (keyboardState.IsKeyDown(Keys.R))
        {
            physicsManager.SetPosition(boundingVolume, respawnPosition);
            return;
        }

        if (jumpMultiplierApplied)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                XnaVector3.Up,
                ballProperties.JumpForce * jumpMultiplier,
                deltaTime);
        }
        else if (speedMultiplierApplied)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                -camera.ForwardXZ.ToBepuVector3(),
                ballProperties.ImpulseForce * speedMultiplier,
                deltaTime);
        }
        else
        {
            if (keyboardState.IsKeyDown(Keys.W))
            {
                keyPressed = true;
                impulseDirection -= camera.ForwardXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                keyPressed = true;
                impulseDirection += camera.ForwardXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                keyPressed = true;
                impulseDirection -= camera.RightXZ.ToBepuVector3();
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                keyPressed = true;
                impulseDirection += camera.RightXZ.ToBepuVector3();
            }

            if (keyPressed)
            {
                physicsManager.ApplyImpulse(boundingVolume,
                    impulseDirection,
                    ballProperties.ImpulseForce,
                    deltaTime);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && canJump && canJumpTimer >= CanJumpThreshold)
            {
                canJumpTimer = 0f;
                audioManager.PlayJumpSound(GameState.BallType);

                physicsManager.ApplyImpulse(boundingVolume,
                    XnaVector3.Up,
                    ballProperties.JumpForce,
                    deltaTime);
            }
        }

        float currentSpeed = physicsManager.GetLinearVelocity(boundingVolume).Length();

        if (currentSpeed > 0.1f && canJump)
        {
            if (!isRolling)
            {
                isRolling = true;
                audioManager.PlayRollingSound();
            }
            audioManager.UpdateRollingSound(GameState.BallType, currentSpeed);
        }
        else
        {
            if (isRolling)
            {
                isRolling = false;
                audioManager.StopRollingSound();
            }
        }

        // Actualizo matriz mundo
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));

        canJump = false;
        speedMultiplierApplied = false;
        jumpMultiplierApplied = false;

        inactivityTimer += deltaTime;
        canJumpTimer += deltaTime;

        if (inactivityTimer >= InactivityThreshold)
        {
            physicsManager.SetPosition(boundingVolume, respawnPosition);
            inactivityTimer = 0f;
        }
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition)
    {
        var model = modelManager.SphereModel;
        var effect = effectManager.BlinnPhongShader;
        Texture2D texture;
        BlinnPhongMaterial material;

        // Selección del material y textura según el tipo de bola
        if (BallType.Goma == GameState.BallType)
        {
            texture = textureManager.RubberTexture;
            material = MaterialPresets.Goma;
        }
        else if (BallType.Metal == GameState.BallType)
        {
            texture = textureManager.SphereMetalTexture;
            material = MaterialPresets.Metal;
        }
        else
        {
            texture = textureManager.SphereMarbleTexture;
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

            // Textura base
            effect.Parameters["baseTexture"]?.SetValue(texture);

            // Dibujo
            mesh.Draw();
        }
    }


    public void NotifyCollition(IColisionable with)
    {
        if (with.CanPlayerBallJumpOnIt)
        {
            canJump = true;
        }

        if (with.BodyType == BodyType.Checkpoint)
        {
            var checkpoint = with as Checkpoint;
            respawnPosition = checkpoint.GetPlayerBallRespawnPosition();
        }

        if (with.BodyType == BodyType.SpeedPowerUp)
        {
            var speedPowerUp = with as SpeedPowerUp;
            speedMultiplierApplied = true;
            speedMultiplier = speedPowerUp.SpeedMultiplier;
        }

        if (with.BodyType == BodyType.JumpPowerUp)
        {
            var jumpPowerUp = with as JumpPowerUp;
            jumpMultiplierApplied = true;
            jumpMultiplier = jumpPowerUp.JumpMultiplier;
        }

        inactivityTimer = 0f;
    }

    public float GetCurrentSpeed()
    {
        return physicsManager.GetLinearVelocity(boundingVolume).Length();
    }

    public void Reset()
    {
        physicsManager.RemoveBoundingVolume(boundingVolume);

        ballProperties = BallPresets.Presets[GameState.BallType];

        boundingVolume = physicsManager.AddDynamicSphere(
            radius: ballProperties.Radius,
            mass: ballProperties.Mass,
            friction: ballProperties.Friction,
            dampingRatio: ballProperties.DampingRatio,
            springFrequency: ballProperties.SpringFrequency,
            maximumRecoveryVelocity: ballProperties.MaximumRecoveryVelocity,
            initialPosition: InitialPosition,
            this);

        world = XnaMatrix.CreateTranslation(InitialPosition);

        canJump = false;
        jumpMultiplierApplied = false;
        speedMultiplierApplied = false;
    }

    public void UpdatePositionAndRotation(XnaVector3 position, XnaQuaternion rotation)
    {
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(rotation) *
                XnaMatrix.CreateTranslation(position);
    }
}