using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using madake1.Sets;
using System;
using ReLogic.Content;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        public UIPanel mainPanel;
        public UIImageButton singleButton;
        public UIImageButton tenButton;
        public UIImageButton closeButton; // 关闭按钮
        private UIElement realGoldContainer;// 容器用于显示 RealGold 奖励

        public override void OnInitialize()
        {
            // ================= 主面板 =================
            mainPanel = new UIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Left.Set(400f, 0f);
            mainPanel.Top.Set(200f, 0f);
            mainPanel.Width.Set(400f, 0f);
            mainPanel.Height.Set(256f, 0f);
            mainPanel.BackgroundColor = Color.Transparent;
            mainPanel.BorderColor = Color.Transparent;

            // ================= 面板背景 =================
            UIImage panelBackground = new UIImage(
                ModContent.Request<Texture2D>("madake1/Content/UI/mianban")
            );
            panelBackground.Width.Set(0f, 1f);
            panelBackground.Height.Set(0f, 1f);
            mainPanel.Append(panelBackground);

            Append(mainPanel);

            // ================= 按钮容器 =================
            UIElement buttonContainer = new UIElement();
            float buttonWidth = 58f;
            float buttonHeight = 30f;
            float spacing = 12f;

            float containerWidth = buttonWidth * 2 + spacing;
            float containerHeight = buttonHeight;

            buttonContainer.Width.Set(containerWidth, 0f);
            buttonContainer.Height.Set(containerHeight, 0f);

            buttonContainer.HAlign = 0.5f; // 水平居中
            buttonContainer.VAlign = 1f;   // 底部对齐
            buttonContainer.Top.Set(-10f, 0f); // 向上偏移 10 像素

            mainPanel.Append(buttonContainer);

            // ================= 单抽按钮 =================
            singleButton = new UIImageButton(
                ModContent.Request<Texture2D>("madake1/Content/UI/danchou")
            );
            singleButton.Width.Set(buttonWidth, 0f);
            singleButton.Height.Set(buttonHeight, 0f);
            singleButton.Left.Set(0f, 0f);
            singleButton.Top.Set(0f, 0f);

            // ================= 十连按钮 =================
            tenButton = new UIImageButton(
                ModContent.Request<Texture2D>("madake1/Content/UI/shilian")
            );
            tenButton.Width.Set(buttonWidth, 0f);
            tenButton.Height.Set(buttonHeight, 0f);
            tenButton.Left.Set(buttonWidth + spacing, 0f); // 紧跟单抽按钮 + 间距
            tenButton.Top.Set(0f, 0f);

            buttonContainer.Append(singleButton);
            buttonContainer.Append(tenButton);

            // ================= 关闭按钮 =================
            closeButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/guanbi"));
            closeButton.Width.Set(30f, 0f);  // 贴图宽度
            closeButton.Height.Set(30f, 0f); // 贴图高度

            // 放在右上角，距离边框 8 像素
            closeButton.Left.Set(mainPanel.Width.Pixels - 30f - 8f, 0f);
            closeButton.Top.Set(8f, 0f);

            // 点击关闭 UI
            closeButton.OnLeftClick += (evt, elm) =>
            {
                var gachaSystem = ModContent.GetInstance<GachaUISystem>();
                if (gachaSystem.GachaUIInterface.CurrentState != null)
                {
                    gachaSystem.GachaUIInterface.SetState(null);
                }
            };

            mainPanel.Append(closeButton);
        }



    }
}
