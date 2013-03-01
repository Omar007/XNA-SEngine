using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SEngineMono.Input;

namespace SEngineMono.Cameras
{
    /// <summary>
    /// Class for a free moving camera.
    /// Mostly usefull for debugging or flying around the world
    /// </summary>
    public sealed class FreeCamera : Camera
    {
        /// <summary>
        /// Maximum rotation speed.
        /// </summary>
        private const float rotationSpeed = 0.3f;

        /// <summary>
        /// Maximum move speed.
        /// </summary>
        private const float moveSpeed = 10.0f;

        /// <summary>
        /// The camera position.
        /// </summary>
        private Vector3 cameraPosition;

        /// <summary>
        /// Amount of yaw.
        /// </summary>
        private float leftrightRot;

        /// <summary>
        /// Amount of pitch.
        /// </summary>
        private float updownRot;

        /// <summary>
        /// Rotation matrix.
        /// </summary>
        private Matrix cameraRotation;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FreeCamera(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            cameraPosition = new Vector3(0, 2, 0);
            cameraRotation = Matrix.Identity;
            leftrightRot = updownRot = 0;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), 16 / 9, nearClip, farClip);

            UpdateViewMatrix();
        }

        /// <summary>
        /// Called whenever the camera is updated.
        /// Rotates and moves the camera.
        /// </summary>
        /// <param name="gameTime">Time since last update (deltaTime).</param>
        public void Update(GameTime gameTime)
        {
            Vector2 change = InputState.Instance.mousePositionChange;

            if (change != Vector2.Zero)
            {
                leftrightRot -= rotationSpeed * change.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                updownRot -= rotationSpeed * change.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
                Mouse.SetPosition(1024 / 2, 768 / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = Vector3.Zero;

            if (InputState.Instance.getKeyState(Keys.W) == EInputState.Active)
                moveVector.Z -= 1f;
            if (InputState.Instance.getKeyState(Keys.S) == EInputState.Active)
                moveVector.Z += 1f;
            if (InputState.Instance.getKeyState(Keys.D) == EInputState.Active)
                moveVector.X += 1f;
            if (InputState.Instance.getKeyState(Keys.A) == EInputState.Active)
                moveVector.X -= 1f;
            if (InputState.Instance.getKeyState(Keys.Space) == EInputState.Active)
                moveVector.Y += 1f;
            if (InputState.Instance.getKeyState(Keys.LeftControl) == EInputState.Active)
                moveVector.Y -= 1f;

            if (InputState.Instance.getKeyState(Keys.LeftShift) == EInputState.Active)
                moveVector *= 10;

            AddToCameraPosition(moveVector * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Adds the given vector to the camera position and recalculates the view matrix.
        /// </summary>
        /// <param name="vectorToAdd">The vector to add to the position.</param>
        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        /// <summary>
        /// Updates the view matrix.
        /// </summary>
        private void UpdateViewMatrix()
        {
            Vector3 cameraRotatedTarget = Vector3.Transform(Vector3.Forward, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraRotatedUpVector);
        }
    }
}
