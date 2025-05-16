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
    public XnaVector3 Position => world.Translation.ToBepuVector3();

    public BodyType BodyType => BodyType.PlayerBall;

    public bool CanPlayerBallJumpOnIt => false;

    private const float ModelRadius = 1.00f;

    private readonly ModelManager modelManager;
    private readonly EffectManager effectManager;
    private readonly PhysicsManager physicsManager;
    private readonly TextureManager textureManager;

    private readonly BodyHandle boundingVolume;

    private XnaMatrix world;

    private BallProperties ballProperties;

    private bool canJump;

    private float XScale => ballProperties.Radius / ModelRadius;
    private float YScale => ballProperties.Radius / ModelRadius;
    private float ZScale => ballProperties.Radius / ModelRadius;

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

        this.ballProperties = BallPresets.Presets[ballType];

        boundingVolume = this.physicsManager.AddDynamicSphere(
            radius: this.ballProperties.Radius,
            mass: this.ballProperties.Mass,
            friction: this.ballProperties.Friction,
            dampingRatio: this.ballProperties.DampingRatio,
            springFrequency: this.ballProperties.SpringFrequency,
            maximumRecoveryVelocity: this.ballProperties.MaximumRecoveryVelocity,
            initialPosition: initialPosition,
            this);
        world = XnaMatrix.CreateTranslation(initialPosition);

        this.canJump = false;
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var keyPressed = false;
        var impulseDirection = BepuVector3.Zero;

        // La pelota siempre esta activa en el mundo física
        physicsManager.Awake(boundingVolume);

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

        if (keyboardState.IsKeyDown(Keys.Space) && this.canJump)
        {
            physicsManager.ApplyImpulse(boundingVolume,
                XnaVector3.Up,
                impulseOffset: XnaVector3.Zero,
                this.ballProperties.JumpForce,
                deltaTime);
        }

        // Actualizo matriz mundo
        world = XnaMatrix.CreateScale(XScale, YScale, ZScale) *
                XnaMatrix.CreateFromQuaternion(physicsManager.GetOrientation(boundingVolume)) *
                XnaMatrix.CreateTranslation(physicsManager.GetPosition(boundingVolume));

        this.canJump = false;
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var model = this.modelManager.SphereModel;
        var effect = this.effectManager.SphereShader;

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = effect;
            }

            effect.Parameters["View"]?.SetValue(view);
            effect.Parameters["Projection"]?.SetValue(projection);
            effect.Parameters["World"]?.SetValue(world);
            effect.Parameters["ModelTexture"]?.SetValue(textureManager.RubberTexture);

            mesh.Draw();
        }
    }

    public void NotifyCollition(IColisionable with)
    {
        if (with.CanPlayerBallJumpOnIt)
        {
            this.canJump = true;
        }
    }
}