using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using madake1.Sets;
using System.Collections.Generic;
using Madake1.Content.UI;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        public UIPanel mainPanel;
        public UIImageButton singleButton;
        public UIImageButton tenButton;
        public UIImageButton closeButton;

        // ===== 大奖展示列表 =====
        private UIList rewardList;
        private UIScrollbar rewardScrollbar;
        private UIPanel rewardPanel;

        private GachaMachineConfig currentMachine;

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
            Append(mainPanel);

            // ================= 面板背景 =================
            UIImage panelBackground = new UIImage(
                ModContent.Request<Texture2D>("madake1/Content/UI/mianban")
            );
            panelBackground.Width.Set(0f, 1f);
            panelBackground.Height.Set(0f, 1f);
            mainPanel.Append(panelBackground);

            // ================= 大奖展示区域 =================
            rewardPanel = new UIPanel();
            rewardPanel.Left.Set(20f, 0f);
            rewardPanel.Top.Set(20f, 0f);
            rewardPanel.Width.Set(200f, 0f);
            rewardPanel.Height.Set(120f, 0f);
            rewardPanel.BackgroundColor = new Color(0, 0, 0, 120);
            rewardPanel.BorderColor = Color.Gold;
            mainPanel.Append(rewardPanel);

            rewardList = new UIList();
            rewardList.Width.Set(-20f, 1f);
            rewardList.Height.Set(0f, 1f);
            rewardList.ListPadding = 6f;
            rewardPanel.Append(rewardList);

            rewardScrollbar = new UIScrollbar();
            rewardScrollbar.Height.Set(0f, 1f);
            rewardScrollbar.Left.Set(-20f, 1f);
            rewardPanel.Append(rewardScrollbar);

            rewardList.SetScrollbar(rewardScrollbar);

            // ================= 按钮容器 =================
            UIElement buttonContainer = new UIElement();
            float buttonWidth = 58f;
            float buttonHeight = 30f;
            float spacing = 12f;

            buttonContainer.Width.Set(buttonWidth * 2 + spacing, 0f);
            buttonContainer.Height.Set(buttonHeight, 0f);
            buttonContainer.HAlign = 0.5f;
            buttonContainer.VAlign = 1f;
            buttonContainer.Top.Set(-10f, 0f);
            mainPanel.Append(buttonContainer);

            // ================= 单抽按钮 =================
            singleButton = new UIImageButton(
                ModContent.Request<Texture2D>("madake1/Content/UI/danchou")
            );
            singleButton.Width.Set(buttonWidth, 0f);
            singleButton.Height.Set(buttonHeight, 0f);
            buttonContainer.Append(singleButton);

            // ================= 十连按钮 =================
            tenButton = new UIImageButton(
                ModContent.Request<Texture2D>("madake1/Content/UI/shilian")
            );
            tenButton.Width.Set(buttonWidth, 0f);
            tenButton.Height.Set(buttonHeight, 0f);
            tenButton.Left.Set(buttonWidth + spacing, 0f);
            buttonContainer.Append(tenButton);

            // ================= 关闭按钮 =================
            closeButton = new UIImageButton(
                ModContent.Request<Texture2D>("madake1/Content/UI/guanbi")
            );
            closeButton.Width.Set(30f, 0f);
            closeButton.Height.Set(30f, 0f);
            closeButton.Left.Set(mainPanel.Width.Pixels - 38f, 0f);
            closeButton.Top.Set(8f, 0f);

            closeButton.OnLeftClick += (evt, elm) =>
            {
                var system = ModContent.GetInstance<GachaUISystem>();
                system.GachaUIInterface.SetState(null);
            };

            mainPanel.Append(closeButton);
        }

        // ================= 设置当前扭蛋机 & 加载大奖展示 =================
        public void SetMachine(GachaMachineConfig machine)
        {
            currentMachine = machine;

            if (rewardList == null) // UI还没初始化
                return;

            rewardList.Clear();

            if (currentMachine == null || currentMachine.rewardsRealGold == null)
                return;

            foreach (var reward in currentMachine.rewardsRealGold)
            {
                // 先解析 ItemType
                reward.ParseItemType();

                Item item = new Item();
                item.SetDefaults(reward.itemType); // 这里必须是 int

                RewardDisplayItem uiItem =
                    new RewardDisplayItem(
                        item,
                        reward.count,
                        Color.Gold,
                        selectable: true
                    );

                rewardList.Add(uiItem);
            }
        }
    }
}
