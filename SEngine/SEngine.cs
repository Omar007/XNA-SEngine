using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SEngine.Base;
using SEngine.Screens;

namespace SEngine
{
	public class SEngine
	{
		private static SEngine sEngine;
		internal static SEngine SEngine { get { return sEngine; } }

		public static SEngine CreateInstance(Game game, ContentManager contentManager, ContentManager levelContentManager)
		{
			if (sEngine == null)
			{
				sEngine = new SEngine(game, contentManager, levelContentManager);
			}
			else
			{
				sEngine.gameData = new GameData(game.GraphicsDevice, contentManager, levelContentManager);
			}
			return sEngine;
		}

		#region Fields
		private GameData gameData;

		private SimpleConsole console;
		#endregion

		#region Properties
		public GameData GameData { get { return gameData; } }

		public SimpleConsole Console { get { return console; } }
		#endregion

		private SEngine(Game game, ContentManager contentManager, ContentManager levelContentManager)
		{
			gameData = new GameData(game.GraphicsDevice, contentManager, levelContentManager);

			console = new SimpleConsole(game);
			game.Components.Add(console);
		}

		//public void update(GameTime gameTime)
		//{
		//    console.Update(gameTime);
		//}

		//public void draw(GameTime gameTime)
		//{
		//    console.Draw(gameTime);
		//}
	}
}
