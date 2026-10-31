using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using madake1.Common.WorldRules;
using madake1.Content.Items.Tiles;
using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader;

namespace madake1.Common.Gacha
{
    public class GachaSystem : ModSystem
    {
        public static List<GachaMachineConfig> Machines;

        public override void OnWorldLoad()
        {
            Machines = new List<GachaMachineConfig>();

            try
            {
                // 使用 Mod 内置资源读取 JSON
                byte[] bytes = Mod.GetFileBytes("Data/Gacha/GachaConfig.json");
                string json = System.Text.Encoding.UTF8.GetString(bytes);

                var loaded = JsonConvert.DeserializeObject<List<GachaMachineConfig>>(json)
                    ?? throw new InvalidDataException("扭蛋机配置为空。");
                if (loaded.Any(m => m == null || string.IsNullOrWhiteSpace(m.mapId)
                    || string.IsNullOrWhiteSpace(m.machineId) || string.IsNullOrWhiteSpace(m.displayName)))
                    throw new InvalidDataException("扭蛋机必须配置地图 ID、机器 ID 和名称。");
                if (loaded.GroupBy(m => m.machineId, StringComparer.Ordinal).Any(g => g.Count() > 1))
                    throw new InvalidDataException("扭蛋机 ID 重复。");
                Machines = loaded;

                Main.NewText($"已加载扭蛋机数量: {Machines?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"加载扭蛋机配置失败: {ex}");
                Main.NewText("未找到或解析 GachaConfig.json ❌");
            }
        }

        public override void PostWorldLoad()
        {
            if (!AdventureWorldSystem.IsBundledWorld || Machines == null)
                return;

            int tileType = ModContent.TileType<GachaMachineTile>();
            foreach (var machine in Machines.Where(m => m.placeOnWorldLoad && m.mapId == AdventureWorldSystem.MapId))
            {
                int left = machine.centerX - 2;
                int top = machine.centerY - 4;
                if (left < 0 || top < 0 || left + 3 >= Main.maxTilesX || machine.centerY + 1 >= Main.maxTilesY)
                {
                    Mod.Logger.Warn($"扭蛋机 {machine.machineId} 的坐标超出世界边界。");
                    continue;
                }

                bool alreadyPlaced = true;
                bool clear = true;
                for (int x = left; x < left + 4; x++)
                    for (int y = top; y < top + 5; y++)
                    {
                        var tile = Main.tile[x, y];
                        alreadyPlaced &= tile.HasTile && tile.TileType == tileType;
                        clear &= !tile.HasTile;
                    }

                if (alreadyPlaced)
                    continue;
                if (!clear)
                {
                    Mod.Logger.Warn($"扭蛋机 {machine.machineId} 的位置被方块占用，未自动放置。");
                    continue;
                }

                AdventureBuildPermission.AllowWorldSetup = true;
                try
                {
                    if (!WorldGen.PlaceObject(machine.centerX, machine.centerY, tileType, mute: true))
                        Mod.Logger.Warn($"扭蛋机 {machine.machineId} 放置失败，请检查承重地面。");
                }
                finally
                {
                    AdventureBuildPermission.AllowWorldSetup = false;
                }
            }
        }


        public static GachaMachineConfig FindMachine(int tileX, int tileY)
        {
            if (Machines == null)
                return null;

            foreach (var m in Machines)
            {
                if (m.mapId != AdventureWorldSystem.MapId)
                    continue;
                float dx = tileX - m.centerX;
                float dy = tileY - m.centerY;

                if (dx * dx + dy * dy <= m.radius * m.radius)
                    return m;
            }

            return null;
        }
    }
}
