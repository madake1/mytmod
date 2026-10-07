using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace madake1.Common.UI
{
    public class GachaUILockPlayer : ModPlayer { }

    public class GachaUILockTile : GlobalTile
    {
        // 判断UI是否打开
        private bool IsGachaUILocked => ModContent.GetInstance<madake1.Content.UI.GachaUISystem>()
                                                .GachaUIInterface?.CurrentState != null;

        // 阻止破坏方块（家具也在这里）
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (IsGachaUILocked)
                return false;
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }

        // 阻止放置方块
        public override bool CanPlace(int i, int j, int type)
        {
            if (IsGachaUILocked)
                return false;
            return base.CanPlace(i, j, type);
        }

        // 阻止炸药破坏
        public override bool CanExplode(int i, int j, int type)
        {
            if (IsGachaUILocked)
                return false;
            return base.CanExplode(i, j, type);
        }
    }

    public class GachaUILockWall : GlobalWall
    {
        private bool IsGachaUILocked => ModContent.GetInstance<madake1.Content.UI.GachaUISystem>()
                                                .GachaUIInterface?.CurrentState != null;

        public override void KillWall(int i, int j, int type, ref bool fail)
        {
            if (IsGachaUILocked)
                fail = true; // 阻止破坏墙
        }
    }

    public class GachaUILockItem : GlobalItem
    {
        private bool IsGachaUILocked => ModContent.GetInstance<madake1.Content.UI.GachaUISystem>()
                                                .GachaUIInterface?.CurrentState != null;

        // 阻止使用物品和攻击
        public override bool CanUseItem(Item item, Player player)
        {
            if (IsGachaUILocked)
                return false;
            return base.CanUseItem(item, player);
        }
    }
}
