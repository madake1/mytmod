using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

public class GachaMachineConfig
{
    public int centerX;
    public int centerY;
    public int radius;

    public List<GachaReward> rewardsRealGold;
    public List<GachaReward> rewardsGold;
    public List<GachaReward> rewardsPurple;
    public List<GachaReward> rewardsBlue;
}

// 奖励数据类
public class GachaReward
{
    public string item; // JSON 中的短名字
    public int itemType; // 解析后的 ItemType
    public int count;

    public void ParseItemType()
    {
        if (item.StartsWith("Terraria.ID.ItemID."))
        {
            string name = item.Replace("Terraria.ID.ItemID.", "");
            var field = typeof(Terraria.ID.ItemID).GetField(name);
            if (field != null)
                itemType = (int)field.GetValue(null);
        }
        else
        {
            // 自定义 ModItem，通过映射表
            if (ModItemMap.map.ContainsKey(item))
                itemType = ModItemMap.map[item];
            else
                itemType = 0; // 避免报错
        }
    }
}
