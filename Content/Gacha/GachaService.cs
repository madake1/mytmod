using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace madake1.Content.Gacha
{
    public enum GachaQuality
    {
        Blue,
        Purple,
        Gold,
        RealGold
    }

    public class GachaResult
    {
        public int itemType;
        public int stack;
        public GachaQuality quality;

        public GachaResult(int itemType, int stack, GachaQuality quality)
        {
            this.itemType = itemType;
            this.stack = stack;
            this.quality = quality;
        }
    }

    public static class GachaService
    {
        public static List<GachaResult> DoPull(Player player, GachaMachineConfig config, int times)
        {
            var gachaPlayer = player.GetModPlayer<GachaPlayer>();
            var results = new List<GachaResult>();
            var machineId = config.GetMachineId();

            for (int i = 0; i < times; i++)
            {
                // 获取玩家当前抽卡数据（用于保底计数）
                var data = gachaPlayer.GetData(machineId);

                // 计算品质
                GachaQuality quality = GachaQuality.Blue;

                // 每80抽必出金色
                if (data.pullsSinceLastGold >= 79)
                {
                    quality = GachaQuality.Gold;
                }
                // 每10抽必出紫色（如果不是金色）
                else if (data.pullsSinceLastPurple >= 9)
                {
                    quality = GachaQuality.Purple;
                }
                else
                {
                    int roll = Main.rand.Next(100);
                    if (roll < 1 && config.rewardsRealGold.Count > 0) quality = GachaQuality.RealGold; // 1%大奖
                    else if (roll < 10 && config.rewardsGold.Count > 0) quality = GachaQuality.Gold;   // 10%金色
                    else if (roll < 30 && config.rewardsPurple.Count > 0) quality = GachaQuality.Purple; // 30%紫色
                    else quality = GachaQuality.Blue;
                }

                // 根据品质从对应列表随机选择物品
                List<GachaReward> pool = quality switch
                {
                    GachaQuality.RealGold => config.rewardsRealGold,
                    GachaQuality.Gold => config.rewardsGold,
                    GachaQuality.Purple => config.rewardsPurple,
                    _ => config.rewardsBlue
                };

                if (pool == null || pool.Count == 0)
                {
                    // 如果对应列表为空，降级为蓝色列表
                    pool = config.rewardsBlue;
                }

                // 随机选择一个
                var reward = pool[Main.rand.Next(pool.Count)];
                reward.ParseItemType();
                int itemType = reward.itemType;
                int stack = reward.count;

                // ===== 调试日志 =====
                Main.NewText($"[Gacha] 选中物品: {reward.item}, itemType={itemType}, count={stack}, quality={quality}", Color.Yellow);

                // 添加到结果列表
                results.Add(new GachaResult(itemType, stack, quality));

                // 发给玩家
                if (itemType > 0)
                    Item.NewItem(player.GetSource_Misc("GachaTest"), player.position, itemType, stack);
                else
                    Main.NewText($"[Gacha] 生成失败: {reward.item} 的 itemType=0", Color.Red);

                // 更新抽卡统计
                data.totalPullCount++;
                if (quality == GachaQuality.Purple)
                    data.pullsSinceLastPurple = 0;
                else
                    data.pullsSinceLastPurple++;

                if (quality == GachaQuality.Gold || quality == GachaQuality.RealGold)
                    data.pullsSinceLastGold = 0;
                else
                    data.pullsSinceLastGold++;
            }

            return results;
        }
    }
}
