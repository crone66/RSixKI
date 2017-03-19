using Microsoft.Xna.Framework;

namespace RSixKI
{
    public class MovingArgs
    {
        public Vector2 PreviousePosition;
        public Vector2 Position;
        public bool Cancel;

        public MovingArgs(Vector2 previousePosition, Vector2 position)
        {
            PreviousePosition = previousePosition;
            Position = position;
            Cancel = false;
        }
    }
}
