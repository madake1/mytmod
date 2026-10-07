namespace madake1.Common.Gacha;
using Terraria.ModLoader.IO;
// ====================== GachaMachineData ======================
public class GachaMachineData
{
    public string machineId; // 用于保存 Dictionary 的 key

    public int totalPullCount;
    public int pullsSinceLastGold;
    public int pullsSinceLastPurple;

    public int selectedUpIndex = -1; // 改为列表索引，-1 表示未选中
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
            ["selectedUpIndex"] = selectedUpIndex,
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
            selectedUpIndex = tag.GetInt("selectedUpIndex"),
            upGuaranteeActive = tag.GetBool("upGuaranteeActive")
        };
    }
}
