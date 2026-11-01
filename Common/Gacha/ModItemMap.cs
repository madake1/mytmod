namespace madake1.Common.Gacha;
using System.Collections.Generic;
using Terraria.ModLoader;
using madake1.Content.Items.Weapons.Melee; // 引入你 ModItem 命名空间
using madake1.Content.Items.ArmorSets;

public static class ModItemMap
{
    public static Dictionary<string, int> map = new Dictionary<string, int>()
    {
        { "test1", ModContent.ItemType<test1>() },
        { "test2", ModContent.ItemType<test2>() },
        { "Cunhaojian", ModContent.ItemType<Cunhaojian>() },
        { "PlatinumArmorSetBundle", ModContent.ItemType<PlatinumArmorSetBundle>() },
        { "GoldArmorSetBundle", ModContent.ItemType<GoldArmorSetBundle>() },
        { "RoninArmorSetBundle", ModContent.ItemType<RoninArmorSetBundle>() },
        { "WolfArmorSetBundle", ModContent.ItemType<WolfArmorSetBundle>() },
        // 如果有其他自定义 ModItem，也在这里加：
        // { "OtherItem", ModContent.ItemType<OtherItem>() },
    };
}
