using Microsoft.Xna.Framework.Input;

namespace TGC.TP.FIFO.Cameras;

public class TargetCamera
{
    // Matrices de la camara
    public XnaMatrix World { get; private set; }

    public XnaMatrix Projection { get; private set; }

    public XnaMatrix View { get; private set; }

    // Distancia de la camara al objetivo
    private float CameraTargetDistance;

    // Direccion hacia adelante de la camara (ignorando componente Y)
    public XnaVector3 ForwardXZ => XnaVector3.Normalize(new XnaVector3(World.Backward.X, 0, World.Backward.Z));

    // Direccion hacia la derecha de la camara (ignorando componente Y)
    public XnaVector3 RightXZ => XnaVector3.Normalize(new XnaVector3(World.Right.X, 0, World.Right.Z));

    // Sensibilidad de la rotacion al input del mouse
    private float MouseSensitivity;

    // Ultima posicion registrada del mouse
    private XnaVector2 LastMousePosition;

    // Se esta rotando la camara?
    private bool IsRotating = false;

    // Rotacion actual de la camara
    // Usamos quaterniones para evitar el gimbal lock
    public XnaQuaternion Rotation = XnaQuaternion.Identity;

    // Posicion del objetivo de la camara
    private XnaVector3 TargetPosition;

    // Offset para la posicion final de la camara
    private XnaVector3 Offset;

    public TargetCamera(float fov, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, XnaVector3 initialTargetPosition, float cameraTargetDistance, float mouseSensitivity)
    {
        CameraTargetDistance = cameraTargetDistance;
        MouseSensitivity = mouseSensitivity;
        TargetPosition = initialTargetPosition;

        Projection = XnaMatrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlaneDistance, farPlaneDistance);

        UpdateCameraView();
        UpdateCameraWorld();
    }

    // Actualizamos la rotacion de la camara con click derecho
    public void Update(XnaVector3 targetPosition)
    {
        TargetPosition = targetPosition;

        // Obtengo input del mouse
        var mouse = Mouse.GetState();
        var currentMousePosition = new XnaVector2(mouse.X, mouse.Y);

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
                var yaw = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, -deltaMousePosition.X * MouseSensitivity);

                // Rotacion vertical
                // Obtenemos el eje X actual transformando Vector3.Right con la orientacion actual
                var localRight = XnaVector3.Transform(XnaVector3.Right, Rotation);
                var pitch = XnaQuaternion.CreateFromAxisAngle(localRight, -deltaMousePosition.Y * MouseSensitivity);

                // Actualizamos la rotacion final de la camara
                Rotation = XnaQuaternion.Normalize(pitch * yaw * Rotation);
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
        Offset = XnaVector3.Transform(XnaVector3.Backward * CameraTargetDistance, Rotation);

        View = XnaMatrix.CreateLookAt(
            cameraPosition: TargetPosition + Offset,
            cameraTarget: TargetPosition,
            cameraUpVector: XnaVector3.Up);
    }

    private void UpdateCameraWorld()
    {
        World = XnaMatrix.Invert(View);
    }
}