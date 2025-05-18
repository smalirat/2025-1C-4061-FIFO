using BepuPhysics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Fisica;
using TGC.MonoGame.TP.Modelos;
using TGC.MonoGame.TP.Texturas;
using TGC.MonoGame.TP.Utilidades;

namespace TGC.MonoGame.TP.Objetos.Ball;

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

    private BodyHandle boundingVolume;

    private XnaMatrix world;

    private BallProperties ballProperties;

    private bool canJump;
    private bool jumpMultiplierApplied;
    private bool speedMultiplierApplied;

    private float speedMultiplier;
    private float jumpMultiplier;

    private float XScale => ballProperties.Radius / ModelRadius;
    private float YScale => ballProperties.Radius / ModelRadius;
    private float ZScale => ballProperties.Radius / ModelRadius;
    private BallType ballType;

    public PlayerBall(ModelManager modelManager,
        EffectManager effectManager,
        PhysicsManager physicsManager,
        TextureManager textureManager,
        GraphicsDevice graphicsDevice,
        XnaVector3 initialPosition,
        BallType ballType)
    {
        this.modelManager = modelManager;
        this.effectManager = effectManager;
        this.physicsManager = physicsManager;
        this.textureManager = textureManager;

        this.ballType = ballType;
        this.respawnPosition = initialPosition;

        this.ballProperties = BallPresets.Presets[ballType];

        boundingVolume = this.physicsManager.AddDynamicSphere(
            radius: this.ballProperties.Radius,
            mass: this.ballProperties.Mass,
            friction: this.ballProperties.Friction,
            dampingRatio: this.ballProperties.DampingRatio,
            springFrequency: this.ballProperties.SpringFrequency,
            maximumRecoveryVelocity: this.ballProperties.MaximumRecoveryVelocity,
            initialPosition: respawnPosition,
            this);

        world = XnaMatrix.CreateTranslation(respawnPosition);

        this.canJump = false;
        this.jumpMultiplierApplied = false;
        this.speedMultiplierApplied = false;
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
                this.ballProperties.JumpForce * jumpMultiplier,
                deltaTime);
        }
        else if (speedMultiplierApplied)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                -camera.ForwardXZ.ToBepuVector3(), // Ya normalizado
                impulseOffset: XnaVector3.Zero, // Centro de masa
                this.ballProperties.ImpulseForce * speedMultiplier,
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
                    this.ballProperties.ImpulseForce,
                    deltaTime);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && this.canJump && canJumpTimer >= CanJumpThreshold)
            {
                canJumpTimer = 0f;

                physicsManager.ApplyImpulse(boundingVolume,
                    XnaVector3.Up,
                    impulseOffset: XnaVector3.Zero,
                    this.ballProperties.JumpForce,
                    deltaTime);
            }
        }

        // Actualizo matriz mundo
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));

        this.canJump = false;
        this.speedMultiplierApplied = false;
        this.jumpMultiplierApplied = false;

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
        var model = this.modelManager.SphereModel;
        var effect = this.effectManager.SphereShader;
        Texture2D texture = null;

        if (BallType.Rubber == this.ballType)
        {
            texture = textureManager.RubberTexture;
        }
        else if (BallType.Metal == this.ballType)
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
            this.canJump = true;
        }

        if (with.BodyType == BodyType.Checkpoint)
        {
            var checkpoint = with as Checkpoint;
            respawnPosition = checkpoint.GetPlayerBallRespawnPosition();
        }

        if (with.BodyType == BodyType.SpeedPowerUp)
        {
            var speedPowerUp = with as SpeedPowerUp;
            this.speedMultiplierApplied = true;
            this.speedMultiplier = speedPowerUp.SpeedMultiplier;
        }

        if (with.BodyType == BodyType.JumpPowerUp)
        {
            var jumpPowerUp = with as JumpPowerUp;
            this.jumpMultiplierApplied = true;
            this.jumpMultiplier = jumpPowerUp.JumpMultiplier;
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
}