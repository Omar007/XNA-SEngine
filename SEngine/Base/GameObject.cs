using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SEngine.Base
{
	public class GameObject : BaseObject
	{
		#region Fields
		private Matrix worldMatrix = Matrix.Identity;
		#endregion

		#region Properties
		public Matrix WorldMatrix { get { return worldMatrix; } }
		#endregion
	}
}
