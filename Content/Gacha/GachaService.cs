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
            int? selectedRealGoldIndex = gachaPlayer.GetSelectedReward(machineId);
            for (int i = 0; i < times; i++)
            {
                var data = gachaPlayer.GetData(machineId);

                // 1. 判断品质
                var quality = DetermineQuality(data, config, selectedRealGoldIndex);

                // 2. 生成池子
                var pool = GeneratePool(data, config, selectedRealGoldIndex, quality);

                if (pool == null || pool.Count == 0)
                {
                    Main.NewText("[GachaDebug] 抽奖池为空，降级为蓝色奖励池");
                    pool = config.rewardsBlue;
                }

                // 3. 抽物品
                GachaReward reward;
                    // 这里是判断是否为真保底且玩家选了大奖
                bool forceSpecialReward =
                    quality == GachaQuality.RealGold &&
                    selectedRealGoldIndex.HasValue;

                if (forceSpecialReward)
                {
                    reward = config.rewardsRealGold[selectedRealGoldIndex.Value];
                    Main.NewText("[GachaDebug] 强制抽中选中的大奖");
                }
                else
                {
                    reward = pool[Main.rand.Next(pool.Count)];
                }


                reward.ParseItemType();
                int itemType = reward.itemType;
                int stack = reward.count;

                results.Add(new GachaResult(itemType, stack, quality));
                Item.NewItem(player.GetSource_Misc("GachaTest"), player.position, itemType, stack);

                // 4. 更新计数
                UpdateCounters(data, quality);

                // 5. 日志代替动画
                Main.NewText($"[GachaDebug] 第{i + 1}抽: {itemType} x{stack}, 品质: {quality}, pullsSinceLastGold={data.pullsSinceLastGold}, pullsSinceLastPurple={data.pullsSinceLastPurple}");
            }

            return results;
        }

        // ===================== 决定这一抽的品质 =====================

        private static GachaQuality DetermineQuality(GachaMachineData data, GachaMachineConfig config, int? selectedRealGoldIndex)
        {
            if (data.pullsSinceLastGold >= 79)
            {
                Main.NewText("[GachaDebug] 金色保底触发");
                return DetermineGoldQuality(data, config, selectedRealGoldIndex);
            }

            if (data.pullsSinceLastPurple >= 9)
            {
                Main.NewText("[GachaDebug] 小保底触发");
                return GachaQuality.Purple;
            }

            // 概率递增
            float purpleChance = 0.10f;
            if (data.pullsSinceLastPurple >= 7)
                purpleChance += 0.5f * (data.pullsSinceLastPurple - 7);

            float goldChance = 0.01f;
            if (data.pullsSinceLastGold >= 50)
                goldChance += 0.02f * (data.pullsSinceLastGold - 50 + 1);

            float rollF = Main.rand.NextFloat();

            if (rollF < goldChance)
            {
                Main.NewText($"[GachaDebug] 随机抽到金色,goldChance={goldChance:F4}");
                return DetermineGoldQuality(data, config, selectedRealGoldIndex);
            }
            else if (rollF < purpleChance)
            {
                Main.NewText($"[GachaDebug] 随机抽到紫色, purpleChance={purpleChance:F4}");
                return GachaQuality.Purple;
            }
            else
            {
                return GachaQuality.Blue;
            }
        }

        // ===================== 金色保底品质判定 =====================
        private static GachaQuality DetermineGoldQuality(GachaMachineData data, GachaMachineConfig config, int? selectedRealGoldIndex)
        {
            throw new NotImplementedException();
        }

        private static List<GachaReward> GeneratePool(GachaMachineData data, GachaMachineConfig config, int? selectedRealGoldIndex, GachaQuality quality)
        {
            List<GachaReward> pool = null;

            // 大保底逻辑
            bool triggerGoldGuarantee = data.pullsSinceLastGold >= 79;
            if (triggerGoldGuarantee)
            {
                Main.NewText("[GachaDebug] 生成大保底池子");

                if (selectedRealGoldIndex.HasValue && config.rewardsRealGold.Count > selectedRealGoldIndex.Value)
                {
                    var selectedReward = config.rewardsRealGold[selectedRealGoldIndex.Value];

                    if (data.upGuaranteeActive)
                    {
                        Main.NewText("[GachaDebug] 真保底必出选中大奖");
                        pool = new List<GachaReward> { selectedReward };
                        data.upGuaranteeActive = false;
                    }
                    else
                    {
                        Main.NewText("[GachaDebug] 普通大保底: 选中的大奖 + rewardsGold混合池");
                        pool = new List<GachaReward> { selectedReward };
                        pool.AddRange(config.rewardsGold);

                        // 测试抽中选中大奖的概率
                        var chosenIndex = Main.rand.Next(pool.Count);
                        var chosen = pool[chosenIndex];
                        Main.NewText($"[GachaDebug] 本次大保底随机选中: {chosen.item}");
                        if (chosen.itemType != selectedReward.itemType)
                            data.upGuaranteeActive = true; // 本次没抽中 → 下一次必出
                    }
                }
                else
                {
                    Main.NewText("[GachaDebug] 玩家未选大奖: rewardsRealGold + rewardsGold混合池");
                    pool = new List<GachaReward>();
                    pool.AddRange(config.rewardsGold);
                    pool.AddRange(config.rewardsRealGold);
                }
            }
            else if (quality == GachaQuality.Purple)
            {
                pool = config.rewardsPurple;
            }
            else if (quality == GachaQuality.Gold)
            {
                pool = config.rewardsGold;
            }
            else
            {
                pool = config.rewardsBlue;
            }

            Main.NewText($"[GachaDebug] 生成池子大小: {pool?.Count ?? 0}");
            return pool;
        }

        private static void UpdateCounters(GachaMachineData data, GachaQuality quality)
        {
            data.pullsSinceLastPurple = (quality == GachaQuality.Purple) ? 0 : data.pullsSinceLastPurple + 1;
            data.pullsSinceLastGold = (quality == GachaQuality.Gold || quality == GachaQuality.RealGold) ? 0 : data.pullsSinceLastGold + 1;
            data.totalPullCount++;
        }
    }
}
