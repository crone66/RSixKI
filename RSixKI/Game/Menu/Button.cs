using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class Button : Control
    {
        private Texture2D texture;
        private SpriteFont font;
        private Color textColor;
        private Color hoverTextColor;
        private string text;
        private Vector2 textPosition;

        public Button(string name, int index, Rectangle rectangle, bool isVisable, bool isSelected, Texture2D texture, SpriteFont font, Color textColor, Color hoverTextColor, string text) : base(name, index, rectangle, isVisable, isSelected)
        {
            this.texture = texture;
            this.font = font;
            this.text = text;
            this.textColor = textColor;
            this.hoverTextColor = hoverTextColor;

            textPosition = rectangle.Center.ToVector2() - (font.MeasureString(text) / 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.DrawString(font, text, textPosition, Hovered || isSelected ? hoverTextColor : textColor );
        }
    }
}
