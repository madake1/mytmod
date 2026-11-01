using System.Collections.Generic;
using System.IO;
using System.Linq;
using madake1.Common.Networking;
using madake1.Common.WorldRules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Adventure;

internal enum AdventureAction : byte { UnlockCrystal = 2, Teleport = 3 }

internal static class AdventurePackets
{
    private static ModPacket NewPacket(MyMessageType type)
    {
        var packet = ModContent.GetInstance<MDK>().GetPacket();
        packet.Write((byte)type);
        return packet;
    }

    public static void SendPlayerState(Player player, int toWho = -1)
    {
        var state = player.GetModPlayer<AdventurePlayer>();
        var packet = NewPacket(MyMessageType.AdventureState);
        packet.Write(AdventureWorldSystem.ProgressScope);
        var crystals = state.CurrentCrystals.ToList();
        packet.Write((ushort)crystals.Count);
        foreach (string id in crystals)
            packet.Write(id);
        var quests = state.CurrentQuests.ToList();
        packet.Write((ushort)quests.Count);
        foreach (var quest in quests)
        {
            packet.Write(quest.Key);
            packet.Write((byte)quest.Value);
        }
        packet.Send(toWho);
    }

    public static void Request(AdventureAction action, string id)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
        {
            string message = AdventureService.Execute(Main.LocalPlayer, action, id);
            Main.NewText(message);
            return;
        }
        var packet = NewPacket(MyMessageType.AdventureAction);
        packet.Write((byte)action);
        packet.Write(id);
        packet.Send();
    }

    public static void Handle(MyMessageType type, BinaryReader reader, int whoAmI)
    {
        if (type == MyMessageType.AdventureState)
        {
            string scope = reader.ReadString();
            int crystalCount = reader.ReadUInt16();
            if (crystalCount > 256) return;
            var crystals = new List<string>();
            for (int i = 0; i < crystalCount; i++) crystals.Add(reader.ReadString());
            int questCount = reader.ReadUInt16();
            if (questCount > 256) return;
            var quests = new List<KeyValuePair<string, int>>();
            for (int i = 0; i < questCount; i++) quests.Add(new(reader.ReadString(), reader.ReadByte()));
            if (scope != AdventureWorldSystem.ProgressScope || string.IsNullOrEmpty(scope)) return;
            Player player = Main.netMode == NetmodeID.Server ? Main.player[whoAmI] : Main.LocalPlayer;
            player.GetModPlayer<AdventurePlayer>().ReplaceCurrent(crystals, quests);
            return;
        }
        if (type == MyMessageType.AdventureAction && Main.netMode == NetmodeID.Server)
        {
            var action = (AdventureAction)reader.ReadByte();
            string id = reader.ReadString();
            if (id.Length > 80 || whoAmI < 0 || whoAmI >= Main.maxPlayers || !Main.player[whoAmI].active) return;
            Player player = Main.player[whoAmI];
            string message = AdventureService.Execute(player, action, id);
            SendPlayerState(player, whoAmI);
            var notice = NewPacket(MyMessageType.AdventureNotice);
            notice.Write(message);
            notice.Send(whoAmI);
            return;
        }
        if (type == MyMessageType.AdventureNotice && Main.netMode == NetmodeID.MultiplayerClient)
            Main.NewText(reader.ReadString());
    }
}

internal static class AdventureService
{
    public static string Execute(Player player, AdventureAction action, string id)
    {
        if (!AdventureWorldSystem.IsAdventureWorld || string.IsNullOrEmpty(AdventureWorldSystem.ProgressScope))
            return "当前不是冒险地图。";
        var state = player.GetModPlayer<AdventurePlayer>();
        if (action == AdventureAction.UnlockCrystal)
        {
            if (!CrystalWorldSystem.TryGetStation(id, out var station)
                || !CrystalWorldSystem.IsPlayerNear(player, station))
                return "需要靠近这座传送水晶。";
            return state.UnlockCrystal(id) ? "已解锁这座传送水晶。" : "这座传送水晶已经解锁。";
        }
        if (action == AdventureAction.Teleport)
        {
            if (!state.HasCrystal(id) || !CrystalWorldSystem.TryGetStation(id, out var station))
                return "目标传送水晶尚未解锁或已被移走。";
            if (!CrystalWorldSystem.HasUnlockedStationNearby(player, state))
                return "请先靠近已解锁的传送水晶。";
            if (!CrystalWorldSystem.TryFindLanding(player, station, out Vector2 position))
                return "目标水晶旁没有安全落点。";
            player.Teleport(position, 1);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, player.whoAmI, position.X, position.Y);
            return "已传送到目标水晶。";
        }
        return "未知操作。";
    }
}
