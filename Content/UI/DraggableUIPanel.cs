using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;

namespace madake1.Content.UI
{
    public class DraggableUIPanel : UIPanel
    {
        private bool dragging;
        private Vector2 dragOffset;

        public DraggableUIPanel()
        {
            OnLeftMouseDown += LeftMouseDown;
            OnLeftMouseUp += LeftMouseUp;
        }

        private void LeftMouseDown(UIMouseEvent evt, UIElement listeningElement)
        {
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        private void LeftMouseUp(UIMouseEvent evt, UIElement listeningElement)
        {
            dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                Vector2 mouse = Main.MouseScreen;
                Left.Set(mouse.X - dragOffset.X, 0f);
                Top.Set(mouse.Y - dragOffset.Y, 0f);
                Recalculate();
            }
        }
    }
}
