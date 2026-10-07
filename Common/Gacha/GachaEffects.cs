using System.Collections.Generic;
using madake1.Common.Networking;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Gacha
{
    // Terraria-specific reward and notification adapter.
    public static class GachaEffects
    {
        public static void SpawnGachaItem(Player player, GachaResult result)
        {
            if (player == null || !player.active) return;
            Item.NewItem(player.GetSource_Misc("Gacha"), player.position, result.itemType, result.stack);
        }

        public static void SpawnGachaItems(Player player, List<GachaResult> results)
        {
            foreach (var result in results)
                SpawnGachaItem(player, result);
        }

        public static void ShowGoldText(Player player, GachaResult result)
        {
            if (player == null || !player.active || result == null) return;

            string itemName = Lang.GetItemNameValue(result.itemType);
            string text = $"{player.name} 抽到了 {itemName}！";
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, Color.Gold, text, true, false);
        }

        public static void BroadcastGoldText(Player player, int itemType)
        {
            if (Main.netMode != NetmodeID.Server) return;

            var packet = ModContent.GetInstance<MDK>().GetPacket();
            packet.Write((byte)MyMessageType.GoldDrop);
            packet.Write((byte)player.whoAmI);
            packet.Write(itemType);
            packet.Send();
        }
    }
}
