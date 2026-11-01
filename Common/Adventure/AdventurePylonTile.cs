using madake1.Common.WorldRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;

namespace madake1.Common.Adventure;

// Tile layout and artwork follow ExampleMod's ExamplePylonTile. The station rules are ours.
public sealed class AdventurePylonEntity : TEModdedPylon { }

public sealed class AdventurePylonTile : ModPylon
{
    private Asset<Texture2D> lockedTexture;
    private Asset<Texture2D> unlockedTexture;
    private Asset<Texture2D> mapIcon;

    public override void Load()
    {
        if (Main.dedServ) return;
        lockedTexture = ModContent.Request<Texture2D>(Texture);
        unlockedTexture = ModContent.Request<Texture2D>(Texture + "_Unlocked");
        mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
    }

    public override void SetStaticDefaults()
    {
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        VanillaFallbackOnModDeletion = TileID.TeleportationPylon;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        var entity = ModContent.GetInstance<AdventurePylonEntity>();
        TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(entity.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(entity.Hook_AfterPlacement, -1, 0, false);
        TileObjectData.addTile(Type);

        TileID.Sets.PreventsSandfall[Type] = true;
        AddToArray(ref TileID.Sets.CountsAsPylon);
        AddMapEntry(new Color(60, 85, 170), CreateMapEntryName());
    }

    public override bool CanPlacePylon() => true;
    public override NPCShop.Entry GetNPCShopEntry() => null;

    public override bool RightClick(int i, int j)
    {
        if (AdventureWorldSystem.IsAdventureWorld
            && CrystalWorldSystem.TryGetStationAtTile(i, j, out var station))
        {
            string key = CrystalWorldSystem.StationKey(station);
            if (!Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(key))
                AdventurePackets.Request(AdventureAction.UnlockCrystal, key);
        }
        return base.RightClick(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        Main.LocalPlayer.cursorItemIconEnabled = true;
        Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AdventurePylonItem>();
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
        => ModContent.GetInstance<AdventurePylonEntity>().Kill(i, j);

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.dedServ || !CrystalWorldSystem.TryGetStationAtTile(i, j, out var station)
            || !Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(CrystalWorldSystem.StationKey(station)))
            return;
        r = 0.13f;
        g = 0.22f;
        b = 0.5f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];
        var station = new Point16(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
        bool unlocked = AdventureWorldSystem.IsAdventureWorld
            && Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(CrystalWorldSystem.StationKey(station));
        Texture2D texture = (unlocked ? unlockedTexture : lockedTexture).Value;
        Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        spriteBatch.Draw(texture,
            new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + offset,
            new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j));
        return false;
    }

    public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText,
        TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
    {
        string key = CrystalWorldSystem.StationKey(pylonInfo.PositionInTiles);
        if (!AdventureWorldSystem.IsAdventureWorld
            || !Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(key))
            return;
        bool hovered = DefaultDrawMapIcon(ref context, mapIcon,
            pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f),
            drawColor, deselectedScale, selectedScale);
        if (!hovered) return;
        Main.cancelWormHole = true;
        mouseOverText = "传送水晶";
        if (!Main.mouseLeft || !Main.mouseLeftRelease) return;
        Main.mouseLeftRelease = false;
        PlayerInput.LockGamepadButtons("MouseLeft");
        Main.mapFullscreen = false;
        AdventurePackets.Request(AdventureAction.Teleport, key);
    }
}
