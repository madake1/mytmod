using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using madake1.Common.Gacha;

namespace madake1.Content.UI
{
    public class RareRewardHint : UIElement
    {
        // Change this RGB value to tune the rare reward text.
        private static readonly Color PurpleText = new Color(190, 115, 238);
        private readonly Asset<Texture2D> icon;
        private string tooltip = "每10连必出以下稀有道具之一：\n暂无";

        public RareRewardHint(Asset<Texture2D> icon)
        {
            this.icon = icon;
            Width.Set(24f, 0f);
            Height.Set(24f, 0f);
        }

        public void SetRewards(List<GachaReward> rewards)
        {
            var names = new List<string>();
            if (rewards != null)
            {
                foreach (var reward in rewards)
                {
                    reward.ParseItemType();
                    if (reward.itemType <= 0) continue;
                    Item item = new Item();
                    item.SetDefaults(reward.itemType);
                    names.Add(item.Name);
                }
            }
            tooltip = "每10连必出以下稀有道具之一：\n" +
                (names.Count == 0 ? "暂无" : string.Join("、", names));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(icon.Value, GetDimensions().ToRectangle(), Color.White);
            if (!IsMouseHovering) return;
            Main.LocalPlayer.mouseInterface = true;

            Vector2 measured = FontAssets.MouseText.Value.MeasureString(tooltip) * 0.8f;
            int width = (int)measured.X + 4;
            int height = (int)measured.Y + 4;
            int x = Math.Clamp((int)Main.MouseScreen.X + 14, 8, Math.Max(8, Main.screenWidth - width - 8));
            int y = Math.Clamp((int)Main.MouseScreen.Y - height - 10, 8, Math.Max(8, Main.screenHeight - height - 8));
            Utils.DrawBorderString(spriteBatch, tooltip, new Vector2(x, y), PurpleText, 0.8f);
        }
    }
}
