using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP.Cameras;

public class TargetCamera
{
    // Matrices de la camara
    public Matrix World { get; private set; }

    public Matrix Projection { get; private set; }

    public Matrix View { get; private set; }

    // Distancia de la camara al objetivo
    private float CameraTargetDistance;

    // Direccion hacia adelante de la camara (ignorando componente Y)
    public Vector3 ForwardXZ => Vector3.Normalize(new Vector3(World.Backward.X, 0, World.Backward.Z));

    // Direccion hacia la derecha de la camara (ignorando componente Y)
    public Vector3 RightXZ => Vector3.Normalize(new Vector3(World.Right.X, 0, World.Right.Z));

    // Sensibilidad de la rotacion al input del mouse
    private float MouseSensitivity;

    // Ultima posicion registrada del mouse
    private Vector2 LastMousePosition;

    // Se esta rotando la camara?
    private bool IsRotating = false;

    // Rotacion actual de la camara
    // Usamos quaterniones para evitar el gimbal lock
    public Quaternion Rotation = Quaternion.Identity;

    // Posicion del objetivo de la camara
    private Vector3 TargetPosition;

    // Offset para la posicion final de la camara
    private Vector3 Offset;

    public TargetCamera(float fov, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, Vector3 initialTargetPosition, float cameraTargetDistance, float mouseSensitivity)
    {
        CameraTargetDistance = cameraTargetDistance;
        MouseSensitivity = mouseSensitivity;
        TargetPosition = initialTargetPosition;

        Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlaneDistance, farPlaneDistance);

        UpdateCameraView();
        UpdateCameraWorld();
    }

    // Actualizamos la rotacion de la camara con click derecho
    public void Update(Vector3 targetPosition)
    {
        TargetPosition = targetPosition;

        // Obtengo input del mouse
        var mouse = Mouse.GetState();
        var currentMousePosition = new Vector2(mouse.X, mouse.Y);

        // Si se toco el boton derecho del mouse
        if (mouse.RightButton == ButtonState.Pressed)
        {
            // Quiere decir que se esta queriendo rotar la camara
            if (!IsRotating)
            {
                IsRotating = true;
            }
            else
            {
                // Actualizo las rotaciones segun el desplazamiento relativa del mouse
                var deltaMousePosition = currentMousePosition - LastMousePosition;

                // Rotacion horizontal
                var yaw = Quaternion.CreateFromAxisAngle(Vector3.Up, -deltaMousePosition.X * MouseSensitivity);

                // Rotacion vertical
                // Obtenemos el eje X actual transformando Vector3.Right con la orientacion actual
                var localRight = Vector3.Transform(Vector3.Right, Rotation);
                var pitch = Quaternion.CreateFromAxisAngle(localRight, -deltaMousePosition.Y * MouseSensitivity);

                // Actualizamos la rotacion final de la camara
                Rotation = Quaternion.Normalize(pitch * yaw * Rotation);
            }
        }
        else
        {
            IsRotating = false;
        }

        LastMousePosition = currentMousePosition;

        UpdateCameraView();
        UpdateCameraWorld();
    }

    private void UpdateCameraView()
    {
        // Hacemos que la camara siempre este detras del objetivo
        Offset = Vector3.Transform(Vector3.Backward * CameraTargetDistance, Rotation);

        View = Matrix.CreateLookAt(
            cameraPosition: TargetPosition + Offset,
            cameraTarget: TargetPosition,
            cameraUpVector: Vector3.Up);
    }

    private void UpdateCameraWorld()
    {
        World = Matrix.Invert(View);
    }
}