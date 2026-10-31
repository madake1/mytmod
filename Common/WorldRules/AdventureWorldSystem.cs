using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace madake1.Common.WorldRules;

public sealed class AdventureWorldSystem : ModSystem
{
    public const string AdventureMapId = "madake-adventure";
    public const int CurrentMapVersion = 1;

    public static string MapId { get; private set; } = string.Empty;
    public static int MapVersion { get; private set; }
    public static bool IsAdventureWorld => MapId == AdventureMapId && MapVersion >= 1;

    public override void ClearWorld()
    {
        MapId = string.Empty;
        MapVersion = 0;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (!IsAdventureWorld)
            return;

        tag["mapId"] = MapId;
        tag["mapVersion"] = MapVersion;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        MapId = tag.GetString("mapId");
        MapVersion = tag.GetInt("mapVersion");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(MapId);
        writer.Write(MapVersion);
    }

    public override void NetReceive(BinaryReader reader)
    {
        MapId = reader.ReadString();
        MapVersion = reader.ReadInt32();
    }

    public static void MarkCurrentWorld()
    {
        MapId = AdventureMapId;
        MapVersion = CurrentMapVersion;
    }
}
