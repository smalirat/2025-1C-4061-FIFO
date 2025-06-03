using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Texturas;
using TGC.TP.FIFO.Utilidades;


namespace TGC.TP.FIFO.Objetos.Ball;

public class PlayerBall : IColisionable
{
    private float inactivityTimer = 0f;
    private float changeBallTypeTimer = 0f;
    private float canJumpTimer = 0f;

    private XnaVector3 respawnPosition;
    private const float InactivityThreshold = 10f;
    private const float ChangeBallTypeThreshold = 1f;
    private const float CanJumpThreshold = 0.5f;

    public XnaVector3 Position => world.Translation.ToBepuVector3();

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

    private bool canJump;
    private bool jumpMultiplierApplied;
    private bool speedMultiplierApplied;
    private bool isRolling = false;
    private float speedMultiplier;
    private float jumpMultiplier;

    private float XScale => ballProperties.Radius / ModelRadius;
    private float YScale => ballProperties.Radius / ModelRadius;
    private float ZScale => ballProperties.Radius / ModelRadius;

    public BallType ballType;
    private GraphicsDevice graphicsDevice;
    private XnaVector3 vector3;
    private BallType metal;


    public PlayerBall(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        AudioManager audioManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition,
        BallType ballType)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;
        this.audioManager = audioManager;


        this.ballType = ballType;
        respawnPosition = initialPosition;

        ballProperties = BallPresets.Presets[ballType];

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

        if (keyboardState.IsKeyDown(Keys.B))
        {
            if (changeBallTypeTimer >= ChangeBallTypeThreshold)
            {
                ChangeBallType();
                changeBallTypeTimer = 0f;
                return;
            }
        }

        if (keyboardState.IsKeyDown(Keys.R))
        {
            physicsManager.SetPosition(boundingVolume, respawnPosition);
            return;
        }

        if (jumpMultiplierApplied)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                XnaVector3.Up,
                impulseOffset: XnaVector3.Zero,
                ballProperties.JumpForce * jumpMultiplier,
                deltaTime);
        }
        else if (speedMultiplierApplied)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                -camera.ForwardXZ.ToBepuVector3(), // Ya normalizado
                impulseOffset: XnaVector3.Zero, // Centro de masa
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
                    impulseDirection, // Ya normalizado
                    impulseOffset: XnaVector3.Zero, // Centro de masa
                    ballProperties.ImpulseForce,
                    deltaTime);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && canJump && canJumpTimer >= CanJumpThreshold)
            {
                canJumpTimer = 0f;
                audioManager.PlayJumpSound(this.ballType);

                physicsManager.ApplyImpulse(boundingVolume,
                    XnaVector3.Up,
                    impulseOffset: XnaVector3.Zero,
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
            audioManager.UpdateRollingSound(ballType, currentSpeed);
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
        changeBallTypeTimer += deltaTime;
        canJumpTimer += deltaTime;

        if (inactivityTimer >= InactivityThreshold)
        {
            physicsManager.SetPosition(boundingVolume, respawnPosition);
            inactivityTimer = 0f;
        }
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var model = modelManager.SphereModel;
        var effect = effectManager.SphereShader;
        Texture2D texture = null;

        if (BallType.Rubber == ballType)
        {
            texture = textureManager.RubberTexture;
        }
        else if (BallType.Metal == ballType)
        {
            texture = textureManager.SphereMetalTexture;
        }
        else
        {
            texture = textureManager.SphereMarbleTexture;
        }

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effect;
            }

            effect.Parameters["View"]?.SetValue(view);
            effect.Parameters["Projection"]?.SetValue(projection);
            effect.Parameters["World"]?.SetValue(world);
            effect.Parameters["ModelTexture"]?.SetValue(texture);

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

    private void ChangeBallType()
    {
        BallType next = ballType switch
        {
            BallType.Metal => BallType.Rubber,
            BallType.Rubber => BallType.Stone,
            BallType.Stone => BallType.Metal,
            _ => BallType.Rubber
        };

        ChangeBallTypeTo(next);
    }

    private void ChangeBallTypeTo(BallType newBallType)
    {
        physicsManager.RemoveBoundingVolume(boundingVolume);

        ballType = newBallType;
        ballProperties = BallPresets.Presets[ballType];

        boundingVolume = physicsManager.AddDynamicSphere(
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

    public float GetCurrentSpeed()
    {
        return physicsManager.GetLinearVelocity(boundingVolume).Length();
    }
}