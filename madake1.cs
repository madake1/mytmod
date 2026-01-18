using System.IO;
using Terraria;
using Terraria.ModLoader;
using madake1.Content.Gacha;
using madake1.Content.Net;
using System;
using System.Collections.Generic;

namespace madake1
{
    public class MDK : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                MyMessageType type = (MyMessageType)reader.ReadByte();

                if (type == MyMessageType.GachaResult)
                {
                    int playerID = reader.ReadInt32();
                    var player = Main.player[playerID];
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

                    // 使用 Tools 统一生成物品
                    Tools.SpawnGachaItems(player, results);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"HandlePacket 异常: {ex}");
            }
        }
    }
}
