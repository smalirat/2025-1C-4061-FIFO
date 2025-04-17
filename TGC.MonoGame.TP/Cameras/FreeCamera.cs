using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TGC.MonoGame.TP
{
    /// <summary>
    /// Cámara libre que se puede mover con teclado y rotar con mouse.
    /// </summary>
    class FreeCamera
    {
        public const float DefaultFieldOfViewDegrees = MathHelper.PiOver4;
        public const float DefaultNearPlaneDistance = 0.1f;
        public const float DefaultFarPlaneDistance = 2000;

        private readonly bool _lockMouse;
        private readonly Point _screenCenter;
        private bool _changed;
        private Vector2 _pastMousePosition;
        private float _pitch;
        private float _yaw = -90f;

        public float AspectRatio { get; set; }
        public float FarPlane { get; set; }
        public float FieldOfView { get; set; }
        public float NearPlane { get; set; }

        public Vector3 FrontDirection { get; set; }
        public Vector3 RightDirection { get; set; }
        public Vector3 UpDirection { get; set; }
        public Vector3 Position { get; set; }

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public float MovementSpeed { get; set; } = 100f;
        public float MouseSensitivity { get; set; } = 5f;

        public FreeCamera(float aspectRatio, Vector3 position, Point screenCenter)
        {
            _lockMouse = true;
            _screenCenter = screenCenter;

            AspectRatio = aspectRatio;
            Position = position;
            FieldOfView = DefaultFieldOfViewDegrees;
            NearPlane = DefaultNearPlaneDistance;
            FarPlane = DefaultFarPlaneDistance;

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
            _pastMousePosition = Mouse.GetState().Position.ToVector2();
            UpdateCameraVectors();
            CalculateView();
        }

        public FreeCamera(float aspectRatio, Vector3 position)
        {
            AspectRatio = aspectRatio;
            Position = position;
            FieldOfView = DefaultFieldOfViewDegrees;
            NearPlane = DefaultNearPlaneDistance;
            FarPlane = DefaultFarPlaneDistance;

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
            _pastMousePosition = Mouse.GetState().Position.ToVector2();
            UpdateCameraVectors();
            CalculateView();
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _changed = false;

            ProcessKeyboard(elapsedTime);
            ProcessMouseMovement(elapsedTime);

            if (_changed)
                CalculateView();
        }

        private void ProcessKeyboard(float elapsedTime)
        {
            var keyboardState = Keyboard.GetState();
            float currentMovementSpeed = MovementSpeed;

            if (keyboardState.IsKeyDown(Keys.LeftShift))
                currentMovementSpeed *= 5f;

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                Position -= RightDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                Position += RightDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                Position += FrontDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                Position -= FrontDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }

            // Movimiento vertical (opcional)
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                Position += UpDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                Position -= UpDirection * currentMovementSpeed * elapsedTime;
                _changed = true;
            }
        }

        private void ProcessMouseMovement(float elapsedTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                var mouseDelta = mouseState.Position.ToVector2() - _pastMousePosition;
                mouseDelta *= MouseSensitivity * elapsedTime;

                _yaw -= mouseDelta.X;
                _pitch += mouseDelta.Y;

                _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);

                _changed = true;
                UpdateCameraVectors();

                if (_lockMouse)
                {
                    Mouse.SetPosition(_screenCenter.X, _screenCenter.Y);
                    Mouse.SetCursor(MouseCursor.Crosshair);
                }
                else
                {
                    Mouse.SetCursor(MouseCursor.Arrow);
                }
            }

            _pastMousePosition = mouseState.Position.ToVector2();
        }

        private void UpdateCameraVectors()
        {
            Vector3 tempFront;
            tempFront.X = MathF.Cos(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch));
            tempFront.Y = MathF.Sin(MathHelper.ToRadians(_pitch));
            tempFront.Z = MathF.Sin(MathHelper.ToRadians(_yaw)) * MathF.Cos(MathHelper.ToRadians(_pitch));

            FrontDirection = Vector3.Normalize(tempFront);
            RightDirection = Vector3.Normalize(Vector3.Cross(FrontDirection, Vector3.Up));
            UpDirection = Vector3.Normalize(Vector3.Cross(RightDirection, FrontDirection));
        }

        private void CalculateView()
        {
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }
    }
}
