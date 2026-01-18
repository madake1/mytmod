using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.Items.Weapons.Ranged
{
    public class TwoCrownBow : ModItem
    {
        public override void SetStaticDefaults() 
        {
            // 创意模式献祭数量
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() 
        {
            Item.damage = 110; // 武器伤害
            Item.DamageType = DamageClass.Ranged; // 远程伤害
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 20; // 攻击速度
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot; // 射击动作
            Item.noMelee = true; // 物品自身不造成伤害
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item5; // 射箭声音
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true; // 自动连续使用
            Item.shoot = ProjectileID.WoodenArrowFriendly; // 默认发射箭矢
            Item.shootSpeed = 16f; // 射速
            Item.value = Item.buyPrice(gold: 1);
        }

        // 可选功能：右键使用（这里保持普通使用，不加特殊效果）
        public override bool AltFunctionUse(Player player)
        {
            return false;
        }

        // 射击逻辑：直接发射箭矢
        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 可以修改箭矢类型或添加额外效果，这里直接使用默认箭
            return true; // 返回true表示使用默认发射行为
        }

        public override Vector2? HoldoutOffset()
        {
            // 控制持弓时的偏移
            return new Vector2(-9, 4);
        }

        // 配方示例：只使用泰拉瑞亚自带材料
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 12)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddIngredient(ItemID.SoulofSight, 4)
                .AddIngredient(ItemID.FragmentVortex, 4)
                .AddTile(TileID.MythrilAnvil) // 使用合适工作台
                .Register();
        }
    }
}
