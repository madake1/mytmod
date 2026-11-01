$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$sourceDir = Join-Path $PSScriptRoot 'pylon_source'
$assetDir = Join-Path $PSScriptRoot '..\Common\Adventure'
$preview = Join-Path $PSScriptRoot '..\docs\AdventurePylon_preview.png'
$hoverIcon = Join-Path $PSScriptRoot '..\docs\ExamplePylonMapIcon_reference.png'

$drawingAssemblies = if ($PSVersionTable.PSEdition -eq 'Desktop') {
    @('System.Drawing.dll', 'System.Core.dll')
} else {
    @('System.Drawing.Common', 'System.Drawing.Primitives', 'System.Private.Windows.GdiPlus', 'System.Private.Windows.Core', 'System.Runtime')
}

Add-Type -ReferencedAssemblies $drawingAssemblies -TypeDefinition @'
using System;
using System.Drawing;
using System.Drawing.Imaging;

public static class AdventurePylonArt {
    const int W = 48, H = 64, Count = 12, GlowW = 64, GlowH = 80;
    static readonly int[] Bob = { 0, 0, -1, -1, -1, -1, 0, 0, 1, 1, 1, 1 };

    static Bitmap Unpack(Bitmap packed) {
        Bitmap b = new Bitmap(W,H,PixelFormat.Format32bppArgb);
        for(int y=0;y<H;y++) for(int x=0;x<W;x++)
            b.SetPixel(x,y,packed.GetPixel(x+x/16*2,y+y/16*2));
        return b;
    }
    static Bitmap Pack(Bitmap b) {
        Bitmap packed = new Bitmap(54,72,PixelFormat.Format32bppArgb);
        for(int y=0;y<H;y++) for(int x=0;x<W;x++)
            packed.SetPixel(x+x/16*2,y+y/16*2,b.GetPixel(x,y));
        return packed;
    }
    static bool IsInk(Color c) {
        return c.A>128 && Math.Max(c.R,Math.Max(c.G,c.B))<70;
    }
    static Bitmap StrengthenOutline(Bitmap source, Color ink) {
        Bitmap b = new Bitmap(W,H,PixelFormat.Format32bppArgb);
        for(int y=0;y<H;y++) for(int x=0;x<W;x++) {
            Color c=source.GetPixel(x,y);
            if(c.A>128) { b.SetPixel(x,y,c); continue; }
            bool touchesInk=false;
            for(int dy=-1;dy<=1 && !touchesInk;dy++) for(int dx=-1;dx<=1;dx++) {
                int sx=x+dx, sy=y+dy;
                if(sx>=0 && sx<W && sy>=0 && sy<H && IsInk(source.GetPixel(sx,sy))) {
                    touchesInk=true; break;
                }
            }
            if(touchesInk) b.SetPixel(x,y,ink);
        }
        return b;
    }
    static bool InCrystal(int x,int y) {
        if(y<0 || y>46) return false;
        int lo,hi;
        if(y<=20) { lo=10; hi=37; }
        else if(y<=30) { lo=11; hi=36; }
        else if(y<=38) { lo=15; hi=34; }
        else if(y<=43) { lo=17; hi=31; }
        else { lo=19; hi=29; }
        return x>=lo && x<=hi;
    }
    static bool IsStone(Color c) {
        if(c.A<128 || IsInk(c)) return false;
        return Math.Abs(c.R-c.G)<28 && Math.Abs(c.G-c.B)<28;
    }
    static Bitmap MakeRingForeground(Bitmap original) {
        Bitmap ring=new Bitmap(W,H,PixelFormat.Format32bppArgb);
        for(int y=20;y<=32;y++) for(int x=4;x<44;x++) {
            Color c=original.GetPixel(x,y);
            if(IsStone(c)) { ring.SetPixel(x,y,c); continue; }
            if(!IsInk(c)) continue;
            bool byStone=false;
            for(int dy=-2;dy<=2 && !byStone;dy++) for(int dx=-2;dx<=2;dx++) {
                int sx=x+dx,sy=y+dy;
                if(sx>=0 && sx<W && sy>=20 && sy<=32 && IsStone(original.GetPixel(sx,sy))) {
                    byStone=true; break;
                }
            }
            if(byStone) ring.SetPixel(x,y,c);
        }
        return ring;
    }
    static void SplitCrystal(Bitmap original, out Bitmap baseTile, out Bitmap body) {
        baseTile=new Bitmap(W,H,PixelFormat.Format32bppArgb);
        body=new Bitmap(W,H,PixelFormat.Format32bppArgb);
        for(int y=0;y<H;y++) for(int x=0;x<W;x++) {
            Color c=original.GetPixel(x,y);
            if(InCrystal(x,y)) body.SetPixel(x,y,c);
            else baseTile.SetPixel(x,y,c);
        }
    }
    static Bitmap MakeGlow(Bitmap body) {
        const double GlowStrength = 100.0; // Overall visible halo brightness.
        Bitmap sheet=new Bitmap(GlowW,GlowH*Count,PixelFormat.Format32bppArgb);
        for(int f=0;f<Count;f++) {
            double pulse=0.5+0.5*Math.Cos(f*Math.PI*2/Count);
            for(int y=0;y<GlowH;y++) for(int x=0;x<GlowW;x++) {
                int bx=x-8, by=y-8-Bob[f];
                if(bx>=0 && bx<W && by>=0 && by<H && body.GetPixel(bx,by).A>128) continue;
                int nearestSquared=100;
                for(int dy=-8;dy<=8;dy++) for(int dx=-8;dx<=8;dx++) {
                    int sx=bx+dx,sy=by+dy;
                    if(sx<0 || sx>=W || sy<0 || sy>=H || body.GetPixel(sx,sy).A<128) continue;
                    int d=dx*dx+dy*dy;
                    if(d<nearestSquared) nearestSquared=d;
                }
                double edgeGlow=nearestSquared<=64
                    ? GlowStrength*Math.Exp(-nearestSquared/30.0) : 0.0;
                double ellipse=Math.Pow((bx-24.0)/11.0,2)+Math.Pow((by-22.0)/16.0,2);
                double wideGlow=GlowStrength*0.35*Math.Exp(-ellipse/2.0);
                int alpha=(int)(Math.Max(edgeGlow,wideGlow)*(0.55+0.45*pulse));
                // Terraria's tile batch uses premultiplied alpha. Store RGB multiplied
                // by A, otherwise faint pixels appear as an opaque cyan outline.
                if(alpha>0) sheet.SetPixel(x,y+f*GlowH,Color.FromArgb(alpha,
                    108*alpha/255,190*alpha/255,248*alpha/255));
            }
        }
        return sheet;
    }
    static void DrawNearest(Graphics g,Bitmap b,int x,int y) {
        g.DrawImage(b,new Rectangle(x,y,W*4,H*4),new Rectangle(0,0,W,H),GraphicsUnit.Pixel);
    }
    static Bitmap PreviewGlow(Bitmap premultiplied) {
        Bitmap preview=new Bitmap(premultiplied.Width,premultiplied.Height,PixelFormat.Format32bppArgb);
        for(int y=0;y<premultiplied.Height;y++) for(int x=0;x<premultiplied.Width;x++) {
            Color c=premultiplied.GetPixel(x,y);
            if(c.A==0) continue;
            preview.SetPixel(x,y,Color.FromArgb(c.A,
                Math.Min(255,c.R*255/c.A),Math.Min(255,c.G*255/c.A),Math.Min(255,c.B*255/c.A)));
        }
        return preview;
    }
    public static void Build(string lockedPath,string unlockedPath,string outputDir,string previewPath) {
        using(Bitmap packedLocked=new Bitmap(lockedPath))
        using(Bitmap packedUnlocked=new Bitmap(unlockedPath))
        using(Bitmap rawLocked=Unpack(packedLocked))
        using(Bitmap rawUnlocked=Unpack(packedUnlocked))
        using(Bitmap locked=StrengthenOutline(rawLocked,Color.FromArgb(17,18,25)))
        using(Bitmap unlocked=StrengthenOutline(rawUnlocked,Color.FromArgb(0,0,17))) {
            Bitmap baseTile, body;
            SplitCrystal(unlocked,out baseTile,out body);
            using(baseTile) using(body)
            using(Bitmap ring=MakeRingForeground(unlocked))
            using(Bitmap glow=MakeGlow(body))
            using(Bitmap previewGlow=PreviewGlow(glow)) {
                using(Bitmap packed=Pack(locked)) packed.Save(System.IO.Path.Combine(outputDir,"AdventurePylonTile.png"),ImageFormat.Png);
                baseTile.Save(System.IO.Path.Combine(outputDir,"AdventurePylonTile_Unlocked.png"),ImageFormat.Png);
                body.Save(System.IO.Path.Combine(outputDir,"AdventurePylonTile_CrystalBody.png"),ImageFormat.Png);
                ring.Save(System.IO.Path.Combine(outputDir,"AdventurePylonTile_RingForeground.png"),ImageFormat.Png);
                glow.Save(System.IO.Path.Combine(outputDir,"AdventurePylonTile_Glow.png"),ImageFormat.Png);
                using(Bitmap preview=new Bitmap(192*3+48,256,PixelFormat.Format32bppArgb))
                using(Graphics g=Graphics.FromImage(preview)) {
                    g.Clear(Color.FromArgb(14,20,30));
                    g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.PixelOffsetMode=System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    DrawNearest(g,locked,0,0);
                    for(int f=0;f<2;f++) {
                        int px=240+f*192;
                        DrawNearest(g,baseTile,px,0);
                        int frame=f==0?0:4;
                        g.DrawImage(previewGlow,new Rectangle(px-32,-32,256,320),
                            new Rectangle(0,frame*GlowH,GlowW,GlowH),GraphicsUnit.Pixel);
                        g.DrawImage(body,new Rectangle(px,f==0?0:-4,192,256),new Rectangle(0,0,W,H),GraphicsUnit.Pixel);
                        DrawNearest(g,ring,px,0);
                    }
                    preview.Save(previewPath,ImageFormat.Png);
                }
            }
        }
    }
}
'@

[AdventurePylonArt]::Build(
    (Join-Path $sourceDir 'AdventurePylonTile_Locked.png'),
    (Join-Path $sourceDir 'AdventurePylonTile_Unlocked.png'),
    $assetDir,
    $preview)

# MouseOver displays the placeable item's texture. Use the gray version of the
# same simple crystal silhouette as the blue map icon.
Copy-Item -LiteralPath $hoverIcon -Destination (Join-Path $assetDir 'AdventurePylonItem.png') -Force
