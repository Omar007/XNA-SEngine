using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SEngine.Base
{
	public class GameData
	{
		#region Fields
		private GraphicsDevice graphicsDevice;

		private ContentManager contentManager;

		private ContentManager levelContentManager;
		#endregion

		#region Properties
		public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }

		public ContentManager ContentManager { get { return contentManager; } }

		public ContentManager LevelContentManager { get { return levelContentManager; } }
		#endregion

		public GameData(GraphicsDevice graphicsDevice, ContentManager contentManager, ContentManager levelContentManager)
		{
			this.graphicsDevice = graphicsDevice;
			this.contentManager = contentManager;
			this.levelContentManager = levelContentManager;
		}
	}
}
