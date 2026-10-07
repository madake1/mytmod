namespace madake1.Common.Gacha;
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
    // 设置玩家选中的大奖索引（0-based），-1表示未选
    public void SetSelectedReward(string machineId, int index)
    {
        var data = GetData(machineId);
        data.selectedUpIndex = index;
    }

    // 获取玩家当前选中的大奖索引（如果没选，返回 null）
    public int? GetSelectedReward(string machineId)
    {
        var data = GetData(machineId);
        return data.selectedUpIndex >= 0 ? (int?)data.selectedUpIndex : null;
    }

    // ===================== 大保底状态 =====================
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

