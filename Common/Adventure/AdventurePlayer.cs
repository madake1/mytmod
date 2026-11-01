using System.Collections.Generic;
using System.Linq;
using madake1.Common.WorldRules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace madake1.Common.Adventure;

public sealed class AdventurePlayer : ModPlayer
{
    private sealed class WorldProgress
    {
        public readonly HashSet<string> crystals = new();
        public readonly Dictionary<string, int> quests = new();
    }

    private readonly Dictionary<string, WorldProgress> worlds = new();

    private WorldProgress Current
    {
        get
        {
            string scope = AdventureWorldSystem.ProgressScope;
            if (string.IsNullOrEmpty(scope))
                return null;
            if (!worlds.TryGetValue(scope, out var progress))
                worlds[scope] = progress = new WorldProgress();
            return progress;
        }
    }

    public bool HasCrystal(string id) => Current?.crystals.Contains(id) == true;
    public bool UnlockCrystal(string id) => Current?.crystals.Add(id) == true;
    // 0 = unavailable, 1 = accepted, 2 = completed and reward claimed.
    public int QuestStage(string id) => Current?.quests.GetValueOrDefault(id) ?? 0;
    public void SetQuestStage(string id, int stage)
    {
        if (Current != null)
            Current.quests[id] = stage;
    }

    public override void SaveData(TagCompound tag)
    {
        tag["worldProgress"] = worlds.Select(pair => new TagCompound
        {
            ["scope"] = pair.Key,
            ["crystals"] = pair.Value.crystals.ToList(),
            ["quests"] = pair.Value.quests.Select(q => new TagCompound { ["id"] = q.Key, ["stage"] = q.Value }).ToList()
        }).ToList();
    }

    public override void LoadData(TagCompound tag)
    {
        worlds.Clear();
        if (!tag.ContainsKey("worldProgress"))
            return;
        foreach (var entry in tag.GetList<TagCompound>("worldProgress"))
        {
            string scope = entry.GetString("scope");
            if (string.IsNullOrWhiteSpace(scope))
                continue;
            var progress = new WorldProgress();
            foreach (string id in entry.GetList<string>("crystals"))
                progress.crystals.Add(id);
            foreach (var quest in entry.GetList<TagCompound>("quests"))
                progress.quests[quest.GetString("id")] = quest.GetInt("stage");
            worlds[scope] = progress;
        }
    }

    public override void OnEnterWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient && AdventureWorldSystem.IsAdventureWorld)
            AdventurePackets.SendPlayerState(Player);
    }

    internal IEnumerable<string> CurrentCrystals => Current?.crystals ?? Enumerable.Empty<string>();
    internal IEnumerable<KeyValuePair<string, int>> CurrentQuests => Current?.quests ?? Enumerable.Empty<KeyValuePair<string, int>>();

    internal void ReplaceCurrent(IEnumerable<string> crystals, IEnumerable<KeyValuePair<string, int>> quests)
    {
        var current = Current;
        if (current == null)
            return;
        current.crystals.Clear();
        current.quests.Clear();
        foreach (string id in crystals)
            if (CrystalWorldSystem.IsStationKey(id, out _)
                && (Main.netMode != NetmodeID.Server || CrystalWorldSystem.TryGetStation(id, out _)))
                current.crystals.Add(id);
        foreach (var quest in quests)
            if (!string.IsNullOrWhiteSpace(quest.Key) && quest.Key.Length <= 80
                && quest.Value >= 0 && quest.Value <= 2)
                current.quests[quest.Key] = quest.Value;
    }
}
