using System.IO;
using Terraria;
using Terraria.ModLoader;
using madake1.Common.Gacha;
using madake1.Common.Networking;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace madake1.Common.Networking
{
    internal static class GachaPacketHandler
    {
        internal static void HandlePacket(BinaryReader reader, int whoAmI)
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
                        Player target = Main.player[playerID];
                        GachaEffects.ShowGoldText(target, new GachaResult(itemType, 1, GachaQuality.Gold)); // 临时构造 GachaResult
                        break;
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<MDK>().Logger.Error($"HandlePacket 异常: {ex}");
            }
        }

        private static void HandleGachaResult(BinaryReader reader, int whoAmI)
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
            GachaEffects.SpawnGachaItems(player, results);

            // 出金文字广播
            foreach (var result in results)
            {
                if (result.quality == GachaQuality.Gold || result.quality == GachaQuality.RealGold)
                {
                    GachaEffects.BroadcastGoldText(player, result.itemType); 
                }
            }
        }
    }
}
