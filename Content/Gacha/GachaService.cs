using System.Collections.Generic;
using Terraria;

namespace madake1.Content.Gacha
{
    public static class GachaService
    {
        public static List<GachaResult> DoPull(Player player, int times)
        {
            var results = new List<GachaResult>();

            for (int i = 0; i < times; i++)
            {
                // TODO: 以后这里放保底和概率逻辑
                // 先随机生成一个蓝色道具作为占位
                int itemType = Terraria.ID.ItemID.WoodenSword;
                int stack = 1;
                GachaQuality quality = GachaQuality.Blue;

                results.Add(new GachaResult(itemType, stack, quality));

                // 同时直接发给玩家（方便测试）
                Item.NewItem(player.GetSource_Misc("GachaTest"), player.position, itemType, stack);
            }

            return results;
        }
    }
}
