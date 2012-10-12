using System;
using Microsoft.Xna.Framework;

namespace SEngine.Helpers
{
	public static class HelperFunctions
	{
		public static void TransformBoundingBox(ref BoundingBox src, ref Matrix transform, out BoundingBox dst)
		{
			Vector3 center, extents, rotatedExtents = Vector3.Zero;

			center = (src.Min + src.Max) * 0.5f;
			extents = src.Max - center;

			rotatedExtents.X = Math.Abs(extents.X * transform.Right.X) + Math.Abs(extents.Y * transform.Up.X) + Math.Abs(extents.Z * transform.Forward.X);
			rotatedExtents.Y = Math.Abs(extents.X * transform.Right.Y) + Math.Abs(extents.Y * transform.Up.Y) + Math.Abs(extents.Z * transform.Forward.Y);
			rotatedExtents.Z = Math.Abs(extents.X * transform.Right.Z) + Math.Abs(extents.Y * transform.Up.Z) + Math.Abs(extents.Z * transform.Forward.Z);

			center = Vector3.Transform(center, transform);

			dst.Min = center - rotatedExtents;
			dst.Max = center + rotatedExtents;
		}

		/// <summary>
		/// Creates a rotation matrix from a rotation vector
		/// </summary>
		/// <param name="Rotation"></param>
		/// <returns></returns>
		public static Matrix Vector3ToMatrix(Vector3 Rotation)
		{
			return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
		}

		/// <summary>
		/// Returns Euler angles that point from one point to another
		/// </summary>
		/// <param name="from"></param>
		/// <param name="location"></param>
		/// <returns></returns>
		public static Vector3 AngleTo(Vector3 from, Vector3 location)
		{
			Vector3 v3 = Vector3.Normalize(location - from);

			Vector3 angle = new Vector3()
			{
				X = (float)Math.Asin(v3.Y),
				Y = (float)Math.Atan2(-v3.X, -v3.Z)
			};

			return angle;
		}

		/// <summary>
		/// Converts a Quaternion to Euler angles (X = Yaw, Y = Pitch, Z = Roll)
		/// </summary>
		/// <param name="rotation"></param>
		/// <returns></returns>
		public static Vector3 QuaternionToEulerAngleVector3(Quaternion rotation)
		{
			Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
			Vector3 up = Vector3.Transform(Vector3.Up, rotation);

			Vector3 rotationAxes = AngleTo(Vector3.Zero, forward);

			if (rotationAxes.X == MathHelper.PiOver2)
			{
				rotationAxes.Y = (float)Math.Atan2(up.X, up.Z);
				rotationAxes.Z = 0;
			}
			else if (rotationAxes.X == -MathHelper.PiOver2)
			{
				rotationAxes.Y = (float)Math.Atan2(-up.X, -up.Z);
				rotationAxes.Z = 0;
			}
			else
			{
				up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationAxes.Y));
				up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationAxes.X));

				rotationAxes.Z = (float)Math.Atan2(-up.X, up.Y);
			}

			return rotationAxes;
		}

		/// <summary>
		/// Converts a Rotation Matrix to a quaternion, then into a Vector3 containing
		/// Euler angles (X: Pitch, Y: Yaw, Z: Roll)
		/// </summary>
		/// <param name="Rotation"></param>
		/// <returns></returns>
		public static Vector3 MatrixToEulerAngleVector3(Matrix Rotation)
		{
			Vector3 translation, scale;
			Quaternion rotation;

			Rotation.Decompose(out scale, out rotation, out translation);

			return QuaternionToEulerAngleVector3(rotation);
		}

		public static Vector3 RadiansToDegrees(Vector3 Vector)
		{
			return new Vector3(
				MathHelper.ToDegrees(Vector.X),
				MathHelper.ToDegrees(Vector.Y),
				MathHelper.ToDegrees(Vector.Z));
		}

		public static Vector3 DegreesToRadians(Vector3 Vector)
		{
			return new Vector3(
				MathHelper.ToRadians(Vector.X),
				MathHelper.ToRadians(Vector.Y),
				MathHelper.ToRadians(Vector.Z));
		}
	}
}
