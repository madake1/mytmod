using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using madake1.Sets;
using System.Collections.Generic;
using Madake1.Content.UI;
using Terraria.GameContent;
using madake1.Content.Items.Zhanlipin;
using madake1.Content.Gacha;
using madake1.Content.Net;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        public UIPanel mainPanel;
        public UIImageButton singleButton;
        public UIImageButton tenButton;
        public UIImageButton closeButton;
        private UIText goldCounterText; // 计数文本


        // ===== 大奖展示列表 =====
        private UIElement rewardContainer; 
        private UIPanel rewardPanel;
        private UIText rewardTitleText;
        private GachaMachineConfig currentMachine;
        private RewardDisplayItem selectedRewardItem; // 当前选中的奖励项（单选）

        public override void OnInitialize()
        {
            // ================= 主面板 =================
            mainPanel = new DraggableUIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Left.Set(400f, 0f);
            mainPanel.Top.Set(200f, 0f);
            mainPanel.Width.Set(400f, 0f);
            mainPanel.Height.Set(256f, 0f);
            mainPanel.BackgroundColor = Color.Transparent;
            mainPanel.BorderColor = Color.Transparent;
            Append(mainPanel);

            // ================= 面板背景 =================
            UIImage panelBackground = new UIImage(ModContent.Request<Texture2D>("madake1/Content/UI/mianban"));
            panelBackground.Width.Set(0f, 1f);
            panelBackground.Height.Set(0f, 1f);
            mainPanel.Append(panelBackground);

            // ================= 大奖展示区域 =================
            rewardPanel = new UIPanel();
            rewardPanel.Left.Set(10f, 0f);
            rewardPanel.Top.Set(35f, 0f);
            rewardPanel.Width.Set(380f, 0f);
            rewardPanel.Height.Set(180f, 0f);
            rewardPanel.BackgroundColor = new Color(0, 0, 0, 120);
            rewardPanel.BorderColor = Color.Gold;
            mainPanel.Append(rewardPanel);

            // "大奖"文字标签
            rewardTitleText = new UIText("本期UP！");
            rewardTitleText.Left.Set(10f, 0f);
            rewardTitleText.Top.Set(10f, 0f);
            rewardTitleText.TextColor = Color.Gold;
            mainPanel.Append(rewardTitleText);

            // 奖励项容器（网格布局）
            rewardContainer = new UIElement();
            rewardContainer.Left.Set(10f, 0f);
            rewardContainer.Top.Set(15f, 0f);
            rewardContainer.Width.Set(350f, 0f);
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
            singleButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/danchou"));
            singleButton.Width.Set(buttonWidth, 0f);
            singleButton.Height.Set(buttonHeight, 0f);
            buttonContainer.Append(singleButton);
            singleButton.OnLeftClick += (_, _) =>
            {
                Player player = Main.LocalPlayer;
                if (!TryConsumeJinQiu(player, 1)) return;
                var results = GachaService.DoPull(player, currentMachine, 1);
                GachaRequest.Request(player, currentMachine, results);

                var data = player.GetModPlayer<GachaPlayer>().GetData(currentMachine.GetMachineId());
                UpdateGoldCounter(data.pullsSinceLastGold, 80);
            };

            // ================= 十连按钮 =================
            tenButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/shilian"));
            tenButton.Width.Set(buttonWidth, 0f);
            tenButton.Height.Set(buttonHeight, 0f);
            tenButton.Left.Set(buttonWidth + spacing, 0f);
            buttonContainer.Append(tenButton);
            tenButton.OnLeftClick += (_, _) =>
            {
                Player player = Main.LocalPlayer;
                if (!TryConsumeJinQiu(player, 10)) return;
                var results = GachaService.DoPull(player, currentMachine, 10);
                GachaRequest.Request(player, currentMachine, results);

                var data = player.GetModPlayer<GachaPlayer>().GetData(currentMachine.GetMachineId());
                UpdateGoldCounter(data.pullsSinceLastGold, 80);
            };

            // ================= 关闭按钮 =================
            closeButton = new UIImageButton(ModContent.Request<Texture2D>("madake1/Content/UI/guanbi"));
            closeButton.Width.Set(30f, 0f);
            closeButton.Height.Set(30f, 0f);
            closeButton.Left.Set(mainPanel.Width.Pixels - 38f, 0f);
            closeButton.Top.Set(8f, 0f);
            closeButton.OnLeftClick += (_, _) =>
            {
                ModContent.GetInstance<GachaUISystem>().GachaUIInterface.SetState(null);
            };
            mainPanel.Append(closeButton);

            // ================= 金球计数文本 =================
            goldCounterText = new UIText("0/80");
            goldCounterText.Left.Set(mainPanel.Width.Pixels - 100f, 0f); // 关闭按钮左边约 60-70 px
            goldCounterText.Top.Set(8f, 0f); // 与关闭按钮垂直对齐
            goldCounterText.TextColor = Color.Gold;
            mainPanel.Append(goldCounterText);

        }
        // ================= 更新金球计数 =================
        public void UpdateGoldCounter(int current, int max)
        {
            if (goldCounterText != null)
            {
                goldCounterText.SetText($"{current}/{max}");
            }
        }


        // ================= 设置当前扭蛋机 & 加载大奖展示 =================
        public void SetMachine(GachaMachineConfig machine)
        {
            currentMachine = machine;
            selectedRewardItem = null;
            UpdateRewardTitleText();

            if (rewardContainer == null) return;
            rewardContainer.RemoveAllChildren();
            if (currentMachine == null || currentMachine.rewardsRealGold == null) return;

            // 读取玩家上次选择
            int? savedIndex = Main.LocalPlayer.GetModPlayer<GachaPlayer>().GetSelectedReward(currentMachine.GetMachineId());

            int columns = 5;
            int rows = 2;
            float itemSize = 60f;
            float spacing = 10f;
            int index = 0;

            foreach (var reward in currentMachine.rewardsRealGold)
            {
                if (index >= rows * columns) break;
                reward.ParseItemType();
                Item item = new Item();
                item.SetDefaults(reward.itemType);
                RewardDisplayItem uiItem = new RewardDisplayItem(item, reward.count, Color.Gold, selectable: true);

                int col = index % columns;
                int row = index / columns;
                uiItem.Left.Set(col * (itemSize + spacing), 0f);
                uiItem.Top.Set(row * (itemSize + spacing), 0f);

                uiItem.OnLeftClick += (evt, elm) => { SelectRewardItem(uiItem); };

                // 初始化选中状态
                if (savedIndex.HasValue && index == savedIndex.Value)
                {
                    uiItem.Selected = true;
                    selectedRewardItem = uiItem;
                    UpdateRewardTitleText();
                }

                rewardContainer.Append(uiItem);
                index++;
            }
            var data = Main.LocalPlayer.GetModPlayer<GachaPlayer>().GetData(machine.GetMachineId());
            UpdateGoldCounter(data.pullsSinceLastGold, 80);

        }

        // ================ 选择奖励项（单选逻辑） =================
        private void SelectRewardItem(RewardDisplayItem item)
        {
            Player player = Main.LocalPlayer;
            string machineId = currentMachine.GetMachineId();

            // 点击已选中项取消选择
            if (selectedRewardItem == item && item.Selected)
            {
                item.Selected = false;
                selectedRewardItem = null;
                UpdateRewardTitleText();
                player.GetModPlayer<GachaPlayer>().SetSelectedReward(machineId, -1);
                return;
            }

            // 取消之前选中的项
            if (selectedRewardItem != null && selectedRewardItem != item)
                selectedRewardItem.Selected = false;

            // 选中新项
            selectedRewardItem = item;
            item.Selected = true;
            UpdateRewardTitleText();

            // 保存选中索引
            int selectedIndex = currentMachine.rewardsRealGold.FindIndex(r => r.itemType == item.DisplayItem.type);
            player.GetModPlayer<GachaPlayer>().SetSelectedReward(machineId, selectedIndex >= 0 ? selectedIndex : -1);
        }

        // ================== 更新大奖文字 =================
        private void UpdateRewardTitleText()
        {
            if (rewardTitleText == null) return;
            if (selectedRewardItem != null && selectedRewardItem.Selected)
                rewardTitleText.SetText($"已定轨：{selectedRewardItem.DisplayItem.Name}");
            else
                rewardTitleText.SetText("本期UP！");
        }

        // ================== 检查金球数量 =================
        private bool TryConsumeJinQiu(Player player, int count)
        {
            int jinQiuType = ModContent.ItemType<JinQiu>();
            if (player.CountItem(jinQiuType) < count)
            {
                Main.NewText("金球不够了辣！", Color.Red);
                return false;
            }
            for (int i = 0; i < count; i++) player.ConsumeItem(jinQiuType);
            return true;
        }
    }
}
