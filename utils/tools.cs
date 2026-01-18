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
        public static void SpawnGachaItem(Player player, GachaResult result)
        {
            if (player == null || !player.active) return;
            Item.NewItem(player.GetSource_Misc("Gacha"), player.position, result.itemType, result.stack);
        }

        public static void SpawnGachaItems(Player player, List<GachaResult> results)
        {
            foreach (var r in results)
                SpawnGachaItem(player, r);
        }

        // 公开方法，客户端或服务器调用
public static void ShowGoldText(Player player, GachaResult result)
{
    if (player == null || !player.active || result == null) return;

    string itemName = Lang.GetItemNameValue(result.itemType); // 获取物品显示名
    string text = $"{player.name} 抽到了 {itemName}！";

    // 只在客户端显示 UI
    if (Main.netMode != NetmodeID.Server)
        CombatText.NewText(player.Hitbox, Color.Gold, text, true, false);
}


        // 联机调用，广播给其他客户端
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


    }
}
