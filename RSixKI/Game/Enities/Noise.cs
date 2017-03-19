using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class Noise : CollidAble
    {
        private int width;
        private int height;
        private float elapsedTime = 0f;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public Noise(int width, int height, string name, Vector2 position, bool isVisable = false, bool isActiv = false, int teamId = -1) : base(false, null, width, height, 0, 0, Vector2.One, Color.White, name, position, isVisable, isActiv, teamId)
        {
            this.width = width;
            this.height = height;
        }

        public void Init(Vector2 position, int width, int height, int teamId)
        {
            Vector2 size = new Vector2(width, height);
            position = position - (size / 2);
            this.teamId = teamId;
            this.width = width;
            this.height = height;
            CollisionBox = new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), width, height);
            IsActiv = true;
            Position = position;
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if(elapsedTime > 100f)
            {
                Destroy();
            }
            base.Update(gameTime); 
        }

        public override object Clone()
        {
            return new Noise(width, height, name, Vector2.Zero, false, false, -1);
        }
    }
}
