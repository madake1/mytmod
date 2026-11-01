using Terraria.ModLoader;

namespace madake1.Common.Adventure;

// Only used as a cursor icon; it shares the blue crystal texture used by the map.
public sealed class AdventurePylonUnlockedHoverItem : ModItem
{
    public override string Texture => "madake1/Common/Adventure/AdventurePylonTile_MapIcon";

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 38;
    }
}
