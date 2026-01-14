public class GachaResult
{
    public int itemType;   // 抽到的物品 type
    public int stack;      // 数量
    public GachaQuality quality; // 品质

    public GachaResult(int itemType, int stack, GachaQuality quality)
    {
        this.itemType = itemType;
        this.stack = stack;
        this.quality = quality;
    }
}

// 品质枚举
public enum GachaQuality
{
    Blue,
    Purple,
    Gold
}
