using System.IO;
using madake1.Common.Networking;
using Terraria.ModLoader;

namespace madake1
{
    public class MDK : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            GachaPacketHandler.HandlePacket(reader, whoAmI);
        }
    }
}
