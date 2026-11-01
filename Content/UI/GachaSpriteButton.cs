using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace madake1.Content.UI
{
    public class GachaSpriteButton : UIElement
    {
        private readonly Asset<Texture2D> idle;
        private readonly Asset<Texture2D> hover;
        private readonly int stretchColumn;

        public GachaSpriteButton(Asset<Texture2D> idle, Asset<Texture2D> hover, float width = 58f, float height = 30f, int stretchColumn = -1)
        {
            this.idle = idle;
            this.hover = hover;
            this.stretchColumn = stretchColumn;
            Width.Set(width, 0f);
            Height.Set(height, 0f);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (evt.Target == this)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
            Texture2D texture = (IsMouseHovering ? hover : idle).Value;
            Rectangle bounds = GetDimensions().ToRectangle();
            if (stretchColumn < 0 || bounds.Width <= texture.Width)
            {
                spriteBatch.Draw(texture, bounds, Color.White);
                return;
            }

            int rightWidth = texture.Width - stretchColumn - 1;
            int middleWidth = bounds.Width - stretchColumn - rightWidth;
            spriteBatch.Draw(texture,
                new Rectangle(bounds.X, bounds.Y, stretchColumn, bounds.Height),
                new Rectangle(0, 0, stretchColumn, texture.Height), Color.White);
            spriteBatch.Draw(texture,
                new Rectangle(bounds.X + stretchColumn, bounds.Y, middleWidth, bounds.Height),
                new Rectangle(stretchColumn, 0, 1, texture.Height), Color.White);
            spriteBatch.Draw(texture,
                new Rectangle(bounds.Right - rightWidth, bounds.Y, rightWidth, bounds.Height),
                new Rectangle(stretchColumn + 1, 0, rightWidth, texture.Height), Color.White);
        }

    }
}
