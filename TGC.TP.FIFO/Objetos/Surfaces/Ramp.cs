using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.TP.FIFO.Fisica;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Ramp : FloorWallRamp
{
    public Ramp(PhysicsManager physicsManager, GraphicsDevice graphicsDevice, XnaVector3 position, float width, float length) : base(physicsManager, graphicsDevice, position, XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 10f), width, length, FloorWallRampType.Floor, RampWallTextureType.Dirt)
    {
    }
}
