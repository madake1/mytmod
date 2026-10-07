using System.Collections.Generic;
using System.IO;
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

                Machines = JsonConvert.DeserializeObject<List<GachaMachineConfig>>(json);

                Main.NewText($"已加载扭蛋机数量: {Machines?.Count ?? 0}");
            }
            catch
            {
                Main.NewText("未找到或解析 GachaConfig.json ❌");
            }
        }


        public static GachaMachineConfig FindMachine(int tileX, int tileY)
        {
            if (Machines == null)
                return null;

            foreach (var m in Machines)
            {
                float dx = tileX - m.centerX;
                float dy = tileY - m.centerY;

                if (dx * dx + dy * dy <= m.radius * m.radius)
                    return m;
            }

            return null;
        }
    }
}
