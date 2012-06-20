using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SEngine.Graphics
{
	public interface IRenderable
	{
		void Render(GameTime gameTime);
	}
}
