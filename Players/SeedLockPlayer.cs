using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace madake1.Players
{
    public class SeedLockPlayer : ModPlayer { }

    public class SeedLockTile : GlobalTile
    {
        // private bool IsLockedWorld => true;
        private bool IsLockedWorld => Main.ActiveWorldFileData.Seed == 897520328;


        // 禁止破坏方块（家具也在这里）
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (IsLockedWorld)
            {
                // 阻止破坏方块
                return false;
            }
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        // 禁止放置方块
        public override bool CanPlace(int i, int j, int type)
        {
            if (IsLockedWorld)
            {
                return false;
            }
            return base.CanPlace(i, j, type);
        }

        // 禁止炸药破坏
        public override bool CanExplode(int i, int j, int type)
        {
            if (IsLockedWorld)
            {
                return false;
            }
            return base.CanExplode(i, j, type);
        }
    }

    public class SeedLockPlayerUseItem : ModPlayer
    {
        private bool IsLockedWorld => true;

        public override bool CanUseItem(Item item)
        {
            if (IsLockedWorld)
                return true; // 允许使用
            return base.CanUseItem(item);
        }
    }
}
