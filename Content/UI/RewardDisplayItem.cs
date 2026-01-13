using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Madake1.Content.UI;
public class RewardDisplayItem : UIElement
{
    public Item DisplayItem;
    public int Count;
    public bool Selectable;
    public bool Selected;

    private Color backgroundColor;

    public RewardDisplayItem(Item item, int count, Color bgColor, bool selectable = false)
    {
        DisplayItem = item;
        Count = count;
        Selectable = selectable;
        backgroundColor = bgColor;

        Width.Set(60f, 0f);
        Height.Set(60f, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        CalculatedStyle dims = GetDimensions();

        // 背景
        spriteBatch.Draw(
            TextureAssets.MagicPixel.Value,
            dims.ToRectangle(),
            backgroundColor * 0.8f
        );

        // 选中高亮
        if (Selected)
        {
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                dims.ToRectangle(),
                Color.Yellow * 0.3f
            );
        }

        // 物品贴图
        Texture2D texture = TextureAssets.Item[DisplayItem.type].Value;
        Rectangle frame = Main.itemAnimations[DisplayItem.type]?.GetFrame(texture) ?? texture.Frame();
        float scale = 1f;

        Vector2 pos = dims.Center();
        spriteBatch.Draw(
            texture,
            pos,
            frame,
            Color.White,
            0f,
            frame.Size() / 2f,
            scale,
            SpriteEffects.None,
            0f
        );

        // 数量
        if (Count > 1)
        {
            Utils.DrawBorderString(
                spriteBatch,
                $"x{Count}",
                new Vector2(dims.X + 4, dims.Y + 38),
                Color.White,
                0.7f
            );
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Hover 显示 Tooltip（关键）
        if (IsMouseHovering)
        {
            Main.HoverItem = DisplayItem.Clone();
            Main.hoverItemName = DisplayItem.Name;
        }
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Selectable)
            return;

        Selected = !Selected;
    }
}
