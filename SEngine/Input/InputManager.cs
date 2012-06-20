using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SEngine.Input
{
	public class InputManager
	{
		public const int MAX_INPUTS = 4;

		public readonly KeyboardState[] CurrentKeyboardStates;
		public readonly GamePadState[] CurrentGamePadStates;

		public readonly KeyboardState[] LastKeyboardStates;
		public readonly GamePadState[] LastGamePadStates;

		public readonly bool[] GamePadWasConnected;

		internal InputManager()
		{
			CurrentKeyboardStates = new KeyboardState[MAX_INPUTS];
			CurrentGamePadStates = new GamePadState[MAX_INPUTS];

			LastKeyboardStates = new KeyboardState[MAX_INPUTS];
			LastGamePadStates = new GamePadState[MAX_INPUTS];

			GamePadWasConnected = new bool[MAX_INPUTS];
		}

		public void Update()
		{
			for (int i = 0; i < MAX_INPUTS; i++)
			{
				LastKeyboardStates[i] = CurrentKeyboardStates[i];
				LastGamePadStates[i] = CurrentGamePadStates[i];

				CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
				CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

				// Keep track of whether a gamepad has ever been
				// connected, so we can detect if it is unplugged.
				if (CurrentGamePadStates[i].IsConnected)
				{
					GamePadWasConnected[i] = true;
				}
			}
		}

		public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue)
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return CurrentKeyboardStates[i].IsKeyDown(key);
			}
			else
			{
				// Accept input from any player.
				return (IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
						IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
						IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
						IsKeyPressed(key, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue)
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return CurrentGamePadStates[i].IsButtonDown(button);
			}
			else
			{
				// Accept input from any player.
				return (IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
						IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
						IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
						IsButtonPressed(button, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue)
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return (CurrentKeyboardStates[i].IsKeyDown(key) &&
						LastKeyboardStates[i].IsKeyUp(key));
			}
			else
			{
				// Accept input from any player.
				return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
						IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
						IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
						IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
			}
		}

		public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
		{
			if (controllingPlayer.HasValue)
			{
				// Read input from the specified player.
				playerIndex = controllingPlayer.Value;

				int i = (int)playerIndex;

				return (CurrentGamePadStates[i].IsButtonDown(button) &&
						LastGamePadStates[i].IsButtonUp(button));
			}
			else
			{
				// Accept input from any player.
				return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
						IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
						IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
						IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
			}
		}
	}
}
