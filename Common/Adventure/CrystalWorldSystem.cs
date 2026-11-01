using System;
using System.Linq;
using madake1.Common.WorldRules;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Adventure;

public sealed class CrystalWorldSystem : ModSystem
{
    private const int InteractionRadius = 8;

    public static string StationKey(Point16 topLeft) => $"{topLeft.X}:{topLeft.Y}";

    public static bool IsStationKey(string key, out Point16 topLeft)
    {
        topLeft = default;
        string[] parts = key?.Split(':');
        if (parts?.Length != 2 || !short.TryParse(parts[0], out short x)
            || !short.TryParse(parts[1], out short y)
            || x < 1 || y < 1 || x >= Main.maxTilesX - 3 || y >= Main.maxTilesY - 4)
            return false;
        topLeft = new Point16(x, y);
        return true;
    }

    public static bool TryGetStation(string key, out Point16 topLeft)
    {
        if (!IsStationKey(key, out topLeft)) return false;
        if (!TileEntity.ByPosition.TryGetValue(topLeft, out TileEntity entity)
            || entity is not AdventurePylonEntity pylon)
            return false;
        return pylon.IsTileValidForEntity(topLeft.X, topLeft.Y);
    }

    public static bool TryGetStationAtTile(int i, int j, out Point16 topLeft)
    {
        topLeft = default;
        if (i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY)
            return false;
        Tile tile = Main.tile[i, j];
        if (!tile.HasTile || tile.TileType != ModContent.TileType<AdventurePylonTile>())
            return false;
        topLeft = new Point16(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
        return TryGetStation(StationKey(topLeft), out topLeft);
    }

    public static bool IsPlayerNear(Player player, Point16 station)
        => Vector2.Distance(player.Center, new Vector2((station.X + 1.5f) * 16f, (station.Y + 2f) * 16f))
            <= InteractionRadius * 16f;

    public static bool HasUnlockedStationNearby(Player player, AdventurePlayer progress)
        => TileEntity.ByPosition.Values.OfType<AdventurePylonEntity>().Any(pylon =>
            TryGetStation(StationKey(pylon.Position), out Point16 station)
            && progress.HasCrystal(StationKey(station)) && IsPlayerNear(player, station));

    public static bool TryFindLanding(Player player, Point16 station, out Vector2 position)
    {
        position = default;
        foreach (int dx in new[] { 4, -2, 5, -3, 6, -4 })
            for (int dy = 3; dy <= 6; dy++)
            {
                int x = station.X + dx;
                int y = station.Y + dy;
                if (x < 2 || y < 3 || x >= Main.maxTilesX - 2 || y >= Main.maxTilesY - 2)
                    continue;
                Tile floor = Main.tile[x, y + 1];
                if (!floor.HasTile || !(Main.tileSolid[floor.TileType] || Main.tileSolidTop[floor.TileType]))
                    continue;
                Vector2 candidate = new(x * 16 + 8 - player.width / 2f, (y + 1) * 16 - player.height);
                if (Collision.SolidCollision(candidate, player.width, player.height))
                    continue;
                position = candidate;
                return true;
            }
        return false;
    }

    public override void PostWorldLoad()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || !AdventureWorldSystem.IsAdventureWorld
            || AdventureWorldSystem.LegacyPylonsRemoved)
            return;

        // Previous versions created pylons from JSON. Clear those once so the author can place all stations manually.
        var oldPylons = TileEntity.ByPosition.Values.OfType<AdventurePylonEntity>().ToList();
        AdventureBuildPermission.AllowWorldSetup = true;
        try
        {
            foreach (var pylon in oldPylons)
            {
                Point16 origin = pylon.Position;
                for (int x = origin.X; x < origin.X + 3; x++)
                    for (int y = origin.Y; y < origin.Y + 4; y++)
                        if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<AdventurePylonTile>())
                            WorldGen.KillTile(x, y, noItem: true);
                pylon.Kill(origin.X, origin.Y);
            }
        }
        finally
        {
            AdventureBuildPermission.AllowWorldSetup = false;
        }
        AdventureWorldSystem.LegacyPylonsRemoved = true;
        Main.PylonSystem.RequestImmediateUpdate();
    }
}
