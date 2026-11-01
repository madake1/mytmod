Add-Type -AssemblyName System.Drawing

# Each pixel drawn here is scaled to a 2x2 block in the game texture.
# 25 frames of 40x56 match ExampleMod's ExamplePerson town-NPC animation layout.
$out = Join-Path $PSScriptRoot '../Content/NPCs'
$previewPath = Join-Path $PSScriptRoot '../docs/PrototypeTraveler_preview.png'
$c = @{
    ink   = [System.Drawing.Color]::FromArgb(29, 28, 38)
    hair  = [System.Drawing.Color]::FromArgb(34, 91, 106)
    shine = [System.Drawing.Color]::FromArgb(72, 144, 151)
    hairDark = [System.Drawing.Color]::FromArgb(24, 62, 78)
    skin  = [System.Drawing.Color]::FromArgb(225, 171, 124)
    shade = [System.Drawing.Color]::FromArgb(171, 110, 80)
    shirt = [System.Drawing.Color]::FromArgb(214, 196, 154)
    shirtDark = [System.Drawing.Color]::FromArgb(157, 143, 112)
    gold  = [System.Drawing.Color]::FromArgb(202, 137, 58)
    light = [System.Drawing.Color]::FromArgb(232, 183, 89)
    goldDark = [System.Drawing.Color]::FromArgb(142, 91, 45)
    plum  = [System.Drawing.Color]::FromArgb(91, 63, 92)
    boot  = [System.Drawing.Color]::FromArgb(94, 57, 47)
    bootLight = [System.Drawing.Color]::FromArgb(148, 99, 65)
}

function Paint-Rect($b, [int]$x, [int]$y, [int]$w, [int]$h, $color) {
    for ($iy = $y; $iy -lt $y + $h; $iy++) {
        for ($ix = $x; $ix -lt $x + $w; $ix++) {
            if ($ix -ge 0 -and $ix -lt $b.Width -and $iy -ge 0 -and $iy -lt $b.Height) {
                $b.SetPixel($ix, $iy, $color)
            }
        }
    }
}

function Draw-Pose($b, [int]$frame) {
    $walking = $frame -ge 1 -and $frame -le 15
    $phase = if ($walking) { $frame % 4 } else { 0 }
    $sitting = $frame -ge 16 -and $frame -le 20
    $gesture = $frame -ge 21
    $dy = if ($sitting) { 2 } elseif ($phase -eq 2) { -1 } else { 0 }

    # Boots and trousers have a four-pose stride, rather than translating one image.
    $lx = switch ($phase) { 1 { 6 } 3 { 8 } default { 7 } }
    $rx = switch ($phase) { 1 { 12 } 3 { 10 } default { 11 } }
    if ($sitting) { $lx = 6; $rx = 10 }
    $ly = 21 + $dy - $(if ($phase -eq 1) { 1 } else { 0 })
    $ry = 21 + $dy - $(if ($phase -eq 3) { 1 } else { 0 })
    Paint-Rect $b $lx $ly 3 5 $c.ink; Paint-Rect $b ($lx + 1) $ly 1 3 $c.plum
    Paint-Rect $b $rx $ry 3 5 $c.ink; Paint-Rect $b ($rx + 1) $ry 1 3 $c.plum
    Paint-Rect $b ($lx - 1) (25 + $dy) 4 2 $c.ink; Paint-Rect $b $lx (25 + $dy) 3 1 $c.boot
    Paint-Rect $b $rx (25 + $dy) 4 2 $c.ink; Paint-Rect $b ($rx + 1) (25 + $dy) 3 1 $c.boot
    Paint-Rect $b $lx (24 + $dy) 1 1 $c.bootLight
    Paint-Rect $b ($rx + 1) (24 + $dy) 1 1 $c.bootLight

    # Torso and short shoulder mantle. A one-pixel inset makes a 2-pixel outline.
    Paint-Rect $b 6 (13 + $dy) 9 9 $c.ink
    Paint-Rect $b 7 (15 + $dy) 7 5 $c.shirt
    Paint-Rect $b 7 (17 + $dy) 1 3 $c.shirtDark
    Paint-Rect $b 11 (18 + $dy) 2 1 $c.shirtDark
    Paint-Rect $b 6 (13 + $dy) 9 3 $c.gold
    Paint-Rect $b 7 (13 + $dy) 6 1 $c.light
    Paint-Rect $b 6 (15 + $dy) 3 1 $c.goldDark
    Paint-Rect $b 7 (20 + $dy) 7 2 $c.plum
    Paint-Rect $b 10 (20 + $dy) 1 1 $c.light
    Paint-Rect $b 14 (16 + $dy) 2 5 $c.ink
    Paint-Rect $b 14 (16 + $dy) 1 4 $c.gold
    Paint-Rect $b 14 (20 + $dy) 1 1 $c.goldDark

    # Arms oppose the leg stride; final four frames hold one arm up.
    $la = 15 + $dy + $(if ($phase -eq 1) { 1 } else { 0 })
    $ra = 15 + $dy + $(if ($phase -eq 3) { 1 } else { 0 })
    Paint-Rect $b 4 $la 3 6 $c.ink; Paint-Rect $b 5 ($la + 1) 1 4 $c.plum; Paint-Rect $b 5 ($la + 5) 1 1 $c.skin
    if ($gesture) {
        Paint-Rect $b 15 (13 + $dy) 3 3 $c.ink; Paint-Rect $b 16 (13 + $dy) 1 2 $c.plum
        Paint-Rect $b 17 (12 + $dy) 2 2 $c.skin
    } else {
        Paint-Rect $b 15 $ra 3 6 $c.ink; Paint-Rect $b 16 ($ra + 1) 1 4 $c.plum
        Paint-Rect $b 16 ($ra + 5) 1 1 $c.skin
    }

    # 10-pixel head on a roughly 23-pixel figure; stepped edges avoid a square mask.
    Paint-Rect $b 7 (2 + $dy) 6 1 $c.ink
    Paint-Rect $b 5 (3 + $dy) 10 9 $c.ink
    Paint-Rect $b 6 (12 + $dy) 8 2 $c.ink
    Paint-Rect $b 6 (6 + $dy) 8 6 $c.shade
    Paint-Rect $b 7 (7 + $dy) 7 5 $c.skin
    Paint-Rect $b 6 (3 + $dy) 8 5 $c.hair
    Paint-Rect $b 5 (5 + $dy) 3 5 $c.hair
    Paint-Rect $b 7 (2 + $dy) 5 1 $c.hair
    Paint-Rect $b 7 (4 + $dy) 6 1 $c.shine
    Paint-Rect $b 5 (7 + $dy) 1 3 $c.hairDark
    Paint-Rect $b 13 (4 + $dy) 1 3 $c.hairDark
    Paint-Rect $b 7 (6 + $dy) 1 3 $c.hairDark
    Paint-Rect $b 6 (9 + $dy) 2 2 $c.hair
    Paint-Rect $b 10 (7 + $dy) 4 1 $c.hair
    Paint-Rect $b 12 (9 + $dy) 1 1 $c.ink
    Paint-Rect $b 13 (11 + $dy) 1 1 $c.shade
    Paint-Rect $b 8 (12 + $dy) 4 1 $c.shade
    Paint-Rect $b 9 (13 + $dy) 3 1 $c.skin
}

$sheet = [System.Drawing.Bitmap]::new(40, 1400, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
try {
    $g = [System.Drawing.Graphics]::FromImage($sheet)
    try {
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
        for ($frame = 0; $frame -lt 25; $frame++) {
            $small = [System.Drawing.Bitmap]::new(20, 28, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
            try {
                Draw-Pose $small $frame
                $g.DrawImage($small, [System.Drawing.Rectangle]::new(0, $frame * 56, 40, 56),
                    [System.Drawing.Rectangle]::new(0, 0, 20, 28), [System.Drawing.GraphicsUnit]::Pixel)
            } finally { $small.Dispose() }
        }
    } finally { $g.Dispose() }
    $sheet.Save((Join-Path $out 'PrototypeTraveler.png'), [System.Drawing.Imaging.ImageFormat]::Png)

    $head = [System.Drawing.Bitmap]::new(16, 16, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $g = [System.Drawing.Graphics]::FromImage($head)
        try {
            $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
            $g.DrawImage($sheet, [System.Drawing.Rectangle]::new(0, 0, 16, 16),
                [System.Drawing.Rectangle]::new(10, 6, 20, 20), [System.Drawing.GraphicsUnit]::Pixel)
        } finally { $g.Dispose() }
        $head.Save((Join-Path $out 'PrototypeTraveler_Head.png'), [System.Drawing.Imaging.ImageFormat]::Png)
    } finally { $head.Dispose() }

    $preview = [System.Drawing.Bitmap]::new(960, 336, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    try {
        $g = [System.Drawing.Graphics]::FromImage($preview)
        try {
            $g.Clear([System.Drawing.Color]::FromArgb(112, 126, 142))
            $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
            $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::Half
            for ($frame = 0; $frame -lt 4; $frame++) {
                $g.DrawImage($sheet, [System.Drawing.Rectangle]::new($frame * 240, 0, 240, 336),
                    [System.Drawing.Rectangle]::new(0, $frame * 56, 40, 56), [System.Drawing.GraphicsUnit]::Pixel)
            }
        } finally { $g.Dispose() }
        $preview.Save($previewPath, [System.Drawing.Imaging.ImageFormat]::Png)
    } finally { $preview.Dispose() }
} finally { $sheet.Dispose() }

