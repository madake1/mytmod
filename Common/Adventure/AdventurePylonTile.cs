using System;
using System.IO;
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
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace madake1.Common.Adventure;

// Tile layout and artwork follow ExampleMod's ExamplePylonTile. The station rules are ours.
public sealed class AdventurePylonEntity : TEModdedPylon
{
    internal Guid StationId { get; private set; } = Guid.NewGuid();

    public override void SaveData(TagCompound tag) => tag["stationId"] = StationId.ToString("N");

    public override void LoadData(TagCompound tag)
    {
        if (Guid.TryParseExact(tag.GetString("stationId"), "N", out Guid id))
            StationId = id;
    }

    public override void NetSend(BinaryWriter writer) => writer.Write(StationId.ToString("N"));

    public override void NetReceive(BinaryReader reader)
    {
        if (Guid.TryParseExact(reader.ReadString(), "N", out Guid id))
            StationId = id;
    }
}

public sealed class AdventurePylonTile : ModPylon
{
    private static readonly int[] CrystalBob = { 0, 0, -1, -1, -1, -1, 0, 0, 1, 1, 1, 1 };
    private Asset<Texture2D> lockedTexture;
    private Asset<Texture2D> unlockedTexture;
    private Asset<Texture2D> crystalBodyTexture;
    private Asset<Texture2D> ringForegroundTexture;
    private Asset<Texture2D> glowTexture;
    private Asset<Texture2D> mapIcon;

    public override void Load()
    {
        if (Main.dedServ) return;
        lockedTexture = ModContent.Request<Texture2D>(Texture);
        unlockedTexture = ModContent.Request<Texture2D>(Texture + "_Unlocked");
        crystalBodyTexture = ModContent.Request<Texture2D>(Texture + "_CrystalBody");
        ringForegroundTexture = ModContent.Request<Texture2D>(Texture + "_RingForeground");
        glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow");
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
        if (!AdventureWorldSystem.IsAdventureWorld
            || !CrystalWorldSystem.TryGetStationAtTile(i, j, out var station))
            return true;

        string key = CrystalWorldSystem.StationKey(station);
        if (!Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(key))
        {
            AdventurePackets.Request(AdventureAction.UnlockCrystal, key);
            return true;
        }
        return base.RightClick(i, j);
    }

    public override void MouseOver(int i, int j)
    {
        Main.LocalPlayer.cursorItemIconEnabled = true;
        bool unlocked = AdventureWorldSystem.IsAdventureWorld
            && CrystalWorldSystem.TryGetStationAtTile(i, j, out var station)
            && Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(CrystalWorldSystem.StationKey(station));
        Main.LocalPlayer.cursorItemIconID = unlocked
            ? ModContent.ItemType<AdventurePylonUnlockedHoverItem>()
            : ModContent.ItemType<AdventurePylonItem>();
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        CrystalWorldSystem.RevokeStationAt(new Point16(i, j));
        ModContent.GetInstance<AdventurePylonEntity>().Kill(i, j);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        if (Main.dedServ || !CrystalWorldSystem.TryGetStationAtTile(i, j, out var station)
            || !Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(CrystalWorldSystem.StationKey(station)))
            return;
        float pulse = 0.5f + 0.5f * (float)Math.Cos(Main.GameUpdateCount * Math.PI / 72d);
        r = 0.5f + pulse * 0.08f;
        g = 0.8f + pulse * 0.13f;
        b = 0.7f + pulse * 0.16f;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];
        var station = new Point16(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
        bool unlocked = AdventureWorldSystem.IsAdventureWorld
            && Main.LocalPlayer.GetModPlayer<AdventurePlayer>().HasCrystal(CrystalWorldSystem.StationKey(station));
        Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        if (!unlocked)
        {
            spriteBatch.Draw(lockedTexture.Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + offset,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j));
            return false;
        }

        // Draw the whole unlocked pylon from its top-left tile. Drawing the crystal in
        // SpecialDraw was skipped by the tile renderer when PreDraw returned false.
        if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
            return false;

        int frame = (int)(Main.GameUpdateCount / 12 % CrystalBob.Length);
        Vector2 origin = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + offset;
        Texture2D glow = glowTexture.Value;
        int glowFrameHeight = glow.Height / CrystalBob.Length;
        spriteBatch.Draw(glow,
            origin - new Vector2((glow.Width - 48) / 2f, (glowFrameHeight - 64) / 2f),
            new Rectangle(0, frame * glowFrameHeight, glow.Width, glowFrameHeight), Color.White);
        spriteBatch.Draw(unlockedTexture.Value, origin, null, Lighting.GetColor(i, j));
        spriteBatch.Draw(crystalBodyTexture.Value, origin + new Vector2(0, CrystalBob[frame]),
            null, Color.Lerp(Lighting.GetColor(i, j), Color.White, 0.7f));
        spriteBatch.Draw(ringForegroundTexture.Value, origin,
            null, Lighting.GetColor(i, j));
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
