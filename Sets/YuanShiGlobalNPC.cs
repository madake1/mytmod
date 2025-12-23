using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Sets
{
    public class YuanShiGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // 给小怪掉落 1~5 个原石
            if (!npc.friendly && npc.lifeMax > 5 && npc.boss == false) // 小怪条件
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Zhanlipin.YuanShi>(), 1, 1, 5));
            }

            // 给Boss掉落 100~300 个原石
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Zhanlipin.YuanShi>(), 1, 100, 300));
            }
        }
    }
}
