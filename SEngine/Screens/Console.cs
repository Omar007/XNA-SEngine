using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SEngine.Screens
{
	/// <summary>
	/// Delegate for a console command.
	/// </summary>
	/// <param name="game">The Game instance.</param>
	/// <param name="argv">The command arguments where argv[0] is the command itself.</param>
	/// <param name="gameTime">Time of execution.</param>
	public delegate void ConsoleAction(Game game, string[] argv, GameTime gameTime);

	/// <summary>
	/// Class containing the settings for the console.
	/// </summary>
	public class ConsoleSettings
	{
		public Color Background = Color.Black;
		public Color Color = Color.White;
		public float Alpha = 0.8f;
		public int LineCount = 10;
		public float Padding = 4;
		public string FontAsset = "Fonts/console";
	}

	/// <summary>
	/// Class for a command console.
	/// </summary>
	public class SimpleConsole : DrawableGameComponent
	{
		private ConsoleSettings Settings;

		public Dictionary<string, ConsoleAction> Commands { get; protected set; }

		private List<string> lines;
		private string input;
		private int cursor;
		private string last;

		private float height { get { if (this.font == null) return 200; return (this.font.LineSpacing * Settings.LineCount) + (2 * Settings.Padding); } }

		private BasicEffect effect;
		private SpriteBatch batch;
		private SpriteFont font;

		private KeyboardState kbs;
		private KeyMap keyMap;

		/// <summary>
		/// Constructor. Initializes the console using the given settings or the default if none.
		/// </summary>
		/// <param name="game">The Game instance.</param>
		/// <param name="settings">The settings for this console.</param>
		public SimpleConsole(Game game, ConsoleSettings settings = null)
			: base(game)
		{
			this.Settings = settings;
			if (this.Settings == null)
				this.Settings = new ConsoleSettings();

			this.Commands = new Dictionary<string, ConsoleAction>();
			this.lines = new List<string>();
			this.WriteLine("] " + Game.GetType().Name + " console");
			this.input = this.last = "";

			this.DrawOrder = int.MaxValue / 2;
			this.UpdateOrder = int.MaxValue / 2;
		}

		/// <summary>
		/// Adds a set of default commands to the console.
		/// </summary>
		public void RegisterDefaults()
		{
			this.Commands["quit"] = delegate(Game game, string[] argv, GameTime gameTime)
			{
				game.Exit();
			};

			this.Commands["echo"] = delegate(Game game, string[] argv, GameTime gameTime)
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 1; i < argv.Length; i++)
				{
					sb.Append(argv[i]);
					if (i != argv.Length - 1)
						sb.Append(" ");
				}
				this.WriteLine(sb.ToString());
			};

			this.Commands["commands"] = delegate(Game game, string[] argv, GameTime gameTime)
			{
				this.WriteLine("available commands:");
				foreach (string key in this.Commands.Keys)
				{
					this.WriteLine(key);
				}
			};

			// aliases:
			this.Commands["exit"] = this.Commands["quit"];
			this.Commands["print"] = this.Commands["echo"];
			this.Commands["help"] = this.Commands["commands"];
		}

		/// <summary>
		/// Initializes the console.
		/// </summary>
		public override void Initialize()
		{
			this.Visible = false;
			this.RegisterDefaults();
			this.kbs = Keyboard.GetState();
			this.keyMap = new KeyMap();

			base.Initialize();
		}

		/// <summary>
		/// Loads the required effects and fonts.
		/// </summary>
		protected override void LoadContent()
		{
			effect = new BasicEffect(Game.GraphicsDevice);
			batch = new SpriteBatch(Game.GraphicsDevice);
			font = Game.Content.Load<SpriteFont>(Settings.FontAsset);

			base.LoadContent();
		}

		/// <summary>
		/// Updates the console and executes the commands entered.
		/// </summary>
		/// <param name="gameTime">Time since last update (deltaTime).</param>
		public override void Update(GameTime gameTime)
		{
			KeyboardState kbs = Keyboard.GetState();
			if (!this.Visible)
			{
				if (kbs.IsKeyDown(Keys.OemTilde) && this.kbs.IsKeyUp(Keys.OemTilde))
					this.Visible = true;
				this.kbs = kbs;
				base.Update(gameTime);
				return;
			}
			if (kbs.IsKeyDown(Keys.Escape) && this.kbs.IsKeyUp(Keys.Escape))
			{
				this.Visible = false;
				this.kbs = kbs;
				base.Update(gameTime);
				return;
			}

			#region Read typed letters:
			bool shift = kbs.IsKeyDown(Keys.LeftShift) || kbs.IsKeyDown(Keys.RightShift);
			foreach (Keys k in kbs.GetPressedKeys())
			{
				if (!this.kbs.IsKeyUp(k))
					continue;
				char c = this.keyMap.getChar(k, shift ? KeyMap.Modifier.Shift : KeyMap.Modifier.None);
				if (c != '\0')
					this.input = this.input.Insert(this.cursor++, c.ToString());
			}
			#endregion

			#region Cursor navigation:
			if (kbs.IsKeyDown(Keys.Left) && this.kbs.IsKeyUp(Keys.Left))
				this.cursor--;
			if (kbs.IsKeyDown(Keys.Right) && this.kbs.IsKeyUp(Keys.Right))
				this.cursor++;
			this.cursor = (int)MathHelper.Clamp(this.cursor, 0, this.input.Length);
			if (kbs.IsKeyDown(Keys.Back) && this.kbs.IsKeyUp(Keys.Back) && this.cursor > 0)
			{
				this.input = this.input.Remove(this.cursor - 1, 1);
				this.cursor = (int)MathHelper.Clamp(this.cursor - 1, 0, this.input.Length);
			}
			#endregion

			#region Handle command execution:
			if (kbs.IsKeyDown(Keys.Enter) && this.kbs.IsKeyUp(Keys.Enter))
			{
				ExecuteCommand(this.input, gameTime);

				//string command;
				//if (this.input.Length > 0)
				//    command = this.last = this.input;
				//else
				//    command = this.last;
				//if (command.Length > 0)
				//{
				//    string[] argv = command.Trim().Split();
				//    if (this.Commands.ContainsKey(argv[0]))
				//        this.Commands[argv[0]](Game, argv, gameTime);
				//    else
				//        this.WriteLine("command not found: " + argv[0]);
				//}
				//this.input = "";
				//this.cursor = 0;
			}
			#endregion

			this.kbs = kbs;
			base.Update(gameTime);
		}

		/// <summary>
		/// Draws the console on the screen.
		/// </summary>
		/// <param name="gameTime">Time since the last draw (deltaTime).</param>
		public override void Draw(GameTime gameTime)
		{
			string inputline = "] " + this.input;
			float y = this.height - Settings.Padding - this.font.LineSpacing;

			#region Draw Background Quad:
			VertexPositionColor[] vertices = new VertexPositionColor[] {
                new VertexPositionColor( new Vector3(0, 0, 0), Color.White ),
                new VertexPositionColor( new Vector3(Game.GraphicsDevice.Viewport.Width, 0, 0), Color.White ),
                new VertexPositionColor( new Vector3(Game.GraphicsDevice.Viewport.Width, this.height, 0), Color.White ),
                    
                new VertexPositionColor( new Vector3(0, 0, 0), Color.White ),
                new VertexPositionColor( new Vector3(Game.GraphicsDevice.Viewport.Width, this.height, 0), Color.White ),
                new VertexPositionColor( new Vector3(0, height, 0), Color.White )
            };

			this.effect.Projection = Matrix.CreateOrthographicOffCenter(
				0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, 0,
				0, 1);
			this.effect.DiffuseColor = this.Settings.Background.ToVector3();
			this.effect.Alpha = this.Settings.Alpha;

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, 2);
			}
			#endregion

			this.batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			#region Draw input and lines:
			this.batch.DrawString(this.font, inputline, new Vector2(Settings.Padding, y), this.Settings.Color);
			y -= this.font.LineSpacing;

			for (int i = 1; i < Settings.LineCount && this.lines.Count - i >= 0; i++ )
			{
				string line = this.lines[this.lines.Count - i];
				foreach (string chunk in BuildLines(Game.GraphicsDevice.Viewport.Width - Settings.Padding * 2, line))
				{
					this.batch.DrawString(this.font, chunk, new Vector2(Settings.Padding, y), this.Settings.Color);
					y -= this.font.LineSpacing;
				}
			}
			#endregion

			this.batch.End();

			#region Draw cursor:
			Vector2 cursor = this.font.MeasureString(inputline.Substring(0, 2 + this.cursor)) + new Vector2(Settings.Padding, 0);
			cursor.Y = height - this.font.LineSpacing - Settings.Padding;
			effect.DiffuseColor = Settings.Color.ToVector3();
			effect.Alpha = 1.0f;

			VertexPositionColor[] cursorverts = new VertexPositionColor[] {
                        new VertexPositionColor( new Vector3(cursor, 0), Color.White ),
                        new VertexPositionColor( new Vector3(cursor.X, cursor.Y + font.LineSpacing, 0), Color.White )
                    };
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
					cursorverts, 0, 1);
			}
			#endregion

			base.Draw(gameTime);
		}

		/// <summary>
		/// Executes a command without GameTime data.
		/// </summary>
		/// <param name="input">The command to execute.</param>
		public void ExecuteCommand(string input)
		{
			ExecuteCommand(input, null);
		}

		/// <summary>
		/// Executes the given command with the given GameTime data.
		/// </summary>
		/// <param name="input">The command to execute.</param>
		/// <param name="gameTime">Execution time.</param>
		public void ExecuteCommand(string input, GameTime gameTime)
		{
			if (input.Length > 0)
			{
				this.last = input;
			}
			else
			{
				input = this.last;
			}

			if (input.Length > 0)
			{
				string[] argv = input.Trim().Split();
				if (this.Commands.ContainsKey(argv[0]))
				{
					this.Commands[argv[0]](Game, argv, gameTime);
				}
				else
				{
					IEnumerable<string> foundKeys = this.Commands.Keys.Where<string>(x => x.StartsWith(argv[0]));
					int keysFound = foundKeys.Count<string>();

					switch (keysFound)
					{
						case 1:
							this.Commands[foundKeys.ElementAt<string>(0)](Game, argv, gameTime);
							break;

						case 0:
							this.WriteLine("command not found: " + argv[0]);
							break;

						default:
							this.WriteLine("multiple commands found with: " + argv[0]);
							foreach (string key in foundKeys)
							{
								this.WriteLine(key);
							}
							break;
					}
				}
			}
			this.input = "";
			this.cursor = 0;
		}

		/// <summary>
		/// Write a line to the console.
		/// </summary>
		/// <param name="line">The line to write.</param>
		public void WriteLine(string line)
		{
			this.lines.Add(line);
		}

		/// <summary>
		/// Constructs the given line to fit in the console.
		/// </summary>
		/// <param name="width_bound">Maximum line width.</param>
		/// <param name="text">The line to reconstruct.</param>
		/// <returns>The line split up.</returns>
		private List<string> BuildLines(float width_bound, string text)
		{
			List<string> result = new List<string>();
			StringBuilder line = new StringBuilder();

			float width = 0;

			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				Vector2 size = this.font.MeasureString(c.ToString());
				if (width + size.X >= width_bound)
				{
					result.Insert(0, line.ToString());
					line.Remove(0, line.Length);
					line.Append(c);
					width = size.X;
				}
				else
				{
					line.Append(text[i]);
					width += size.X;
				}
			}

			if (line.Length > 0)
				result.Insert(0, line.ToString());

			return result;
		}
	}

	/// <summary>
	/// Class that maps key information.
	/// </summary>
	public class KeyMap
	{
		/// <summary>
		/// Enum that indicates a modifier over a key.
		/// </summary>
		public enum Modifier : int
		{
			None,
			Shift,
		}

		private Dictionary<Keys, Dictionary<Modifier, char>> map;

		/// <summary>
		/// Constructor. Initializes all key to char bindings.
		/// </summary>
		public KeyMap()
		{
			map = new Dictionary<Keys, Dictionary<Modifier, char>>();
			map[Keys.Space] = new Dictionary<Modifier, char>();
			map[Keys.Space][Modifier.None] = ' ';

			char[] specials = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };

			for (int i = 0; i <= 9; i++)
			{
				char c = (char)(i + 48);
				map[(Keys)c] = new Dictionary<Modifier, char>();
				map[(Keys)c][Modifier.None] = c;
				map[(Keys)c][Modifier.Shift] = specials[i];
			}

			for (char c = 'A'; c <= 'Z'; c++)
			{
				map[(Keys)c] = new Dictionary<Modifier, char>();
				map[(Keys)c][Modifier.None] = (char)(c + 32);
				map[(Keys)c][Modifier.Shift] = c;
			}

			map[Keys.OemPipe] = new Dictionary<Modifier, char>();
			map[Keys.OemPipe][Modifier.None] = '\\';
			map[Keys.OemPipe][Modifier.Shift] = '|';

			map[Keys.OemOpenBrackets] = new Dictionary<Modifier, char>();
			map[Keys.OemOpenBrackets][Modifier.None] = '[';
			map[Keys.OemOpenBrackets][Modifier.Shift] = '{';

			map[Keys.OemCloseBrackets] = new Dictionary<Modifier, char>();
			map[Keys.OemCloseBrackets][Modifier.None] = ']';
			map[Keys.OemCloseBrackets][Modifier.Shift] = '}';

			map[Keys.OemComma] = new Dictionary<Modifier, char>();
			map[Keys.OemComma][Modifier.None] = ',';
			map[Keys.OemComma][Modifier.Shift] = '<';

			map[Keys.OemPeriod] = new Dictionary<Modifier, char>();
			map[Keys.OemPeriod][Modifier.None] = '.';
			map[Keys.OemPeriod][Modifier.Shift] = '>';

			map[Keys.OemSemicolon] = new Dictionary<Modifier, char>();
			map[Keys.OemSemicolon][Modifier.None] = ';';
			map[Keys.OemSemicolon][Modifier.Shift] = ':';

			map[Keys.OemQuestion] = new Dictionary<Modifier, char>();
			map[Keys.OemQuestion][Modifier.None] = '/';
			map[Keys.OemQuestion][Modifier.Shift] = '?';

			map[Keys.OemQuotes] = new Dictionary<Modifier, char>();
			map[Keys.OemQuotes][Modifier.None] = '\'';
			map[Keys.OemQuotes][Modifier.Shift] = '"';

			map[Keys.OemMinus] = new Dictionary<Modifier, char>();
			map[Keys.OemMinus][Modifier.None] = '-';
			map[Keys.OemMinus][Modifier.Shift] = '_';

			map[Keys.OemPlus] = new Dictionary<Modifier, char>();
			map[Keys.OemPlus][Modifier.None] = '=';
			map[Keys.OemPlus][Modifier.Shift] = '+';
		}

		/// <summary>
		/// Gets the char for the given key, using the given modifier.
		/// </summary>
		/// <param name="key">The key to get.</param>
		/// <param name="mod">The modifier to use.</param>
		/// <returns></returns>
		public char getChar(Keys key, Modifier mod)
		{
			if (!map.ContainsKey(key))
				return '\0';
			if (!map[key].ContainsKey(mod))
				return '\0';
			return map[key][mod];
		}

		/// <summary>
		/// Gets all the chars available.
		/// </summary>
		/// <returns>List containing all the available characters.</returns>
		public List<char> listChars()
		{
			List<char> chars = new List<char>();

			foreach (Keys key in map.Keys)
				foreach (Modifier mod in map[key].Keys)
					if (!chars.Contains(map[key][mod]))
						chars.Add(map[key][mod]);

			return chars;
		}
	}
}
