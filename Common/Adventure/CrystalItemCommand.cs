using madake1.Common.WorldRules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Adventure;

public sealed class CrystalItemCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "crystalitem";
    public override string Usage => "/crystalitem";
    public override string Description => "作者领取一座可手动放置的传送水晶";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (Main.netMode != NetmodeID.SinglePlayer
            || caller.Player?.GetModPlayer<AuthorBadgePlayer>().HasAuthorBadge != true)
        {
            caller.Reply("请在单人模式装备作者身份证后领取。");
            return;
        }
        caller.Player.QuickSpawnItem(caller.Player.GetSource_Misc("CrystalAuthorItem"),
            ModContent.ItemType<AdventurePylonItem>());
        caller.Reply("已领取传送水晶道具，放在有三格承重地面的地方。右键放好的水晶即可解锁。");
    }
}
