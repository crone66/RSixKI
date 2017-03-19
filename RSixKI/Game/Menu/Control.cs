using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace RSixKI
{
    public abstract class Control : IComparable<Control>
    {
        protected Rectangle position;
        protected bool isVisable;
        protected bool isSelected;
        private int index;
        private string name;
        private bool hovered;

        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;
        public event EventHandler OnMouseLeftClicked;
        public event EventHandler OnMouseRightClicked;
        public event EventHandler OnEnter;

        private MouseState prevState;

        public Rectangle Position
        {
            get
            {
                return position;
            }
        }

        protected bool IsVisable
        {
            get
            {
                return isVisable;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
        }

        public bool Hovered
        {
            get
            {
                return hovered;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Control(string name, int index, Rectangle rectangle, bool isVisable, bool isSelected)
        {
            this.name = name;
            this.index = index;
            this.isSelected = isSelected;
            position = rectangle;
            this.isVisable = isVisable;
        }

        public virtual void Update(GameTime gameTime)
        {
            MouseState state = Mouse.GetState();
            if (position.Intersects(new Rectangle(state.Position, new Point(15, 15))))
            {
                hovered = true;
                OnMouseEnter?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (hovered)
                {
                    OnMouseLeave?.Invoke(this, EventArgs.Empty);
                    hovered = false;
                }
            }

            if(hovered && state.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed)
            {
                OnMouseLeftClicked?.Invoke(this, EventArgs.Empty);
            }
            else if(hovered && state.RightButton == ButtonState.Released && prevState.RightButton == ButtonState.Pressed)
            {
                OnMouseRightClicked?.Invoke(this, EventArgs.Empty);
            }
            else if(isSelected && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                OnEnter?.Invoke(this, EventArgs.Empty);
            }

            prevState = state;
        }

        public void Select()
        {
            isSelected = true;
        }
        
        public void Unselect()
        {
            isSelected = false;
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public int CompareTo(Control other)
        {
            if (other == null)
                return 1;

            return index.CompareTo(other.index);
        }
    }
}
