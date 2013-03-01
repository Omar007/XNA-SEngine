using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SEngineMono.Cameras
{
    /// <summary>
    /// Class used for all cameras.
    /// </summary>
    public abstract class Camera
    {
        /// <summary>
        /// The bounding frustum of the camera.
        /// </summary>
        public readonly BoundingFrustum bounds = new BoundingFrustum(Matrix.Identity);

        /// <summary>
        /// The cameras near clip.
        /// </summary>
        public virtual float nearClip { get { return 0.01f; } }

        /// <summary>
        /// The cameras far clip.
        /// </summary>
        public virtual float farClip { get { return 250; } }

        /// <summary>
        /// The cameras field of view.
        /// </summary>
        public virtual int fov { get { return 45; } }

        /// <summary>
        /// The cameras tangent field of view. Used by lighting.
        /// </summary>
        public float tanFov { get { return (float)Math.Tan(MathHelper.ToRadians(fov * 0.5f)); } }

        private Matrix _worldMatrix = Matrix.Identity;
        private Matrix _viewMatrix = Matrix.Identity;
        private Matrix _projectionMatrix = Matrix.Identity;
        private Matrix _viewProjectionMatrix = Matrix.Identity;

        /// <summary>
        /// Gets the cameras world matrix.
        /// Sets the world matrix and recalculates the view matrix, viewprojection matrix and the 2D transformation matrix.
        /// </summary>
        public Matrix worldMatrix
        {
            get { return _worldMatrix; }
            set
            {
                _worldMatrix = value;
                Matrix.Invert(ref _worldMatrix, out _viewMatrix);
                Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out _viewProjectionMatrix);
                bounds.Matrix = _viewProjectionMatrix;
                twoDimensionalTransformation = _viewMatrix * invertY;
            }
        }

        /// <summary>
        /// Gets the cameras view matrix.
        /// Sets the view matrix and recalculates the world matrix, viewprojection matrix and the 2D transformation matrix.
        /// </summary>
        public Matrix viewMatrix
        {
            get { return _viewMatrix; }
            set
            {
                _viewMatrix = value;
                Matrix.Invert(ref _viewMatrix, out _worldMatrix);
                Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out _viewProjectionMatrix);
                bounds.Matrix = _viewProjectionMatrix;
                twoDimensionalTransformation = _viewMatrix * invertY;
            }
        }

        /// <summary>
        /// Gets the cameras projection matrix.
        /// Sets the projection matrix and recalculates the viewprojection matrix.
        /// Also sets the 3D spritebatch projection matrix.
        /// </summary>
        public Matrix projectionMatrix
        {
            get { return _projectionMatrix; }
            set
            {
                _projectionMatrix = value;
                Matrix.Multiply(ref _viewMatrix, ref _projectionMatrix, out _viewProjectionMatrix);
                bounds.Matrix = _viewProjectionMatrix;
                spritebatch3DEffect.Projection = _projectionMatrix;
            }
        }

        /// <summary>
        /// Gets the cameras viewprojection matrix.
        /// </summary>
        public Matrix viewProjectionMatrix { get { return _viewProjectionMatrix; } }

        #region 2D in 3D
        /// <summary>
        /// SpriteBatch effect to draw in 3D space.
        /// </summary>
        public BasicEffect spritebatch3DEffect { get; private set; }

        /// <summary>
        /// Matrix to invert the Y-axis.
        /// </summary>
        private static readonly Matrix invertY = Matrix.CreateScale(1, -1, 1);

        /// <summary>
        /// Gets the transformation matrix for 2D objects.
        /// </summary>
        public Matrix twoDimensionalTransformation { get; private set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Camera(GraphicsDevice graphicsDevice)
        {
            spritebatch3DEffect = new BasicEffect(graphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true
            };
            spritebatch3DEffect.World = invertY;
            spritebatch3DEffect.View = Matrix.Identity;
        }
    }
}
