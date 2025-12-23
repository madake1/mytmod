using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        public UIPanel mainPanel;
        public UIImageButton singleButton;
        public UIImageButton tenButton;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Left.Set(400f, 0f);
            mainPanel.Top.Set(200f, 0f);
            mainPanel.Width.Set(300f, 0f);
            mainPanel.Height.Set(200f, 0f);
            Append(mainPanel);

            // 单抽按钮
            singleButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/danchou"));
            singleButton.Left.Set(20f, 0f);
            singleButton.Top.Set(50f, 0f);
            mainPanel.Append(singleButton);

            // 十连按钮
            tenButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/shilian"));
            tenButton.Left.Set(150f, 0f);
            tenButton.Top.Set(50f, 0f);
            mainPanel.Append(tenButton);
        }
    }
}
