using madake1.Content.Gacha;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;

namespace madake1.Content.Net
{
    public enum MyMessageType : byte
    {
        GachaRequest,
        GachaResult
    }

    public static class GachaRequest
    {
        /// <summary>
        /// UI 层调用：客户端抽卡结果
        /// </summary>
        public static void Request(Player player, GachaMachineConfig config, List<GachaResult> results)
        {
            string machineId = config.GetMachineId();

            // ================= 单机模式 =================
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Tools.SpawnGachaItems(player, results);
                // Main.NewText($"[GachaRequest] 单机抽卡完成 machineId={machineId}, count={results.Count}", Color.Yellow);
                return;
            }

            // ================= 多人模式 =================
            // Main.NewText($"[GachaRequest] 客户端抽卡发送服务器 machineId={machineId}, count={results.Count}", Color.Yellow);

            // 发给服务器生成物品
            var packet = ModContent.GetInstance<madake1.MDK>().GetPacket();
            packet.Write((byte)MyMessageType.GachaResult);
            packet.Write(player.whoAmI);
            packet.Write(results.Count);
            foreach (var r in results)
            {
                packet.Write(r.itemType);
                packet.Write(r.stack);
                packet.Write((byte)r.quality);
            }
            packet.Send();
        }
    }
}
