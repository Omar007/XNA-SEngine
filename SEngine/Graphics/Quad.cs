using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SEngine.Graphics
{
	internal class Quad
	{
		/*private static readonly VertexPositionTexture[] vertices = {
			new VertexPositionTexture(new Vector3(-1, -1, 0), Vector2.UnitY),
			new VertexPositionTexture(new Vector3(1, -1, 0), Vector2.One),
			new VertexPositionTexture(new Vector3(-1, 1, 0), Vector2.Zero),
			new VertexPositionTexture(new Vector3(1, 1, 0), Vector2.UnitX)
		};*/

		private static readonly VertexPositionTexture[] vertices = {
			new VertexPositionTexture(new Vector3(-1, 1, 1), Vector2.Zero),
			new VertexPositionTexture(new Vector3(1, 1, 1), Vector2.UnitX),
			new VertexPositionTexture(new Vector3(-1, -1, 1), Vector2.UnitY),
			new VertexPositionTexture(new Vector3(1, -1, 1), Vector2.One)
		};

		public static void RenderFullscreenQuad(GraphicsDevice graphicsDevice)
		{
			graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, vertices, 0, 2);
		}
	}
}
