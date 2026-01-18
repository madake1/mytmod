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
        public static void ShowGoldText(Player player)
        {
            if (player == null || !player.active) return;

            string text = $"{player.name} 出金啦！";

            // 单机或客户端显示
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, Color.Gold, text, true, false);
        }

        // 联机调用，广播给其他客户端
        public static void BroadcastGoldText(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                var packet = ModContent.GetInstance<madake1.MDK>().GetPacket();
                packet.Write((byte)MyMessageType.GoldDrop);
                packet.Write((byte)player.whoAmI);
                packet.Send();
            }
        }

    }
}
