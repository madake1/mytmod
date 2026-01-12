using System.Collections.Generic;

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

public class GachaReward
{
    public string item;
    public int count;
}
