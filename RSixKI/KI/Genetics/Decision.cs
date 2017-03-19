using System;

namespace Genetics
{
    /// <summary>
    /// Decision container
    /// </summary>
    public abstract class Decision : ICloneable
    {
        public int Value;

        public abstract void Init();

        public abstract object Clone();
    }
}
