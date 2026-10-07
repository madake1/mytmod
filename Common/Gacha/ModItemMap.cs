namespace madake1.Common.Gacha;
using System.Collections.Generic;
using Terraria.ModLoader;
using madake1.Content.Items.Weapons.Melee; // 引入你 ModItem 命名空间

public static class ModItemMap
{
    public static Dictionary<string, int> map = new Dictionary<string, int>()
    {
        { "Jinxi", ModContent.ItemType<Jinxi>() },
        { "Changli", ModContent.ItemType<Changli>() },
        { "Cunhaojian", ModContent.ItemType<Cunhaojian>() },
        // 如果有其他自定义 ModItem，也在这里加：
        // { "OtherItem", ModContent.ItemType<OtherItem>() },
    };
}
