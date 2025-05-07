using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace TGC.MonoGame.TP.Cameras;

public class TargetCamera
{
    public Matrix Projection { get; private set; }
    public Matrix View { get; private set; }

    private float yaw = 0f;
    private float pitch = -0.5f;

    private float distance = 30f;
    private Vector2 lastMousePosition;
    private bool isRotating = false;

    public Vector3 ForwardXZ => Vector3.Normalize(new Vector3(View.Backward.X, 0, View.Backward.Z));
    public Vector3 RightXZ => Vector3.Normalize(new Vector3(View.Right.X, 0, View.Right.Z));

    public TargetCamera(float aspectRatio)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 3f, aspectRatio, 0.1f, 1000f);
    }

    public void Update(Vector3 targetPosition)
    {
        var mouse = Mouse.GetState();

        if (mouse.RightButton == ButtonState.Pressed)
        {
            if (!isRotating)
            {
                lastMousePosition = new Vector2(mouse.X, mouse.Y);
                isRotating = true;
            }

            var deltaX = mouse.X - lastMousePosition.X;
            var deltaY = mouse.Y - lastMousePosition.Y;

            yaw -= deltaX * 0.01f;
            pitch -= deltaY * 0.01f;
            pitch = MathHelper.Clamp(pitch, -1.4f, 1.4f);

            lastMousePosition = new Vector2(mouse.X, mouse.Y);
        }
        else
        {
            isRotating = false;
        }

        // Crear rotación combinada con quaternions
        var rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw) *
                       Quaternion.CreateFromAxisAngle(Vector3.Right, pitch);

        // Aplicar rotación al vector backward y escalar por la distancia
        var offset = Vector3.Transform(Vector3.Backward * distance, rotation);

        var cameraPosition = targetPosition + offset;

        View = Matrix.CreateLookAt(cameraPosition, targetPosition, Vector3.Up);
    }
}
