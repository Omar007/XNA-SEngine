using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SEngineMono.Graphics.Renderer
{
    internal static class QuadRenderer
    {
        private static readonly short[] indices = null;
        private static readonly VertexPositionTexture[] verts;

        static QuadRenderer()
        {
            indices = new short[] { 0, 3, 2, 0, 1, 3 };

            verts = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0)),
			    new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0)),
			    new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1)),
			    new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1))
            };
        }

        public static void RenderFullScreenQuad(GraphicsDevice graphicsDevice)
        {
            RenderQuad(graphicsDevice, -Vector2.One, Vector2.One);
        }

        public static void RenderQuad(GraphicsDevice graphicsDevice, Vector2 v1, Vector2 v2)
        {
            verts[0].Position.X = v1.X;
            verts[0].Position.Y = v2.Y;

            verts[1].Position.X = v2.X;
            verts[1].Position.Y = v2.Y;

            verts[2].Position.X = v1.X;
            verts[2].Position.Y = v1.Y;

            verts[3].Position.X = v2.X;
            verts[3].Position.Y = v1.Y;

            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
                (PrimitiveType.TriangleList, verts, 0, 4, indices, 0, 2);
        }
    }
}