using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SEngine.Input
{
	public class InputAction
	{
		private readonly string name;
		private readonly PlayerIndex? controllingPlayer;
		private readonly Buttons? button;
		private readonly Keys? key;
		private readonly bool newPressOnly;

		private readonly ButtonPress buttonTest;
		private readonly KeyPress keyTest;

		// These delegate types map to the methods on InputState. We use these to simplify the evalute method
		// by allowing us to map the appropriate delegates and invoke them, rather than having two separate code paths.
		private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex player);
		private delegate bool KeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex player);

		public InputAction(string name, PlayerIndex? controllingPlayer, Buttons? button, Keys? key, bool newPressOnly, InputManager state)
		{
			this.name = name;
			this.controllingPlayer = controllingPlayer;

			this.button = button;
			this.key = key;

			this.newPressOnly = newPressOnly;

			// Figure out which delegate methods to map from the state which takes care of our "newPressOnly" logic
			if (newPressOnly)
			{
				buttonTest = state.IsNewButtonPress;
				keyTest = state.IsNewKeyPress;
			}
			else
			{
				buttonTest = state.IsButtonPressed;
				keyTest = state.IsKeyPressed;
			}
		}

		public bool Evaluate()
		{
			PlayerIndex player;

			if (button.HasValue && buttonTest(button.Value, controllingPlayer, out player))
			{
				return true;
			}

			if (key.HasValue && keyTest(key.Value, controllingPlayer, out player))
			{
				return true;
			}

			return false;
		}
	}
}
