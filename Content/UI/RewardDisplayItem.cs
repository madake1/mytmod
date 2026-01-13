using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.ModLoader;
using System.IO;
using System;

namespace Madake1.Content.UI;
public class RewardDisplayItem : UIElement
{
    public Item DisplayItem;
    public int Count;
    public bool Selectable;
    public bool Selected;

    private Color backgroundColor;
    
    // #region agent log
    private static void Log(string location, string message, object data, string hypothesisId = "")
    {
        try
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Terraria", "tModLoader", "ModSources", ".cursor", "debug.log");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            var logEntry = new { sessionId = "debug-session", runId = "run1", hypothesisId, location, message, data, timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() };
            File.AppendAllText(logPath, Newtonsoft.Json.JsonConvert.SerializeObject(logEntry) + "\n");
        }
        catch { }
    }
    // #endregion

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
        if (DisplayItem.type > Terraria.ID.ItemID.None && DisplayItem.type < TextureAssets.Item.Length)
        {
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
        // #region agent log
        if (IsMouseHovering)
        {
            Log("RewardDisplayItem.cs:Update", "Mouse hovering", new { itemType = DisplayItem.type, itemName = DisplayItem.Name, itemIsNull = DisplayItem == null }, "G");
        }
        // #endregion
        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true; // 关键：必须设置这个才能显示tooltip
            Main.HoverItem = DisplayItem.Clone();
            Main.hoverItemName = DisplayItem.Name;
            // #region agent log
            Log("RewardDisplayItem.cs:Update", "HoverItem set", new { hoverItemType = Main.HoverItem.type, hoverItemName = Main.hoverItemName, mouseInterface = Main.LocalPlayer.mouseInterface }, "G");
            // #endregion
        }
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Selectable)
            return;

        Selected = !Selected;
    }
}
