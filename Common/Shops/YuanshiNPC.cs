using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using madake1.Content.Items.Zhanlipin;

namespace madake1.Common.Shops
{
    public class YuanshiNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Merchant)
            {
                shop.Add<YuanShi>();
                shop.Add<JinQiu>();
            }
        }
    }
}
