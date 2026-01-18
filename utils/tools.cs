using System.Collections.Generic;
using madake1.Content.Gacha;
using Microsoft.Xna.Framework;
using Stubble.Core.Contexts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using madake1.Content.Net;
namespace madake1.Utils
{

    public static class Tools
    {
        // ================ 生成物品 =================
        public static void SpawnGachaItem(Player player, GachaResult result)
        {
            if (player == null || !player.active) return;
            Item.NewItem(player.GetSource_Misc("Gacha"), player.position, result.itemType, result.stack);
        }
        // ================ 生成物品列表 =================
        public static void SpawnGachaItems(Player player, List<GachaResult> results)
        {
            foreach (var r in results)
                SpawnGachaItem(player, r);
        }

        // ================ 出金文字 =================
        public static void ShowGoldText(Player player, GachaResult result)
        {
            if (player == null || !player.active || result == null) return;

            string itemName = Lang.GetItemNameValue(result.itemType); // 获取物品显示名
            string text = $"{player.name} 抽到了 {itemName}！";

            // 只在客户端显示 UI
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, Color.Gold, text, true, false);
        }


        // ================ 广播出金文字 =================
        public static void BroadcastGoldText(Player player, int itemType)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var packet = ModContent.GetInstance<MDK>().GetPacket();
                packet.Write((byte)Content.Net.MyMessageType.GoldDrop);
                packet.Write((byte)player.whoAmI);
                packet.Write(itemType); // 新增物品类型
                packet.Send();
            }
        }

        // ================ 出紫文字 =================
        public static void ShowPurpleText(Player player, GachaResult result)
        {
            if (player == null || !player.active || result == null) return;

            string itemName = Lang.GetItemNameValue(result.itemType); // 获取物品显示名
            string text = $"{player.name} 抽到了 {itemName}！";

            // 只在客户端显示 UI
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, Color.MediumPurple, text, true, false);
        }
        // ================ 广播出紫文字 =================
        public static void BroadcastPurpleText(Player player, int itemType)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var packet = ModContent.GetInstance<MDK>().GetPacket();
                packet.Write((byte)Content.Net.MyMessageType.GoldDrop); // 复用 GoldDrop 类型
                packet.Write((byte)player.whoAmI);
                packet.Write(itemType);
                packet.Write((byte)GachaQuality.Purple); // 明确告诉客户端这是紫色
                packet.Send();
            }
        }


    }
}
