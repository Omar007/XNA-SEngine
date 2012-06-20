using Microsoft.Xna.Framework;

namespace SEngine.GameObjects
{
	public class PlayerObject
	{
		#region Fields
		private PlayerIndex playerIndex;
		#endregion

		#region Properties
		public PlayerIndex PlayerIndex { get { return playerIndex; } }
		#endregion

		public PlayerObject()
		{
		}
	}
}
