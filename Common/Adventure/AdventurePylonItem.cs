using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Adventure;

public sealed class AdventurePylonItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<AdventurePylonTile>());
        Item.maxStack = 99;
        Item.rare = ItemRarityID.Blue;
    }
}
