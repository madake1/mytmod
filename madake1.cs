using System.IO;
using Terraria;
using Terraria.ModLoader;
using madake1.Content.Gacha;
using madake1.Content.Net;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using madake1.Utils;

namespace madake1
{
    public class MDK : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                MyMessageType type = (MyMessageType)reader.ReadByte();

                switch (type)
                {
                    case MyMessageType.GachaResult:
                        HandleGachaResult(reader, whoAmI);
                        break;

                    case MyMessageType.GoldDrop:
                        byte playerID = reader.ReadByte();
                        int itemType = reader.ReadInt32();
                        GachaQuality quality = (GachaQuality)reader.ReadByte();
                        Player target = Main.player[playerID];

                        if (quality == GachaQuality.Gold || quality == GachaQuality.RealGold)
                            Tools.ShowGoldText(target, new GachaResult(itemType, 1, quality));
                        else if (quality == GachaQuality.Purple)
                            Tools.ShowPurpleText(target, new GachaResult(itemType, 1, quality));

                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"HandlePacket 异常: {ex}");
            }
        }

        private void HandleGachaResult(BinaryReader reader, int whoAmI)
        {
            int playerID = reader.ReadInt32();
            Player player = Main.player[playerID];
            if (player == null || !player.active) return;

            int count = reader.ReadInt32();
            var results = new List<GachaResult>();

            for (int i = 0; i < count; i++)
            {
                int itemType = reader.ReadInt32();
                int stack = reader.ReadInt32();
                GachaQuality quality = (GachaQuality)reader.ReadByte();
                results.Add(new GachaResult(itemType, stack, quality));
            }

            // 生成物品
            Tools.SpawnGachaItems(player, results);

            // 出金 / 出紫文字广播
            foreach (var result in results)
            {
                if (result.quality == GachaQuality.Gold || result.quality == GachaQuality.RealGold)
                {
                    Tools.BroadcastGoldText(player, result.itemType);
                }
                else if (result.quality == GachaQuality.Purple)
                {
                    Tools.BroadcastPurpleText(player, result.itemType);
                }
            }
        }
    }
}
