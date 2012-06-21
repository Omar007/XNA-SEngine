using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		// Converts a rotation vector into a rotation matrix
		public static Matrix Vector3ToMatrix(Vector3 Rotation)
		{
			return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
		}

		// Returns Euler angles that point from one point to another
		public static Vector3 AngleTo(Vector3 from, Vector3 location)
		{
			Vector3 angle = new Vector3();
			Vector3 v3 = Vector3.Normalize(location - from);

			angle.X = (float)Math.Asin(v3.Y);
			angle.Y = (float)Math.Atan2((double)-v3.X, (double)-v3.Z);

			return angle;
		}

		// Converts a Quaternion to Euler angles (X = Yaw, Y = Pitch, Z = Roll)
		public static Vector3 QuaternionToEulerAngleVector3(Quaternion rotation)
		{
			Vector3 rotationaxes = new Vector3();
			Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
			Vector3 up = Vector3.Transform(Vector3.Up, rotation);

			rotationaxes = AngleTo(new Vector3(), forward);

			if (rotationaxes.X == MathHelper.PiOver2)
			{
				rotationaxes.Y = (float)Math.Atan2((double)up.X, (double)up.Z);
				rotationaxes.Z = 0;
			}
			else if (rotationaxes.X == -MathHelper.PiOver2)
			{
				rotationaxes.Y = (float)Math.Atan2((double)-up.X, (double)-up.Z);
				rotationaxes.Z = 0;
			}
			else
			{
				up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
				up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));

				rotationaxes.Z = (float)Math.Atan2((double)-up.X, (double)up.Y);
			}

			return rotationaxes;
		}

		// Converts a Rotation Matrix to a quaternion, then into a Vector3 containing
		// Euler angles (X: Pitch, Y: Yaw, Z: Roll)
		public static Vector3 MatrixToEulerAngleVector3(Matrix Rotation)
		{
			Vector3 translation, scale;
			Quaternion rotation;

			Rotation.Decompose(out scale, out rotation, out translation);

			Vector3 eulerVec = QuaternionToEulerAngleVector3(rotation);

			return eulerVec;
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
