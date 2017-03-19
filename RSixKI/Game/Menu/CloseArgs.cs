using System;

namespace RSixKI
{
    public class CloseArgs : EventArgs
    {
        public object Args;
        public CloseArgs(object args)
        {
            Args = args;
        }
    }
}
