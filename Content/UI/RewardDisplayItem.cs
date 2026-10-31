using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.ModLoader;
using System.IO;
using System;

namespace madake1.Content.UI;
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

        Width.Set(50f, 0f);
        Height.Set(50f, 0f);
        
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
        Rectangle bounds = dims.ToRectangle();
        int outline = 2;
        Color outlineColor = new Color(30, 20, 29);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bounds.X, bounds.Y, bounds.Width, outline), outlineColor);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bounds.X, bounds.Bottom - outline, bounds.Width, outline), outlineColor);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bounds.X, bounds.Y, outline, bounds.Height), outlineColor);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bounds.Right - outline, bounds.Y, outline, bounds.Height), outlineColor);

        // 选中高亮（更明显）
        if (Selected)
        {
            Rectangle rect = dims.ToRectangle();
            // 使用更亮的颜色和更大的透明度作为背景
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                rect,
                Color.Yellow * 0.6f // 从0.3f增加到0.6f，更明显
            );
            // 绘制四个边框（更粗的边框，3像素）
            int borderWidth = 2;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), Color.Yellow); // 上
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - borderWidth, rect.Width, borderWidth), Color.Yellow); // 下
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), Color.Yellow); // 左
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - borderWidth, rect.Y, borderWidth, rect.Height), Color.Yellow); // 右
        }

        // 物品贴图
        if (DisplayItem.type > Terraria.ID.ItemID.None && DisplayItem.type < TextureAssets.Item.Length)
        {
            // 确保texture已加载（特别是原版物品）
            if (TextureAssets.Item[DisplayItem.type].State == ReLogic.Content.AssetState.NotLoaded)
            {
                Main.instance.LoadItem(DisplayItem.type);
            }
            
            Texture2D texture = TextureAssets.Item[DisplayItem.type].Value;
            if (texture != null && texture.Width > 1 && texture.Height > 1) // 检查texture是否有效
            {
                Rectangle frame = Main.itemAnimations[DisplayItem.type]?.GetFrame(texture) ?? texture.Frame();
                float scale = 1f;
                
                // 计算合适的缩放比例以适应60x60的显示区域
                float maxSize = 50f; // 留出一些边距
                if (frame.Width > maxSize || frame.Height > maxSize)
                {
                    if (frame.Width > frame.Height)
                        scale = maxSize / frame.Width;
                    else
                        scale = maxSize / frame.Height;
                }

                Vector2 pos = dims.Center();
                spriteBatch.Draw(
                    texture,
                    pos,
                    frame,
                    DisplayItem.GetAlpha(Color.White),
                    0f,
                    frame.Size() / 2f,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        // 数量显示（总是显示，不只是>1时）
        if (Count > 0)
        {
            // 在右上角显示数量
            string countText = Count.ToString();
            Vector2 textPos = new Vector2(dims.X + dims.Width - 20f, dims.Y + 2f);
            // 绘制背景（黑色半透明）使文字更清晰
            Utils.DrawBorderString(
                spriteBatch,
                countText,
                textPos + new Vector2(1f, 1f),
                Color.Black * 0.8f,
                0.8f
            );
            // 绘制文字
            Utils.DrawBorderString(
                spriteBatch,
                countText,
                textPos,
                Color.White,
                0.8f
            );
        }
        
        // 在Draw中处理悬停tooltip（关键：必须在Draw中设置）
        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
            Main.HoverItem = DisplayItem.Clone();
            Main.hoverItemName = DisplayItem.Name;
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // 悬停处理已移到Draw方法中
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        // 移除这里的处理，让父类GachaMachineUI的SelectRewardItem方法来处理单选逻辑
        // 这样确保只能选中一个
        if (!Selectable)
            return;
        base.LeftClick(evt);
        // 不在这里处理Selected状态，由父类统一管理
    }
}
