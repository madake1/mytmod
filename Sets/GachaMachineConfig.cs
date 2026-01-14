using System;
using System.Collections.Generic;
using Terraria.ID;

public class GachaMachineConfig
{
    public int centerX;
    public int centerY;
    public int radius;

    public List<GachaReward> rewardsRealGold;
    public List<GachaReward> rewardsGold;
    public List<GachaReward> rewardsPurple;
    public List<GachaReward> rewardsBlue;

    // 增加方法获取机器唯一ID（用于玩家保底数据）
    public string GetMachineId()
    {
        return $"{centerX}_{centerY}";
    }
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
            var field = typeof(ItemID).GetField(name);
            if (field != null)
                itemType = Convert.ToInt32(field.GetValue(null));
            else
                itemType = 0;
        }
        else
        {
            // 自定义 ModItem，通过映射表
            if (ModItemMap.map.ContainsKey(item))
                itemType = ModItemMap.map[item];
            else
                itemType = 0;
        }
    }
}
