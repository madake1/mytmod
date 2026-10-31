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

        public GachaSpriteButton(Asset<Texture2D> idle, Asset<Texture2D> hover, float width = 58f, float height = 30f)
        {
            this.idle = idle;
            this.hover = hover;
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
            spriteBatch.Draw((IsMouseHovering ? hover : idle).Value, GetDimensions().ToRectangle(), Color.White);
        }
    }
}
