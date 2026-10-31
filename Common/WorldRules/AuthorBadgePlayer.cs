using Terraria.ModLoader;

namespace madake1.Common.WorldRules;

public sealed class AuthorBadgePlayer : ModPlayer
{
    public bool HasAuthorBadge { get; set; }

    public override void ResetEffects()
    {
        HasAuthorBadge = false;
    }
}
