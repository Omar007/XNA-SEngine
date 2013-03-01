using Microsoft.Xna.Framework.Graphics;

namespace SEngine.Graphics.Renderer
{
	internal class DefferedReentrantRenderer
	{
		private RenderTarget2D gBuffer;

		internal DefferedReentrantRenderer(GraphicsDevice graphicsDevice)
		{
			gBuffer = new RenderTarget2D(graphicsDevice, 1920, 1080, false, SurfaceFormat.HdrBlendable, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
		}
	}
}
