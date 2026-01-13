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
        public UIList rewardList;
        private UIScrollbar rewardScrollbar;
        private UIPanel rewardPanel;

        private GachaMachineConfig currentMachine;
        
        // #region agent log
        private static void Log(string location, string message, object data, string hypothesisId = "")
        {
            try
            {
                string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId, location, message, data, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
                File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
            }
            catch { }
        }
        // #endregion

        public override void OnInitialize()
        {
            // #region agent log
            Log("GachaMachineUI.cs:OnInitialize", "OnInitialize called", new { }, "A");
            // #endregion
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
            
            // #region agent log
            Log("GachaMachineUI.cs:OnInitialize", "rewardList initialized", new { rewardListIsNull = rewardList == null }, "A");
            // #endregion

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
            // #region agent log
            Log("GachaMachineUI.cs:SetMachine", "SetMachine entry", new { machineIsNull = machine == null, rewardListIsNull = rewardList == null, rewardsCount = machine?.rewardsRealGold?.Count ?? 0 }, "A");
            // #endregion
            currentMachine = machine;

            if (rewardList == null) // UI还没初始化
            {
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "rewardList is null, returning early", new { }, "A");
                // #endregion
                return;
            }

            rewardList.Clear();

            if (currentMachine == null || currentMachine.rewardsRealGold == null)
            {
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "machine or rewardsRealGold is null", new { machineIsNull = currentMachine == null, rewardsIsNull = currentMachine?.rewardsRealGold == null }, "A");
                // #endregion
                return;
            }

            // #region agent log
            Log("GachaMachineUI.cs:SetMachine", "Starting reward loop", new { rewardsCount = currentMachine.rewardsRealGold.Count }, "A");
            // #endregion
            foreach (var reward in currentMachine.rewardsRealGold)
            {
                // 先解析 ItemType
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "Before ParseItemType", new { item = reward.item, itemType = reward.itemType }, "D");
                // #endregion
                reward.ParseItemType();
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "After ParseItemType", new { item = reward.item, itemType = reward.itemType }, "D");
                // #endregion
                


                Item item = new Item();
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "Item created", new { itemTypeBeforeSetDefaults = item.type, rewardItemType = reward.itemType }, "E");
                // #endregion
                
                item.SetDefaults(reward.itemType); // 这里必须是 int
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "After SetDefaults", new { itemType = item.type, itemName = item.Name, rewardItemType = reward.itemType }, "E");
                // #endregion
                
                var tex = TextureAssets.Item[item.type].Value;
                Console.WriteLine($"Texture fetched for {item.Name}: {tex.Width}x{tex.Height}");
                Console.WriteLine($"Item: {reward.item}, Type: {reward.itemType}");

                RewardDisplayItem uiItem =
                    new RewardDisplayItem(
                        item,
                        reward.count,
                        Color.Gold,
                        selectable: true
                    );

                rewardList.Add(uiItem);
                // #region agent log
                Log("GachaMachineUI.cs:SetMachine", "Item added to list", new { itemType = item.type, itemName = item.Name }, "A");
                // #endregion
            }
            // #region agent log
            Log("GachaMachineUI.cs:SetMachine", "SetMachine exit", new { itemsAdded = currentMachine.rewardsRealGold.Count }, "A");
            // #endregion
        }
    }
}
