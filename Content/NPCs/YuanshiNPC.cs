using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using madake1.Content.Items.Zhanlipin; // 引入你的道具命名空间

namespace madake1.Content.NPCs
{
    public class YuanshiNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            // 商人出售 YuanShi
            if (shop.NpcType == NPCID.Merchant)
            {
                // shop.Add<YuanShi>(); 
                shop.Add<JinQiu>(); 
            }
        }
    }
}
