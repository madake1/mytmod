using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using madake1.Common.Gacha;

namespace madake1.Content.UI
{
    public class GachaMachineUI : UIState
    {
        private const string TextureRoot = "madake1/Content/UI/";
        private const float UpRowLeft = 46f;
        private const float UpRowTop = 70f;
        private const float UpRowCenterX = 200f;
        private const float UpItemSize = 50f;
        private const float PermanentRowLeft = 46f;
        private const float PermanentRowTop = 160f;
        private const float PermanentItemSize = 40f;
        private const float ItemGap = 12f;
        private const float PullButtonWidth = 64f;
        public UIPanel mainPanel;
        public GachaSpriteButton singleButton;
        public GachaSpriteButton tenButton;
        public GachaSpriteButton closeButton;
        private UIText goldCounterText;
        private UIText selectedText;
        private UIElement upContainer;
        private UIElement permanentContainer;
        private RareRewardHint rareHint;
        private GachaMachineConfig currentMachine;
        private RewardDisplayItem selectedRewardItem;

        public override void OnInitialize()
        {
            mainPanel = new DraggableUIPanel();
            mainPanel.SetPadding(0);
            mainPanel.Left.Set(400f, 0f);
            mainPanel.Top.Set(180f, 0f);
            mainPanel.Width.Set(400f, 0f);
            mainPanel.Height.Set(278f, 0f);
            mainPanel.BackgroundColor = Color.Transparent;
            mainPanel.BorderColor = Color.Transparent;
            Append(mainPanel);

            UIImage panelBackground = new UIImage(ModContent.Request<Texture2D>(TextureRoot + "mianban"));
            panelBackground.Width.Set(400f, 0f);
            panelBackground.Height.Set(256f, 0f);
            mainPanel.Append(panelBackground);

            UIText upTitle = new UIText("本期UP");
            upTitle.Left.Set(46f, 0f);
            upTitle.Top.Set(50f, 0f);
            upTitle.TextColor = new Color(129, 81, 18);
            mainPanel.Append(upTitle);

            selectedText = new UIText("未定轨", 0.8f);
            selectedText.Left.Set(190f, 0f);
            selectedText.Top.Set(50f, 0f);
            selectedText.TextColor = new Color(115, 67, 19);
            mainPanel.Append(selectedText);

            upContainer = new UIElement();
            upContainer.Left.Set(UpRowLeft, 0f);
            upContainer.Top.Set(UpRowTop, 0f);
            upContainer.Width.Set(308f, 0f);
            upContainer.Height.Set(50f, 0f);
            mainPanel.Append(upContainer);

            UIText permanentTitle = new UIText("本期常驻", 0.8f);
            permanentTitle.Left.Set(46f, 0f);
            permanentTitle.Top.Set(128f, 0f);
            permanentTitle.TextColor = new Color(129, 81, 18);
            mainPanel.Append(permanentTitle);

            permanentContainer = new UIElement();
            permanentContainer.Left.Set(PermanentRowLeft, 0f);
            permanentContainer.Top.Set(PermanentRowTop, 0f);
            permanentContainer.Width.Set(308f, 0f);
            permanentContainer.Height.Set(PermanentItemSize, 0f);
            mainPanel.Append(permanentContainer);

            rareHint = new RareRewardHint(ModContent.Request<Texture2D>(TextureRoot + "xiyou_shanguang"));
            rareHint.Left.Set(335f, 0f);
            rareHint.Top.Set(213f, 0f);
            mainPanel.Append(rareHint);

            goldCounterText = new UIText("保底 0/80", 0.85f);
            goldCounterText.Left.Set(240f, 0f);
            goldCounterText.Top.Set(210f, 0f);
            goldCounterText.TextColor = new Color(255, 223, 131);
            mainPanel.Append(goldCounterText);

            singleButton = CreateButton("danchou", 60f);
            singleButton.OnLeftClick += (_, _) => Pull(1);
            tenButton = CreateButton("shilian", 276f);
            tenButton.OnLeftClick += (_, _) => Pull(10);
            closeButton = new GachaSpriteButton(
                ModContent.Request<Texture2D>(TextureRoot + "guanbi_chazi"),
                ModContent.Request<Texture2D>(TextureRoot + "guanbi_chazi_hover"),
                24f, 24f);
            closeButton.Left.Set(338f, 0f);
            closeButton.Top.Set(41f, 0f);
            mainPanel.Append(closeButton);
            closeButton.OnLeftClick += (_, _) => ModContent.GetInstance<GachaUISystem>().GachaUIInterface.SetState(null);
        }

        private GachaSpriteButton CreateButton(string name, float left)
        {
            var button = new GachaSpriteButton(
                ModContent.Request<Texture2D>(TextureRoot + name),
                ModContent.Request<Texture2D>(TextureRoot + name + "_hover"),
                width: PullButtonWidth, stretchColumn: 30);
            button.Left.Set(left, 0f);
            button.Top.Set(248f, 0f);
            mainPanel.Append(button);
            return button;
        }

        private void Pull(int count)
        {
            if (currentMachine == null) return;
            Player player = Main.LocalPlayer;
            if (!GachaWorkflow.TryPull(player, currentMachine, count, out string error))
            {
                Main.NewText(error, Color.Red);
                return;
            }
            UpdateGoldCounter(player.GetModPlayer<GachaPlayer>().GetData(currentMachine.GetMachineId()).pullsSinceLastGold, 80);
        }

        public void UpdateGoldCounter(int current, int max)
        {
            goldCounterText?.SetText($"保底 {current}/{max}");
        }

        public void SetMachine(GachaMachineConfig machine)
        {
            currentMachine = machine;
            selectedRewardItem = null;
            UpdateSelectedText();
            if (upContainer == null) return;
            upContainer.RemoveAllChildren();
            permanentContainer.RemoveAllChildren();
            rareHint.SetRewards(machine?.rewardsPurple);
            if (machine == null) return;

            int? savedIndex = Main.LocalPlayer.GetModPlayer<GachaPlayer>().GetSelectedReward(machine.GetMachineId());
            if (machine.rewardsRealGold != null)
            {
                int visibleCount = Math.Min(machine.rewardsRealGold.Count, 5);
                float firstItemX = UpRowCenterX - (visibleCount * UpItemSize + (visibleCount - 1) * ItemGap) / 2f - UpRowLeft;
                for (int index = 0; index < visibleCount; index++)
                {
                    var reward = machine.rewardsRealGold[index];
                    reward.ParseItemType();
                    if (reward.itemType <= 0) continue;
                    Item item = new Item();
                    item.SetDefaults(reward.itemType);
                    var uiItem = new RewardDisplayItem(item, reward.count, new Color(218, 169, 39), selectable: true, size: UpItemSize);
                    uiItem.Left.Set(firstItemX + index * (UpItemSize + ItemGap), 0f);
                    int rewardIndex = index;
                    uiItem.OnLeftClick += (_, _) => SelectRewardItem(uiItem, rewardIndex);
                    if (savedIndex == index)
                    {
                        uiItem.Selected = true;
                        selectedRewardItem = uiItem;
                    }
                    upContainer.Append(uiItem);
                }
            }
            if (machine.rewardsGold != null)
            {
                for (int index = 0; index < Math.Min(machine.rewardsGold.Count, 6); index++)
                {
                    var reward = machine.rewardsGold[index];
                    reward.ParseItemType();
                    if (reward.itemType <= 0) continue;
                    Item item = new Item();
                    item.SetDefaults(reward.itemType);
                    var uiItem = new RewardDisplayItem(item, reward.count, new Color(171, 131, 53), size: PermanentItemSize);
                    uiItem.Left.Set(index * (PermanentItemSize + ItemGap), 0f);
                    permanentContainer.Append(uiItem);
                }
            }
            UpdateSelectedText();
            var data = Main.LocalPlayer.GetModPlayer<GachaPlayer>().GetData(machine.GetMachineId());
            UpdateGoldCounter(data.pullsSinceLastGold, 80);
        }

        private void SelectRewardItem(RewardDisplayItem item, int selectedIndex)
        {
            if (currentMachine == null) return;
            var player = Main.LocalPlayer.GetModPlayer<GachaPlayer>();
            if (selectedRewardItem == item && item.Selected)
            {
                item.Selected = false;
                selectedRewardItem = null;
                player.SetSelectedReward(currentMachine.GetMachineId(), -1);
            }
            else
            {
                if (selectedRewardItem != null) selectedRewardItem.Selected = false;
                selectedRewardItem = item;
                item.Selected = true;
                player.SetSelectedReward(currentMachine.GetMachineId(), selectedIndex);
            }
            UpdateSelectedText();
        }

        private void UpdateSelectedText()
        {
            selectedText?.SetText(selectedRewardItem?.Selected == true
                ? $"已定轨：{selectedRewardItem.DisplayItem.Name}"
                : "未定轨");
        }
    }
}
