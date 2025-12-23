using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace madake1.Content.Items.Weapons
{
    public class Jinxi : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(Terraria.ID.ItemID.Muramasa);
            
            Item.damage *= 2;
            Item.shootSpeed *= 0.8f;
        }

        // 物品合成表的设置部分
    public override void AddRecipes()
    {
        // 1.4的合成表具体写法可以查看下列网址来详细看看，这里只是一个简单的示例
        // 这个网址是TML的教程，英文
        //https://github.com/tModLoader/tModLoader/wiki/Basic-Recipes
        // 这个网址是汉化exmod，中文
        //https://github.com/Cyrillya/Example-Mod-Zh-Project/blob/master/Content/ExampleRecipes.cs
        // 生成1个这种物品
        var recipe = CreateRecipe();
        // 这样可以生成50个
        // CreateRecipe(50);
        // 合成材料，需要10个泥土块
        // 哦对了，如果你写俩种相同的材料的话
        // 它们会分别显示而不是合并成一个
        // （我家门前有两颗树，一颗是枣树，另一颗还是枣树）
        recipe.AddIngredient(Terraria.ID.ItemID.DirtBlock, 10);
        // 以及在工作台旁边
        recipe.AddTile(Terraria.ID.TileID.WorkBenches);
        // 把这个合成表装进tr的系统里
        recipe.Register();
    }
        
    }
}