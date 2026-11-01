using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace madake1.Common.WorldRules;

public sealed class AdventureWorldSystem : ModSystem
{
    public const string AdventureMapId = "madake-adventure";
    public const int CurrentMapVersion = 1;
    // Stored in the vanilla .wld header, so the distributed map needs no .twld marker.
    private static readonly Guid BundledWorldId = new("4f238fb5-44c5-428c-932d-d394743e1a08");
    private const int BundledWorldNumericId = 1527058634;

    private static string savedMapId = string.Empty;
    private static int savedMapVersion;
    private static string instanceId = string.Empty;
    private static string networkScope = string.Empty;
    private static readonly HashSet<string> worldEvents = new();
    internal static bool LegacyPylonsRemoved { get; set; }

    internal static bool IsBundledWorld => Main.worldID == BundledWorldNumericId
        && Main.ActiveWorldFileData?.UniqueId == BundledWorldId;

    public static string MapId => IsBundledWorld ? AdventureMapId : savedMapId;
    public static int MapVersion => IsBundledWorld ? CurrentMapVersion : savedMapVersion;
    public static bool IsAdventureWorld => MapId == AdventureMapId && MapVersion >= 1;
    public static bool HasEvent(string id) => worldEvents.Contains(id);

    public static void SetEvent(string id)
    {
        if (!IsAdventureWorld || string.IsNullOrWhiteSpace(id) || Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient)
            return;
        if (worldEvents.Add(id) && Main.netMode == Terraria.ID.NetmodeID.Server)
            NetMessage.SendData(Terraria.ID.MessageID.WorldData);
    }
    // The path fingerprint separates copies even when both .wld and .twld were copied.
    public static string ProgressScope
    {
        get
        {
            if (!IsAdventureWorld)
                return string.Empty;
            if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient)
                return networkScope;
            string path = Main.ActiveWorldFileData?.Path ?? string.Empty;
            byte[] digest = SHA256.HashData(Encoding.UTF8.GetBytes(path.ToUpperInvariant()));
            return instanceId + ":" + Convert.ToHexString(digest, 0, 8);
        }
    }

    public override void ClearWorld()
    {
        savedMapId = string.Empty;
        savedMapVersion = 0;
        instanceId = string.Empty;
        networkScope = string.Empty;
        worldEvents.Clear();
        LegacyPylonsRemoved = false;
        AdventureBuildPermission.AllowWorldSetup = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (IsAdventureWorld)
        {
            tag["adventureInstanceId"] = instanceId;
            tag["adventureEvents"] = worldEvents.ToList();
            tag["legacyPylonsRemoved"] = LegacyPylonsRemoved;
        }
        // The bundled map is identified by its vanilla GUID. Keep .twld optional for it.
        if (!IsAdventureWorld || IsBundledWorld)
            return;

        tag["mapId"] = MapId;
        tag["mapVersion"] = MapVersion;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        savedMapId = tag.GetString("mapId");
        savedMapVersion = tag.GetInt("mapVersion");
        instanceId = tag.GetString("adventureInstanceId");
        LegacyPylonsRemoved = tag.GetBool("legacyPylonsRemoved");
        worldEvents.Clear();
        if (tag.ContainsKey("adventureEvents"))
            foreach (string id in tag.GetList<string>("adventureEvents"))
                worldEvents.Add(id);
    }

    public override void PostWorldLoad()
    {
        if (IsAdventureWorld && Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient && string.IsNullOrEmpty(instanceId))
            instanceId = Guid.NewGuid().ToString("N");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(MapId);
        writer.Write(MapVersion);
        writer.Write(ProgressScope);
        writer.Write((ushort)worldEvents.Count);
        foreach (string id in worldEvents)
            writer.Write(id);
    }

    public override void NetReceive(BinaryReader reader)
    {
        savedMapId = reader.ReadString();
        savedMapVersion = reader.ReadInt32();
        networkScope = reader.ReadString();
        worldEvents.Clear();
        int count = reader.ReadUInt16();
        for (int i = 0; i < count; i++)
            worldEvents.Add(reader.ReadString());
    }

    public static void MarkCurrentWorld()
    {
        savedMapId = AdventureMapId;
        savedMapVersion = CurrentMapVersion;
    }
}
