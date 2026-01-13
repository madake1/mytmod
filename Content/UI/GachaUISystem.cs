using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using madake1.Sets;
using System.IO;
using System;
using System.Linq;
namespace madake1.Content.UI
{
    public class GachaUISystem : ModSystem
    {
        internal UserInterface GachaUIInterface;
        internal GachaMachineUI GachaUIState;

        // 最近一次打开UI的家具位置
        private int uiTileX = -1;
        private int uiTileY = -1;
        
        // 保存待设置的machine，用于UI初始化后设置
        private GachaMachineConfig pendingMachine = null;
        
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

        public override void Load()
        {
            if (!Main.dedServ)
            {
                GachaUIState = new GachaMachineUI();
                GachaUIInterface = new UserInterface();
                GachaUIInterface.SetState(null);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // 如果UI打开了，检查玩家距离
            if (GachaUIInterface?.CurrentState != null && uiTileX >= 0 && uiTileY >= 0)
            {
                Player player = Main.LocalPlayer;
                float maxDistance = 100f; // 最大距离，可根据需要调整

                Vector2 playerPos = player.Center;
                Vector2 tilePos = new Vector2(uiTileX * 16 + 24, uiTileY * 16 + 24); 
                // 16像素为一个格子，+24让坐标大约在家具中心

                if (Vector2.Distance(playerPos, tilePos) > maxDistance)
                {
                    GachaUIInterface.SetState(null);
                    uiTileX = -1;
                    uiTileY = -1;
                }
            }



            GachaUIInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<Terraria.UI.GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new Terraria.UI.LegacyGameInterfaceLayer(
                    "madake1: GachaUI",
                    delegate
                    {
                        GachaUIInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    Terraria.UI.InterfaceScaleType.UI)
                );
            }
        }

        public void ToggleUI(int tileX = -1, int tileY = -1)
        {
            // #region agent log
            Log("GachaUISystem.cs:ToggleUI", "ToggleUI called", new { tileX, tileY, currentStateIsNull = GachaUIInterface?.CurrentState == null }, "A");
            // #endregion
            if (GachaUIInterface.CurrentState == null)
            {
                // #region agent log
                Log("GachaUISystem.cs:ToggleUI", "Setting UI state", new { }, "A");
                // #endregion
                GachaUIInterface.SetState(GachaUIState);
                uiTileX = tileX;
                uiTileY = tileY;
                
                // 如果有待设置的machine，在UI初始化后设置
                if (pendingMachine != null)
                {
                    // #region agent log
                    Log("GachaUISystem.cs:ToggleUI", "Setting pending machine after UI init", new { }, "A");
                    // #endregion
                    // 延迟一帧确保UI完全初始化
                    Main.QueueMainThreadAction(() =>
                    {
                        GachaUIState.SetMachine(pendingMachine);
                        pendingMachine = null;
                    });
                }
                
                // #region agent log
                Log("GachaUISystem.cs:ToggleUI", "UI state set", new { stateSet = true }, "A");
                // #endregion
            }
            else
            {
                GachaUIInterface.SetState(null);
                uiTileX = -1;
                uiTileY = -1;
                pendingMachine = null;
            }
        }
        public void OpenPanel(GachaMachineConfig machine, int tileX = -1, int tileY = -1)
        {
            // #region agent log
            Log("GachaUISystem.cs:OpenPanel", "OpenPanel called", new { tileX, tileY, machineIsNull = machine == null, rewardsCount = machine?.rewardsRealGold?.Count ?? 0 }, "A");
            // #endregion
            // 保存当前扭蛋机位置，用于距离检测
            uiTileX = tileX;
            uiTileY = tileY;

            // 如果UI还没打开，保存machine待设置
            if (GachaUIInterface?.CurrentState == null)
            {
                // #region agent log
                Log("GachaUISystem.cs:OpenPanel", "UI not open, saving pending machine", new { }, "A");
                // #endregion
                pendingMachine = machine;
            }
            else
            {
                // 把奖励数据传给 UI 状态
                // #region agent log
                Log("GachaUISystem.cs:OpenPanel", "Before SetMachine", new { }, "A");
                // #endregion
                GachaUIState.SetMachine(machine);
                // #region agent log
                Log("GachaUISystem.cs:OpenPanel", "After SetMachine", new { }, "A");
                // #endregion
            }
        }
    }
}
