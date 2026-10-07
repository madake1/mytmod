using madake1.Common.Networking;
using madake1.Content.Items.Zhanlipin;
using Terraria;
using Terraria.ModLoader;

namespace madake1.Common.Gacha
{
    // Prototype workflow. Client-side multiplayer authority still needs redesign.
    internal static class GachaWorkflow
    {
        internal static bool TryPull(Player player, GachaMachineConfig machine, int count, out string error)
        {
            error = null;
            if (player == null || !player.active || machine == null || (count != 1 && count != 10))
            {
                error = "无法抽卡：玩家、卡池或抽卡次数无效。";
                return false;
            }

            int currencyType = ModContent.ItemType<JinQiu>();
            if (player.CountItem(currencyType) < count)
            {
                error = "金球不够了辣！";
                return false;
            }
            for (int i = 0; i < count; i++)
                player.ConsumeItem(currencyType);

            var results = GachaService.DoPull(player, machine, count);
            GachaRequest.Request(player, machine, results);
            return true;
        }
    }
}
