using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.WorldRules;

internal static class AdventureBuildPermission
{
    // Authoring is intentionally single-player only. Dedicated servers keep the map locked.
    public static bool CanEdit => Main.netMode == NetmodeID.SinglePlayer
        && Main.LocalPlayer?.active == true
        && Main.LocalPlayer.GetModPlayer<AuthorBadgePlayer>().HasAuthorBadge;

    public static bool IsLocked => AdventureWorldSystem.IsAdventureWorld && !CanEdit;
}

public sealed class AdventureWorldTile : GlobalTile
{
    public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        => !AdventureBuildPermission.IsLocked && base.CanKillTile(i, j, type, ref blockDamaged);

    public override bool CanPlace(int i, int j, int type)
        => !AdventureBuildPermission.IsLocked && base.CanPlace(i, j, type);

    public override bool CanExplode(int i, int j, int type)
        => !AdventureBuildPermission.IsLocked && base.CanExplode(i, j, type);

    public override bool CanReplace(int i, int j, int type, int tileTypeBeingPlaced)
        => !AdventureBuildPermission.IsLocked && base.CanReplace(i, j, type, tileTypeBeingPlaced);
}

public sealed class AdventureWorldWall : GlobalWall
{
    public override bool CanPlace(int i, int j, int type)
        => !AdventureBuildPermission.IsLocked && base.CanPlace(i, j, type);

    public override bool CanExplode(int i, int j, int type)
        => !AdventureBuildPermission.IsLocked && base.CanExplode(i, j, type);

    public override void KillWall(int i, int j, int type, ref bool fail)
    {
        if (AdventureBuildPermission.IsLocked)
            fail = true;
        else
            base.KillWall(i, j, type, ref fail);
    }
}
