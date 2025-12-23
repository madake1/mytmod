using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.DataStructures;
using madake1.Content.UI;
using Terraria.Enums;
using Terraria.ModLoader.IO; // 添加必要的命名空间

namespace madake1.Content.Items.Tiles
{
    public class GachaMachineTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 4; // 宽度为 4 格（64 像素）
            TileObjectData.newTile.Height = 5; // 高度为 5 格（80 像素）
            TileObjectData.newTile.Origin = new Point16(2, 4); // 设置原点位置
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[5] { 16, 16, 16, 16, 16 }; // 每一行的高度
            TileObjectData.addTile(Type);

            var name = CreateMapEntryName();
        
            // 设置自定义图片资源
            AddMapEntry(new Color(200, 200, 200), name);

            // AdjTiles 可以先注释掉
            // AdjTiles = new int[] { TileID.Dresser };
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.myPlayer == Main.LocalPlayer.whoAmI)
            {
                ModContent.GetInstance<GachaUISystem>().ToggleUI(i,j);
            }
            return true;
        }
    }

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
