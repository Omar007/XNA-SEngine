using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SEngine.Base;
using SEngine.Screens;

namespace SEngine
{
	public class SEngine
	{
		public static GameData GameData { get; private set; }

		#region Fields
		private SimpleConsole console;
		#endregion

		#region Properties
		public SimpleConsole Console { get { return console; } }
		#endregion

		public SEngine(Game game)
			: this(game, game.Content, game.Content)
		{
		}

		public SEngine(Game game, ContentManager contentManager, ContentManager levelContentManager)
		{
			GameData = new GameData(game.GraphicsDevice, contentManager, levelContentManager);

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
