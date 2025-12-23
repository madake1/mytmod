using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.Items.Zhanlipin
{
    public class JinQiu : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 32;           // 贴图宽度，根据实际图片修改
            Item.height = 32;          // 贴图高度
            Item.maxStack = 999;       // 最大叠加
            Item.value = Item.buyPrice(silver: 10); // 出售价格
            Item.rare = ItemRarityID.Green; // 稀有度
        }

        public override void AddRecipes()
        {
            // 160 个原石可以在工作台合成 Jinqiu（假设 Jinqiu 已经定义）
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<YuanShi>(), 160);
            recipe.AddTile(TileID.WorkBenches); // 工作台
            recipe.Register();
        }
    }
}
