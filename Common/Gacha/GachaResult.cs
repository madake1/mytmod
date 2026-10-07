using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace madake1.Common.Gacha
{
    public enum GachaQuality
    {
        Blue,
        Purple,
        Gold,
        RealGold
    }

    public class GachaResult
    {
        public int itemType;
        public int stack;
        public GachaQuality quality;

        public GachaResult(int itemType, int stack, GachaQuality quality)
        {
            this.itemType = itemType;
            this.stack = stack;
            this.quality = quality;
        }
    }

}
