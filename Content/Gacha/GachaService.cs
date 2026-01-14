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

                // ========================= 1. 判断品质 =========================
                var quality = DetermineQuality(gachaPlayer, data, config, selectedRealGoldIndex);

                // ========================= 2. 判断是否大保底 =========================
                bool hasUp = selectedRealGoldIndex.HasValue;
                bool upGuaranteeActive = gachaPlayer.IsUpGuaranteeActive(machineId);
                bool forceUp = quality == GachaQuality.RealGold && hasUp && upGuaranteeActive;

                // ========================= 3. 生成池子 =========================
                var pool = GeneratePool(config, quality, selectedRealGoldIndex, forceUp);

                // 防护：空池子降级
                if (pool == null || pool.Count == 0)
                {
                    Main.NewText("[GachaDebug] 抽奖池为空，降级为蓝色奖励池");
                    pool = config.rewardsBlue.Count > 0 ? config.rewardsBlue : new List<GachaReward>();
                }

                // ========================= 4. 抽物品 =========================
                GachaReward reward = null;

                // 安全检查 selectedRealGoldIndex
                bool validIndex = selectedRealGoldIndex.HasValue &&
                                  selectedRealGoldIndex.Value >= 0 &&
                                  selectedRealGoldIndex.Value < config.rewardsRealGold.Count;

                if (forceUp && validIndex)
                {
                    // 真大保底：必出选中的奖励
                    reward = config.rewardsRealGold[selectedRealGoldIndex.Value];
                    Main.NewText("[DoPull] 真大保底：必出选中的大奖");
                    gachaPlayer.SetUpGuarantee(machineId, false); // 消耗大保底
                }
                else if (forceUp && !validIndex)
                {
                    // 防护：索引无效 → 随机 RealGold
                    Main.NewText("[DoPull] 真大保底索引无效，随机抽 RealGold");
                    pool = config.rewardsRealGold.Count > 0 ? config.rewardsRealGold : config.rewardsBlue;
                    reward = pool[Main.rand.Next(pool.Count)];
                    gachaPlayer.SetUpGuarantee(machineId, false);
                }
                else if (quality == GachaQuality.RealGold && hasUp)
                {
                    // 非大保底 RealGold
                    if (config.rewardsRealGold.Count > 0)
                    {
                        reward = pool[Main.rand.Next(pool.Count)];

                        GachaReward selectedReward = validIndex ? config.rewardsRealGold[selectedRealGoldIndex.Value] : null;

                        if (selectedReward != null && reward.itemType != selectedReward.itemType)
                        {
                            gachaPlayer.SetUpGuarantee(machineId, true);
                            Main.NewText("[GachaDebug] 非大保底 RealGold 歪了，激活大保底");
                        }
                        else if (selectedReward != null)
                        {
                            Main.NewText("[GachaDebug] 非大保底 RealGold 抽中选中的大奖");
                        }
                    }
                    else
                    {
                        // 防护：RealGold 池为空 → 随机蓝色奖励
                        pool = config.rewardsBlue.Count > 0 ? config.rewardsBlue : new List<GachaReward>();
                        reward = pool[Main.rand.Next(pool.Count)];
                    }
                }
                else
                {
                    // 其他情况：随机抽池子
                    if (pool.Count > 0)
                        reward = pool[Main.rand.Next(pool.Count)];
                    else
                        reward = config.rewardsBlue.Count > 0 ? config.rewardsBlue[Main.rand.Next(config.rewardsBlue.Count)] : null;
                }

                // ========================= 5. 创建物品 =========================
                if (reward != null)
                {
                    reward.ParseItemType();
                    int itemType = reward.itemType;
                    int stack = reward.count;

                    results.Add(new GachaResult(itemType, stack, quality));
                    Item.NewItem(player.GetSource_Misc("GachaTest"), player.position, itemType, stack);
                }

                // ========================= 6. 更新计数 =========================
                UpdateCounters(data, quality);
            }

            return results;
        }

        // ===================== 决定这一抽的品质 =====================
        private static GachaQuality DetermineQuality(GachaPlayer player, GachaMachineData data, GachaMachineConfig config, int? selectedRealGoldIndex)
        {
            if (data.pullsSinceLastGold >= 79)
            {
                Main.NewText("[DetermineQuality] 金色保底触发");
                return DetermineGoldQuality(player, data, config, selectedRealGoldIndex);
            }

            if (data.pullsSinceLastPurple >= 9)
                return GachaQuality.Purple;

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
                Main.NewText("[DetermineQuality] 随机抽到金色");
                return DetermineGoldQuality(player, data, config, selectedRealGoldIndex);
            }
            else if (rollF < purpleChance)
            {
                return GachaQuality.Purple;
            }
            else
            {
                return GachaQuality.Blue;
            }
        }

        // ===================== 金色保底品质判定 =====================
        private static GachaQuality DetermineGoldQuality(GachaPlayer player, GachaMachineData data, GachaMachineConfig config, int? selectedRealGoldIndex)
        {
            string machineId = config.GetMachineId();

            bool validIndex = selectedRealGoldIndex.HasValue &&
                              selectedRealGoldIndex.Value >= 0 &&
                              selectedRealGoldIndex.Value < config.rewardsRealGold.Count;

            if (player.IsUpGuaranteeActive(machineId) && validIndex)
            {
                Main.NewText("[DetermineGoldQuality] 真保底触发，必出选中的 RealGold");
                return GachaQuality.RealGold;
            }
            else if (player.IsUpGuaranteeActive(machineId) && !validIndex)
            {
                Main.NewText("[DetermineGoldQuality] 真保底索引无效，降级为 Gold");
                player.SetUpGuarantee(machineId, true);
                return GachaQuality.Gold;
            }

            float roll = Main.rand.NextFloat();
            if (selectedRealGoldIndex.HasValue && validIndex)
            {
                if (roll < 0.4f)
                {
                    Main.NewText("[DetermineGoldQuality] 非大保底出普通 Gold → 激活大保底");
                    player.SetUpGuarantee(machineId, true);
                    return GachaQuality.Gold;
                }
                else
                {
                    return GachaQuality.RealGold;
                }
            }
            else
            {
                if (roll < 0.5f)
                {
                    Main.NewText("[DetermineGoldQuality] 没有选中 → Gold");
                    return GachaQuality.Gold;
                }
                else
                {
                    Main.NewText("[DetermineGoldQuality] 没有选中 → RealGold");
                    return GachaQuality.RealGold;
                }
            }
        }

        private static List<GachaReward> GeneratePool(GachaMachineConfig config, GachaQuality quality, int? selectedRealGoldIndex, bool forceUp)
        {
            List<GachaReward> pool;
            bool validIndex = selectedRealGoldIndex.HasValue &&
                              selectedRealGoldIndex.Value >= 0 &&
                              selectedRealGoldIndex.Value < config.rewardsRealGold.Count;

            switch (quality)
            {
                case GachaQuality.RealGold:
                    if (forceUp && validIndex)
                    {
                        Main.NewText("[GeneratePool] 真保底池：必出选中的大奖");
                        pool = new List<GachaReward> { config.rewardsRealGold[selectedRealGoldIndex.Value] };
                    }
                    else
                    {
                        Main.NewText("[GeneratePool] 普通 RealGold 池");
                        pool = config.rewardsRealGold.Count > 0 ? config.rewardsRealGold : config.rewardsBlue;
                    }
                    break;
                case GachaQuality.Gold:
                    pool = config.rewardsGold.Count > 0 ? config.rewardsGold : config.rewardsBlue;
                    break;
                case GachaQuality.Purple:
                    pool = config.rewardsPurple.Count > 0 ? config.rewardsPurple : config.rewardsBlue;
                    break;
                default:
                    pool = config.rewardsBlue;
                    break;
            }
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
