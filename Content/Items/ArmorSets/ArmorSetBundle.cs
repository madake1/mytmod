using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.Items.ArmorSets
{
    public abstract class ArmorSetBundle : ModItem
    {
        protected abstract int HeadItem { get; }
        protected abstract int BodyItem { get; }
        protected abstract int LegsItem { get; }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.OpenableBag[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(HeadItem));
            itemLoot.Add(ItemDropRule.Common(BodyItem));
            itemLoot.Add(ItemDropRule.Common(LegsItem));
        }
    }

    public class PlatinumArmorSetBundle : ArmorSetBundle
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.PlatinumHelmet}";
        protected override int HeadItem => ItemID.PlatinumHelmet;
        protected override int BodyItem => ItemID.PlatinumChainmail;
        protected override int LegsItem => ItemID.PlatinumGreaves;
    }

    public class GoldArmorSetBundle : ArmorSetBundle
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.GoldHelmet}";
        protected override int HeadItem => ItemID.GoldHelmet;
        protected override int BodyItem => ItemID.GoldChainmail;
        protected override int LegsItem => ItemID.GoldGreaves;
    }

    public class RoninArmorSetBundle : ArmorSetBundle
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.RoninHat}";
        protected override int HeadItem => ItemID.RoninHat;
        protected override int BodyItem => ItemID.RoninShirt;
        protected override int LegsItem => ItemID.RoninPants;
    }

    public class WolfArmorSetBundle : ArmorSetBundle
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.WolfMask}";
        protected override int HeadItem => ItemID.WolfMask;
        protected override int BodyItem => ItemID.WolfShirt;
        protected override int LegsItem => ItemID.WolfPants;
    }
}
