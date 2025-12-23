using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.Items.Zhanlipin
{
    public class YuanShi : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;           // 贴图宽度，根据实际图片修改
            Item.height = 32;          // 贴图高度
            Item.maxStack = 999;       // 最大叠加数量
            Item.value = Item.buyPrice(silver: 1); // 出售价格
            Item.rare = ItemRarityID.Blue; // 稀有度
        }


    }
}
