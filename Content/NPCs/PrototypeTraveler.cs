using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace madake1.Content.NPCs;

// Visual and interaction prototype only; no quest or story behavior belongs here.
[AutoloadHead]
public sealed class PrototypeTraveler : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 25;
        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 0;
        NPC.lifeMax = 250;
        NPC.dontTakeDamage = true;
        AnimationType = NPCID.Guide;
    }

    // Keep normal housing spawn disabled; the adventure-world test system places one near the player.
    public override bool CanTownNPCSpawn(int numTownNPCs) => false;

    public override List<string> SetNPCNameList() => new() { DisplayName.Value };

    public override string GetChat() => this.GetLocalization("TestChat").Value;
}
