using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using System.Linq;

public class GachaMachineConfig
{
    public int centerX;
    public int centerY;
    public int radius;

    public List<GachaReward> rewardsRealGold;
    public List<GachaReward> rewardsGold;
    public List<GachaReward> rewardsPurple;
    public List<GachaReward> rewardsBlue;
}

// 奖励数据类
public class GachaReward
{
    public string item; // JSON 中的短名字
    public int itemType; // 解析后的 ItemType
    public int count;

    public void ParseItemType()
    {
        // #region agent log
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "GachaMachineConfig.cs:ParseItemType", message = "ParseItemType entry", data = new { item = this.item, itemTypeBefore = this.itemType }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
        }
        catch { }
        // #endregion
        if (item.StartsWith("Terraria.ID.ItemID."))
        {
            string name = item.Replace("Terraria.ID.ItemID.", "");
            var field = typeof(Terraria.ID.ItemID).GetField(name);
            if (field != null)
            {
                itemType = Convert.ToInt32(field.GetValue(null));
                // #region agent log
                try
                {
                    string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                    var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "GachaMachineConfig.cs:ParseItemType", message = "Vanilla item parsed", data = new { item = this.item, name = name, itemType = this.itemType }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
                    File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
                }
                catch { }
                // #endregion
            }
            else
            {
                itemType = 0;
                // #region agent log
                try
                {
                    string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                    var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "GachaMachineConfig.cs:ParseItemType", message = "Vanilla item field not found", data = new { item = this.item, name = name, itemType = this.itemType }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
                    File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
                }
                catch { }
                // #endregion
            }

        }
        else
        {
            // 自定义 ModItem，通过映射表
            if (ModItemMap.map.ContainsKey(item))
            {
                itemType = ModItemMap.map[item];
                // #region agent log
                try
                {
                    string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                    var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "GachaMachineConfig.cs:ParseItemType", message = "Mod item parsed", data = new { item = this.item, itemType = this.itemType }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
                    File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
                }
                catch { }
                // #endregion
            }
            else
            {
                itemType = 0;
                // #region agent log
                try
                {
                    string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
                    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                    var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId = "D", location = "GachaMachineConfig.cs:ParseItemType", message = "Mod item not found in map", data = new { item = this.item, itemType = this.itemType }, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
                    File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
                }
                catch { }
                // #endregion
            }
        }
    }
}
