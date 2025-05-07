using Microsoft.Xna.Framework;
using System;

namespace TGC.MonoGame.TP.Cameras;

/// <summary>
/// Camara fija en tercera persona que sigue a un objeto
/// </summary>
public class TargetCamera
{
    // Matrices de vista y proyeccion
    public Matrix Projection { get; private set; }

    public Matrix View { get; private set; }

    public TargetCamera(float aspectRatio)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 3f, aspectRatio, 0.1f, 1000f);
    }

    /// <summary>
    /// Actualiza la posicion de la camara respecto al objeto seguido
    /// </summary>
    public void Update(Vector3 targetPosition)
    {
        var backOffset = new Vector3(0, 10f, 30f); // Cámara detrás y arriba
        var cameraPosition = targetPosition + backOffset;

        View = Matrix.CreateLookAt(cameraPosition, targetPosition, Vector3.Up);
    }
}