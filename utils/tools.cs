using System.Collections.Generic;
using madake1.Content.Gacha;
using Terraria;

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
}
