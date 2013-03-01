using Microsoft.Xna.Framework;

namespace SEngineMono.Core
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
