namespace madake1.Content.UI;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria;
using madake1.Content.UI;
using Terraria.GameContent.UI.Elements;

public class RewardListState : UIState
{
    public UIList RewardList;
    private UIScrollbar scrollbar;

    public override void OnInitialize()
    {
        UIPanel panel = new UIPanel();
        panel.SetPadding(10);
        panel.Width.Set(300f, 0f);
        panel.Height.Set(200f, 0f);
        panel.Left.Set(400f, 0f);
        panel.Top.Set(200f, 0f);
        Append(panel);

        RewardList = new UIList();
        RewardList.Width.Set(-20f, 1f);
        RewardList.Height.Set(0f, 1f);
        RewardList.ListPadding = 6f;
        panel.Append(RewardList);

        scrollbar = new UIScrollbar();
        scrollbar.Height.Set(0f, 1f);
        scrollbar.Left.Set(-20f, 1f);
        panel.Append(scrollbar);

        RewardList.SetScrollbar(scrollbar);
    }

    public void AddReward(Item item, int count, Color bgColor, bool selectable = false)
    {
        RewardDisplayItem uiItem =
            new RewardDisplayItem(item, count, bgColor, selectable);

        RewardList.Add(uiItem);
    }

    public void Clear()
    {
        RewardList.Clear();
    }
}
