using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace madake1.Content.Items.Weapons.Melee
{
    [LegacyName("Changli")]
    public class test2 : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(Terraria.ID.ItemID.Muramasa);

            Item.damage *= 2;
            Item.shootSpeed *= 0.8f;
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(Terraria.ID.ItemID.StoneBlock, 10);
            recipe.AddTile(Terraria.ID.TileID.WorkBenches);
            recipe.Register();
        }

    }
}
