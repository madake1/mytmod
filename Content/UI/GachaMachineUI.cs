using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using madake1.Sets;
using System.Collections.Generic;
using System;
using Madake1.Content.UI;
using Terraria.GameContent;
using System.IO;
using System.Linq;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        public UIPanel mainPanel;
        public UIImageButton singleButton;
        public UIImageButton tenButton;
        public UIImageButton closeButton;

        // ===== 大奖展示列表 =====
        private UIElement rewardContainer; // 容器用于网格布局
        private UIPanel rewardPanel;
        private UIText rewardTitleText; // "大奖"文字标签
        private GachaMachineConfig currentMachine;
        private RewardDisplayItem selectedRewardItem; // 当前选中的奖励项（单选）
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
            rewardPanel.Left.Set(10f, 0f);
            rewardPanel.Top.Set(35f, 0f);
            // 计算：5列 * 60 + 4个间距 * 10 + 左右padding 20 = 300 + 40 + 20 = 360
            // 但需要更大的边距，所以设为380
            rewardPanel.Width.Set(380f, 0f);
            // 计算：文字标签30 + 2行 * 60 + 1个间距 * 10 + 上下padding 20 = 30 + 120 + 10 + 20 = 180
            rewardPanel.Height.Set(180f, 0f);
            rewardPanel.BackgroundColor = new Color(0, 0, 0, 120);
            rewardPanel.BorderColor = Color.Gold;
            mainPanel.Append(rewardPanel);

            // "大奖"文字标签
            rewardTitleText = new UIText("大奖");
            rewardTitleText.Left.Set(10f, 0f);
            rewardTitleText.Top.Set(10f, 0f);
            rewardTitleText.TextColor = Color.Gold;
            mainPanel.Append(rewardTitleText);

            // 奖励项容器（网格布局）
            rewardContainer = new UIElement();
            rewardContainer.Left.Set(10f, 0f);
            rewardContainer.Top.Set(15f, 0f); // 文字下方
            // 5列 * 60 + 4个间距 * 10 = 300 + 40 = 340
            rewardContainer.Width.Set(350f, 0f); // 增加宽度以容纳所有项
            // 2行 * 60 + 1个间距 * 10 = 120 + 10 = 130
            rewardContainer.Height.Set(140f, 0f);
            rewardPanel.Append(rewardContainer);
            

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
            selectedRewardItem = null; // 重置选中状态
            UpdateRewardTitleText(); // 重置文字为"大奖"
            if (rewardContainer == null) // UI还没初始化
            {
                return;
            }
            rewardContainer.RemoveAllChildren(); // 清空容器

            if (currentMachine == null || currentMachine.rewardsRealGold == null)
            {
                return;
            }

            // 网格布局：2行5列
            int columns = 5;
            int rows = 2;
            float itemSize = 60f;
            float spacing = 10f;
            int index = 0;
            
            foreach (var reward in currentMachine.rewardsRealGold)
            {
                if (index >= rows * columns) break; // 最多显示10个（2行5列）
                reward.ParseItemType();
                Item item = new Item();
                item.SetDefaults(reward.itemType);
                RewardDisplayItem uiItem = new RewardDisplayItem(
                    item,
                    reward.count,
                    Color.Gold,
                    selectable: true
                );
                
                // 计算网格位置
                int col = index % columns;
                int row = index / columns;
                uiItem.Left.Set(col * (itemSize + spacing), 0f);
                uiItem.Top.Set(row * (itemSize + spacing), 0f);
                
                // 设置点击事件，实现单选
                uiItem.OnLeftClick += (evt, elm) => {
                    SelectRewardItem(uiItem);
                };
                rewardContainer.Append(uiItem);              
                index++;
            }
           
        }
        
        // 单选奖励项
        private void SelectRewardItem(RewardDisplayItem item)
        {
            // 如果点击的是已选中的项，则取消选中
            if (selectedRewardItem == item && item.Selected)
            {
                item.Selected = false;
                selectedRewardItem = null;
                UpdateRewardTitleText(); // 更新文字为"大奖"
                return;
            }
            
            // 取消之前选中的项
            if (selectedRewardItem != null && selectedRewardItem != item)
            {
                selectedRewardItem.Selected = false;
            }
            
            // 选中新项
            selectedRewardItem = item;
            item.Selected = true;
            UpdateRewardTitleText(); // 更新文字为"已选择：大奖名"
        }
        
        // 更新奖励标题文字
        private void UpdateRewardTitleText()
        {
            if (rewardTitleText == null) return;
            
            if (selectedRewardItem != null && selectedRewardItem.Selected)
            {
                rewardTitleText.SetText($"已定轨：{selectedRewardItem.DisplayItem.Name}");
            }
            else
            {
                rewardTitleText.SetText("本期UP！");
            }
        }
    }
}
