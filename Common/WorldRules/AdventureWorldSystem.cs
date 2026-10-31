using System;
using System.IO;
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

    internal static bool IsBundledWorld => Main.worldID == BundledWorldNumericId
        && Main.ActiveWorldFileData?.UniqueId == BundledWorldId;

    public static string MapId => IsBundledWorld ? AdventureMapId : savedMapId;
    public static int MapVersion => IsBundledWorld ? CurrentMapVersion : savedMapVersion;
    public static bool IsAdventureWorld => MapId == AdventureMapId && MapVersion >= 1;

    public override void ClearWorld()
    {
        savedMapId = string.Empty;
        savedMapVersion = 0;
        AdventureBuildPermission.AllowWorldSetup = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
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
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(MapId);
        writer.Write(MapVersion);
    }

    public override void NetReceive(BinaryReader reader)
    {
        savedMapId = reader.ReadString();
        savedMapVersion = reader.ReadInt32();
    }

    public static void MarkCurrentWorld()
    {
        savedMapId = AdventureMapId;
        savedMapVersion = CurrentMapVersion;
    }
}
