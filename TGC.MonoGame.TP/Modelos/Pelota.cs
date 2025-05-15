using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using TGC.MonoGame.TP.Utilidades;
using TGC.MonoGame.TP.PowerUps;
using BepuVector3 = System.Numerics.Vector3;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;
using System.Linq;

namespace TGC.MonoGame.TP.Modelos;

public class Pelota
{
    private readonly EffectManager effectManager;
    private readonly Simulation simulation;

    public BodyHandle BodyHandle;
    private readonly SpherePrimitive SpherePrimitive;

    public XnaMatrix World { get; private set; }

    public XnaVector3 Position => World.Translation.ToBepuVector3();

    private readonly float mass;
    private readonly float diameter;
    private readonly float radius;

    private List<PowerUp> _powerUps = new(); //aca estaran todos los powerUps

    // Impulso fijo al tocar una tecla
    private const float ImpulsoFijo = 300f;

    public Pelota(EffectManager effectManager, Simulation simulation, GraphicsDevice graphicsDevice, XnaVector3 initialPosition, float diameter = 6f, float mass = 1f)
    {
        this.effectManager = effectManager;
        this.simulation = simulation;
        this.mass = mass;
        this.diameter = diameter;
        this.radius = this.diameter / 2f;

        SpherePrimitive = new SpherePrimitive(graphicsDevice, diameter);
        World = XnaMatrix.CreateTranslation(initialPosition);

        var sphereShape = new Sphere(radius); // Bounding volume
        var shapeIndex = simulation.Shapes.Add(sphereShape);

        var collidableDescription = new CollidableDescription(shapeIndex, maximumSpeculativeMargin: 0.2f);

        var bodyDescription = BodyDescription.CreateDynamic(
            initialPosition.ToBepuVector3(),
            inertia: sphereShape.ComputeInertia(mass),
            collidableDescription,
            new BodyActivityDescription(sleepThreshold: 0.1f)
        );

        BodyHandle = simulation.Bodies.Add(bodyDescription);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var bodyRef = simulation.Bodies.GetBodyReference(BodyHandle);
        bodyRef.Awake = true; // La pelota siempre esta activa en el mundo física

        // Movimiento de la pelota
        var direccion = BepuVector3.Zero;
        var offset = BepuVector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            direccion += camera.ForwardXZ.ToBepuVector3();
            offset = new BepuVector3(0, 0, -radius); // Aplico impulso en el borde frontal
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            direccion -= camera.ForwardXZ.ToBepuVector3();
            offset = new BepuVector3(0, 0, radius); // Aplico impulso en el borde trasero
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            direccion += camera.RightXZ.ToBepuVector3();
            offset = new BepuVector3(-radius, 0, 0); // Aplico impulso en el borde izquierdo
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            direccion -= camera.RightXZ.ToBepuVector3();
            offset = new BepuVector3(radius, 0, 0); // Aplico impulso en el borde derecho
        }

        // Si se oprimio una tecla
        if (direccion != BepuVector3.Zero)
        {
            // A mayor masa menor impulso, mas cuesta mover la pelota
            var impulso = direccion * ImpulsoFijo * deltaTime / mass;

            // Aplico el impulso en algun borde de la pelota
            var puntoDeAplicacion = bodyRef.Pose.Position + offset;

            bodyRef.ApplyImpulse(impulso * deltaTime, puntoDeAplicacion);
        }
        else
        {
            // Aplico fuerza de frenado proporcional a la velocidad
            var velocidadActual = bodyRef.Velocity.Linear;
            var coeficienteFrenado = 0.5f;
            var fuerzaFrenado = -velocidadActual * coeficienteFrenado * deltaTime;

            bodyRef.ApplyImpulse(fuerzaFrenado, bodyRef.Pose.Position);
        }

        // Actualizo matriz mundo
        World = XnaMatrix.CreateFromQuaternion(bodyRef.Pose.Orientation.ToXnaQuaternion()) *
                XnaMatrix.CreateTranslation(bodyRef.Pose.Position.ToXnaVector3());
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        var effect = this.effectManager.PelotaShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(World);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.MediumVioletRed.ToVector3());

        SpherePrimitive.Draw(effect); //cambiar forma de dibujar
    }

    //se agrega los power ups cuando se choca con un powerUP
    public void agregarPowerUp(PowerUp powerUp)
    {
        this._powerUps.Add(powerUp);
    }

    //eliminar los powerUps que ya estan vencidos
    private void quitarPowerUpVencidos()
    {
        this._powerUps.RemoveAll(p => p.NoVigente);
    } 

    //obtencion del valor a modificar para la velocidad
    private int VelocidadAumentadaPorXCuanto(double tiempoActual)
    {
        int velPorX = 1;//el neutro de la multiplicacion
        
        var aumentos = this._powerUps.OfType<Velocidad>().ToList();

        foreach( Velocidad vel in aumentos)
        {
            if(vel.estaVigente(tiempoActual))
            {
                velPorX *= vel.getMultiplicador(); // este puede ser x2, x3, ....
            }
            else
            {
                vel.NoVigente = true; // lo seteamos como que ya esta vencido para sacarlo de la lista
            }
        }

        return velPorX; 
    }

    //obtencion del valor a modificar para el salto
    private float distanciaExtraDeSalto(double tiempoActual)
    {
        float aumento = 0;

        var aumentos = this._powerUps.OfType<Elevacion>().ToList();

        foreach (Elevacion elev in aumentos)
        {
            if (elev.estaVigente(tiempoActual))
            {
                aumento += elev.getElevacionExtra(); // este puede ser +50, +200, +350, ....
            }
            else
            {
                elev.NoVigente = true; // lo seteamos como que ya esta vencido para sacarlo de la lista
            }
        }

        return aumento;
    }
}