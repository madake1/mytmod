using madake1.Common.WorldRules;
using madake1.Content.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Common.Adventure;

// Temporary test placement: one NPC per adventure world, next to the first active player.
public sealed class PrototypeTravelerSpawnSystem : ModSystem
{
    public override void PostUpdateWorld()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient
            || !AdventureWorldSystem.IsAdventureWorld
            || Main.GameUpdateCount % 60 != 0)
            return;

        int npcType = ModContent.NPCType<PrototypeTraveler>();
        if (NPC.AnyNPCs(npcType))
            return;

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];
            if (!player.active || player.dead)
                continue;

            NPC.NewNPC(player.GetSource_Misc("PrototypeTravelerAutoSpawn"),
                (int)player.Center.X + 32, (int)player.Bottom.Y, npcType);
            return;
        }
    }
}
