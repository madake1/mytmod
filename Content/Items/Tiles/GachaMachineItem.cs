using Terraria.ID;
using Terraria.ModLoader;
namespace madake1.Content.Items.Tiles {
    public class GachaMachineItem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 32; // 物品图标宽度
            Item.height = 32; // 物品图标高度
            Item.maxStack = 99; // 最大堆叠数量
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing; // 使用方式
            Item.consumable = true; // 是否消耗
            Item.createTile = ModContent.TileType<GachaMachineTile>(); // 对应的 Tile
            Item.value = 1000; // 物品价值
            Item.rare = ItemRarityID.Blue; // 稀有度
        }
    }
}
