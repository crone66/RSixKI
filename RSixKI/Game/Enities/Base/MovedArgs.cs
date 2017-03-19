using Microsoft.Xna.Framework;

namespace RSixKI
{
    public class MovedArgs
    {
        public Vector2 PreviousePosition;
        public Vector2 Position;

        public MovedArgs(Vector2 previousePosition, Vector2 position)
        {
            PreviousePosition = previousePosition;
            Position = position;
        }
    }
}
