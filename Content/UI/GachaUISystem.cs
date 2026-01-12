using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using madake1.Sets;
namespace madake1.Content.UI
{
    public class GachaUISystem : ModSystem
    {
        internal UserInterface GachaUIInterface;
        internal GachaMachineUI GachaUIState;

        // 最近一次打开UI的家具位置
        private int uiTileX = -1;
        private int uiTileY = -1;

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
            if (GachaUIInterface.CurrentState == null)
            {
                GachaUIInterface.SetState(GachaUIState);
                uiTileX = tileX;
                uiTileY = tileY;
            }
            else
            {
                GachaUIInterface.SetState(null);
                uiTileX = -1;
                uiTileY = -1;
            }
        }
        public void OpenPanel(GachaMachineConfig machine, int tileX = -1, int tileY = -1)
        {
            // 保存当前扭蛋机位置，用于距离检测
            uiTileX = tileX;
            uiTileY = tileY;

            // 把奖励数据传给 UI 状态
            GachaUIState.SetMachine(machine);

            // 打开 UI
            GachaUIInterface.SetState(GachaUIState);
        }
    }
}
