using System;

namespace SEngineMono.Core
{
    public abstract class BaseObject
    {
        #region Fields
        public const ulong flag1 = 0x00000001;

        private Guid guid = Guid.Empty;

        private ulong flags = 0;
        #endregion

        #region Properties
        public Guid Guid { get { return guid; } }

        public string GuidString
        {
            get { return guid.ToString(); }
            set { guid = new Guid(value); }
        }

        public ulong Flags
        {
            get { return flags; }
            set { flags = value; }
        }
        #endregion

        public BaseObject()
        {
            guid = Guid.NewGuid();
            flags = 0;
        }

        public void SetFlag(ulong flag, bool enabled)
        {
            if (enabled)
                flags |= flag;
            else
                flags &= ~flag;
        }

        public bool HasFlag(ulong flag)
        {
            return (flags & flag) != 0;
        }

        public void GenerateGuid()
        {
            guid = Guid.NewGuid();
        }
    }
}
