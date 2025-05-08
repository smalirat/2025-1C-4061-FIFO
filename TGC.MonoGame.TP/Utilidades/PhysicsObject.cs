using BepuPhysics;
using BepuPhysics.Collidables;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGC.MonoGame.TP.Utilidades;
using System;
using TGC.MonoGame.TP.Efectos;
using TGC.MonoGame.TP.Modelos.Primitivas;
using BepuVector3 = System.Numerics.Vector3;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;
using XnaQuaternion = Microsoft.Xna.Framework.Quaternion;
using XnaVector3 = Microsoft.Xna.Framework.Vector3;

namespace TGC.MonoGame.TP;

public class PhysicsObject
{
    private readonly Model model;
    private readonly EffectManager effectManager;
    private readonly Vector3 position;
    private readonly Quaternion rotation;

    public PhysicsObject(
        Model model,
        EffectManager effectManager,
        Simulation simulation,
        GraphicsDevice graphicsDevice,
        Vector3 position,
        Quaternion rotation,
        float width,
        float length)
    {
        this.model = model;
        this.effectManager = effectManager;
        this.position = position;
        this.rotation = rotation;

        // Física
        var boxShape = new Box(width, 40f, length);

        var staticDescription = new StaticDescription(
            new BepuVector3(position.X, position.Y, position.Z),
            rotation.ToBepuQuaternion(),
            simulation.Shapes.Add(boxShape)
        );

        simulation.Statics.Add(staticDescription);
    }

    public void Draw(Matrix view, Matrix projection, Matrix? scale = null, Color? color = null, Matrix? globalOffset = null, Matrix? globalRotation = null)
    {
        DrawUtilities.DrawModel(
            model,
            effectManager.BasicShader,
            view,
            projection,
            translation: Matrix.CreateTranslation(position),
            scale: scale,
            rotation: Matrix.CreateFromQuaternion(rotation),
            color: color,
            globalOffset: globalOffset,
            globalRotation: globalRotation
        );
    }
}
