using madake1.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.WorldRules;

public sealed class AdventureMapCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat | CommandType.Console;
    public override string Command => "madakemap";
    public override string Usage => "/madakemap status|mark";
    public override string Description => "查看或标记 madake 冒险地图";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length == 1 && args[0] == "status")
        {
            caller.Reply(AdventureWorldSystem.IsAdventureWorld
                ? $"冒险地图：{AdventureWorldSystem.MapId} v{AdventureWorldSystem.MapVersion}"
                : "当前是普通世界，没有冒险地图标记。");
            return;
        }

        if (args.Length != 1 || args[0] != "mark")
        {
            caller.Reply(Usage);
            return;
        }

        bool serverConsole = Main.netMode == NetmodeID.Server && caller.CommandType == CommandType.Console;
        if (Main.netMode != NetmodeID.SinglePlayer && !serverConsole)
        {
            caller.Reply("只能在单人模式或服务器控制台给地图添加冒险标记。");
            return;
        }
    
        if (Main.maxTilesX < 8400)
        {
            caller.Reply("请先创建大世界，再添加冒险标记。");
            return;
        }

        if (!serverConsole && caller.Player?.GetModPlayer<AuthorBadgePlayer>().HasAuthorBadge != true)
        {
            caller.Reply("请先在功能饰品栏装备作者身份证。");
            return;
        }

        AdventureWorldSystem.MarkCurrentWorld();
        if (serverConsole)
            NetMessage.SendData(MessageID.WorldData);
        caller.Reply($"已标记为 {AdventureWorldSystem.AdventureMapId} v{AdventureWorldSystem.CurrentMapVersion}。保存并退出世界后，保留 .wld 和 .twld 两个文件。");
    }
}
