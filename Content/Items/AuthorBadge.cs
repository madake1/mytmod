using madake1.Common.WorldRules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.Items;

public sealed class AuthorBadge : ModItem
{
    // Temporary vanilla icon; the item has no recipe, drop, shop entry, or starter grant.
    public override string Texture => $"Terraria/Images/Item_{ItemID.Toolbelt}";

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.maxStack = 1;
        Item.rare = ItemRarityID.Purple;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AuthorBadgePlayer>().HasAuthorBadge = true;
    }
}
