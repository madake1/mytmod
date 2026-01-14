using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

public class GachaPlayer : ModPlayer
{
    // key = machineId（由 json 的 centerX+centerY 生成）
    public Dictionary<string, GachaMachineData> machineData = new();
    public GachaMachineData GetData(string machineId)
    {
        if (!machineData.TryGetValue(machineId, out var data))
        {
            data = new GachaMachineData
            {
                machineId = machineId
            };
            machineData[machineId] = data;
        }
        return data;
    }

    public override void SaveData(TagCompound tag)
    {
        // Dictionary -> List<TagCompound> 进行存储
        var list = machineData.Values.Select(d => d.Save()).ToList();
        tag["machineData"] = list;
    }

    public override void LoadData(TagCompound tag)
    {
        machineData = new Dictionary<string, GachaMachineData>();

        if (!tag.ContainsKey("machineData"))
            return;

        var list = tag.GetList<TagCompound>("machineData");

        foreach (var entry in list)
        {
            var data = GachaMachineData.Load(entry);
            machineData[data.machineId] = data;
        }
    }
    // ===================== 获取/设置选中的大奖 =====================
    // 设置玩家选中的大奖 itemType
    public void SetSelectedReward(string machineId, int itemType)
    {
        var data = GetData(machineId);
        data.selectedUpItemType = itemType;
    }

    // 获取玩家当前选中的大奖 itemType（如果没选，返回 null）
    public int? GetSelectedReward(string machineId)
    {
        var data = GetData(machineId);
        return data.selectedUpItemType > 0 ? (int?)data.selectedUpItemType : null;
    }

    // 检查当前大保底是否激活
    public bool IsUpGuaranteeActive(string machineId)
    {
        var data = GetData(machineId);
        return data.upGuaranteeActive;
    }

    public void SetUpGuarantee(string machineId, bool active)
    {
        var data = GetData(machineId);
        data.upGuaranteeActive = active;
    }
}

// ====================== GachaMachineData ======================
public class GachaMachineData
{
    public string machineId; // 用于保存 Dictionary 的 key

    public int totalPullCount;
    public int pullsSinceLastGold;
    public int pullsSinceLastPurple;

    public int selectedUpItemType = -1;
    public bool upGuaranteeActive;

    // 保存成 TagCompound
    public TagCompound Save()
    {
        return new TagCompound
        {
            ["machineId"] = machineId,
            ["totalPullCount"] = totalPullCount,
            ["pullsSinceLastGold"] = pullsSinceLastGold,
            ["pullsSinceLastPurple"] = pullsSinceLastPurple,
            ["selectedUpItemType"] = selectedUpItemType,
            ["upGuaranteeActive"] = upGuaranteeActive
        };
    }

    // 从 TagCompound 加载
    public static GachaMachineData Load(TagCompound tag)
    {
        return new GachaMachineData
        {
            machineId = tag.GetString("machineId"),
            totalPullCount = tag.GetInt("totalPullCount"),
            pullsSinceLastGold = tag.GetInt("pullsSinceLastGold"),
            pullsSinceLastPurple = tag.GetInt("pullsSinceLastPurple"),
            selectedUpItemType = tag.GetInt("selectedUpItemType"),
            upGuaranteeActive = tag.GetBool("upGuaranteeActive")
        };
    }
    
}
