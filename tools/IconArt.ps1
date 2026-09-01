# Glyfo icon artwork — the vector definition of the mark, drawn with GDI+.
#
# Every concept draws into a square canvas of side $S using normalized coordinates, so one
# definition serves every asset from 16 px to 620 px. Small sizes get a simplified variant:
# strokes that read at 256 px turn to mush at 16 px, so the detail is dropped rather than shrunk.
#
# Dot-source this file; it defines Draw-ConceptA/B/C and Render-Icon.

Add-Type -AssemblyName System.Drawing

# Fluent blue. Light corner to dark corner along the diagonal, the way Windows 11 lights its icons.
$script:BlueLight = [System.Drawing.Color]::FromArgb(0x4C, 0xC2, 0xFF)
$script:BlueDark  = [System.Drawing.Color]::FromArgb(0x00, 0x67, 0xC0)
$script:BlueMid   = [System.Drawing.Color]::FromArgb(0x00, 0x78, 0xD4)
$script:Paper     = [System.Drawing.Color]::FromArgb(0xFF, 0xFF, 0xFF)

function New-GPath {
    New-Object System.Drawing.Drawing2D.GraphicsPath
}

function Add-RoundRect {
    param(
        [System.Drawing.Drawing2D.GraphicsPath]$Path,
        [single]$X, [single]$Y, [single]$W, [single]$H, [single]$R
    )
    $R = [single][Math]::Min($R, [Math]::Min($W, $H) / 2)
    if ($R -le 0.01) {
        $Path.AddRectangle((New-Object System.Drawing.RectangleF($X, $Y, $W, $H)))
        return
    }
    $d = $R * 2
    $Path.AddArc($X, $Y, $d, $d, 180, 90)
    $Path.AddArc($X + $W - $d, $Y, $d, $d, 270, 90)
    $Path.AddArc($X + $W - $d, $Y + $H - $d, $d, $d, 0, 90)
    $Path.AddArc($X, $Y + $H - $d, $d, $d, 90, 90)
    $Path.CloseFigure()
}

# A text line: a capsule, i.e. a rounded rect whose radius is half its height.
function Add-Capsule {
    param([System.Drawing.Drawing2D.GraphicsPath]$Path, [single]$X, [single]$Y, [single]$W, [single]$H)
    Add-RoundRect $Path $X $Y $W $H ($H / 2)
}

function Pt {
    param([single]$X, [single]$Y)
    New-Object System.Drawing.PointF -ArgumentList $X, $Y
}

function New-DiagonalBrush {
    param([single]$S, [System.Drawing.Color]$From = $script:BlueLight, [System.Drawing.Color]$To = $script:BlueDark)
    # Inflated by a hair: a gradient that starts exactly at the bounds wraps at the last pixel.
    $side = [single]($S + 2)
    $rect = New-Object System.Drawing.RectangleF -ArgumentList ([single]-1), ([single]-1), $side, $side
    New-Object System.Drawing.Drawing2D.LinearGradientBrush -ArgumentList $rect, $From, $To, ([single]45)
}

function New-StrokePen {
    param([System.Drawing.Brush]$Brush, [single]$Width)
    $pen = New-Object System.Drawing.Pen($Brush, $Width)
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
    $pen
}

# The four corner marks of a selection frame, drawn as L shapes with round caps and joins.
function Draw-Brackets {
    param(
        [System.Drawing.Graphics]$G, [System.Drawing.Pen]$Pen,
        [single]$X, [single]$Y, [single]$W, [single]$H, [single]$Arm
    )
    $r = $X + $W
    $b = $Y + $H
    $G.DrawLines($Pen, @((Pt $X ($Y + $Arm)), (Pt $X $Y), (Pt ($X + $Arm) $Y)))
    $G.DrawLines($Pen, @((Pt ($r - $Arm) $Y), (Pt $r $Y), (Pt $r ($Y + $Arm))))
    $G.DrawLines($Pen, @((Pt $r ($b - $Arm)), (Pt $r $b), (Pt ($r - $Arm) $b)))
    $G.DrawLines($Pen, @((Pt ($X + $Arm) $b), (Pt $X $b), (Pt $X ($b - $Arm))))
}

# ---------------------------------------------------------------------------------------------
# Concept A — selection frame with text inside. Transparent ground, one object, no plate:
# the Windows 11 house style, and the same corner-bracket vocabulary as Snipping Tool.
# ---------------------------------------------------------------------------------------------
function Draw-ConceptA {
    param([System.Drawing.Graphics]$G, [single]$S, [bool]$Simple)

    $brush = New-DiagonalBrush $S
    try {
        if ($Simple) {
            $stroke = 0.150 * $S
            $arm = 0.300 * $S
            $lines = @(@(0.28, 0.335, 0.44), @(0.28, 0.530, 0.30))
            $lh = 0.135 * $S
        }
        else {
            $stroke = 0.105 * $S
            $arm = 0.265 * $S
            $lines = @(@(0.295, 0.300, 0.410), @(0.295, 0.445, 0.410), @(0.295, 0.590, 0.250))
            $lh = 0.095 * $S
        }

        $inset = $stroke / 2 + 0.02 * $S
        $pen = New-StrokePen $brush $stroke
        try {
            Draw-Brackets $G $pen $inset $inset ($S - 2 * $inset) ($S - 2 * $inset) $arm
        }
        finally { $pen.Dispose() }

        $path = New-GPath
        try {
            foreach ($l in $lines) {
                Add-Capsule $path ($l[0] * $S) ($l[1] * $S) ($l[2] * $S) $lh
            }
            $G.FillPath($brush, $path)
        }
        finally { $path.Dispose() }
    }
    finally { $brush.Dispose() }
}

# ---------------------------------------------------------------------------------------------
# Concept B — solid tile. The whole square is the mark, so the silhouette survives 16 px and any
# background. Closest to Calculator / Store in the first-party set.
# ---------------------------------------------------------------------------------------------
function Draw-ConceptB {
    param([System.Drawing.Graphics]$G, [single]$S, [bool]$Simple)

    $brush = New-DiagonalBrush $S
    $white = New-Object System.Drawing.SolidBrush($script:Paper)
    try {
        $tile = New-GPath
        try {
            Add-RoundRect $tile 0 0 $S $S (0.225 * $S)
            $G.FillPath($brush, $tile)
        }
        finally { $tile.Dispose() }

        if ($Simple) {
            $path = New-GPath
            try {
                Add-Capsule $path (0.235 * $S) (0.330 * $S) (0.530 * $S) (0.115 * $S)
                Add-Capsule $path (0.235 * $S) (0.545 * $S) (0.360 * $S) (0.115 * $S)
                $G.FillPath($white, $path)
            }
            finally { $path.Dispose() }
            return
        }

        $pen = New-StrokePen $white (0.058 * $S)
        try {
            Draw-Brackets $G $pen (0.215 * $S) (0.215 * $S) (0.570 * $S) (0.570 * $S) (0.135 * $S)
        }
        finally { $pen.Dispose() }

        $path = New-GPath
        try {
            Add-Capsule $path (0.325 * $S) (0.415 * $S) (0.350 * $S) (0.072 * $S)
            Add-Capsule $path (0.325 * $S) (0.535 * $S) (0.230 * $S) (0.072 * $S)
            $G.FillPath($white, $path)
        }
        finally { $path.Dispose() }
    }
    finally {
        $brush.Dispose()
        $white.Dispose()
    }
}

# ---------------------------------------------------------------------------------------------
# Concept C — a page of text with the capture frame laid over it. Two stacked objects separated by
# a knockout gap, which is how Windows 11 builds its layered icons (Photos, Snipping Tool).
# ---------------------------------------------------------------------------------------------
function Draw-ConceptC {
    param([System.Drawing.Graphics]$G, [single]$S, [bool]$Simple)

    $brush = New-DiagonalBrush $S
    $paper = New-Object System.Drawing.SolidBrush($script:Paper)
    $ink = New-Object System.Drawing.SolidBrush($script:BlueMid)
    try {
        # The page. Outlined, because a white fill alone would vanish on a light background.
        $cardX = if ($Simple) { 0.060 * $S } else { 0.085 * $S }
        $cardY = 0.055 * $S
        $cardW = if ($Simple) { 0.545 * $S } else { 0.560 * $S }
        $cardH = if ($Simple) { 0.660 * $S } else { 0.720 * $S }
        $edge = if ($Simple) { 0.090 * $S } else { 0.058 * $S }

        $card = New-GPath
        try {
            Add-RoundRect $card $cardX $cardY $cardW $cardH (0.085 * $S)
            $G.FillPath($paper, $card)
            $pen = New-StrokePen $brush $edge
            try { $G.DrawPath($pen, $card) } finally { $pen.Dispose() }
        }
        finally { $card.Dispose() }

        # Ruled text on the page.
        $lines = if ($Simple) {
            @(@(0.165, 0.200, 0.310, 0.110),
              @(0.165, 0.390, 0.310, 0.110))
        }
        else {
            @(@(0.185, 0.190, 0.360, 0.070),
              @(0.185, 0.315, 0.360, 0.070),
              @(0.185, 0.440, 0.235, 0.070))
        }
        $path = New-GPath
        try {
            foreach ($l in $lines) {
                Add-Capsule $path ($l[0] * $S) ($l[1] * $S) ($l[2] * $S) ($l[3] * $S)
            }
            $G.FillPath($ink, $path)
        }
        finally { $path.Dispose() }

        # The capture frame, sitting on top and reaching past the page.
        $fx = if ($Simple) { 0.415 * $S } else { 0.395 * $S }
        $fy = if ($Simple) { 0.415 * $S } else { 0.395 * $S }
        $fw = if ($Simple) { 0.510 * $S } else { 0.530 * $S }
        $stroke = if ($Simple) { 0.130 * $S } else { 0.095 * $S }
        $arm = if ($Simple) { 0.215 * $S } else { 0.185 * $S }

        # Knockout first: erase a fatter copy of the frame straight through to transparency, so the
        # frame stays separate from the page on a light shell and on a dark one alike. Painting the
        # gap in white instead would show as a halo everywhere but on white.
        $gap = if ($Simple) { 0.045 * $S } else { 0.028 * $S }
        $clear = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0, 255, 255, 255))
        $gapPen = New-StrokePen $clear ($stroke + 2 * $gap)
        $previous = $G.CompositingMode
        $G.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
        try { Draw-Brackets $G $gapPen $fx $fy $fw $fw $arm }
        finally {
            $G.CompositingMode = $previous
            $gapPen.Dispose()
            $clear.Dispose()
        }

        $framePen = New-StrokePen $brush $stroke
        try { Draw-Brackets $G $framePen $fx $fy $fw $fw $arm } finally { $framePen.Dispose() }
    }
    finally {
        $brush.Dispose()
        $paper.Dispose()
        $ink.Dispose()
    }
}

# ---------------------------------------------------------------------------------------------
# The monochrome badge, for the lock screen. One flat white silhouette with the text knocked out;
# Windows tints it itself, so any colour here would be thrown away.
# ---------------------------------------------------------------------------------------------
function Draw-Badge {
    param([System.Drawing.Graphics]$G, [single]$S)

    $white = New-Object System.Drawing.SolidBrush($script:Paper)
    $clear = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(0, 255, 255, 255))
    try {
        $tile = New-GPath
        try {
            Add-RoundRect $tile 0 0 $S $S (0.225 * $S)
            $G.FillPath($white, $tile)
        }
        finally { $tile.Dispose() }

        $path = New-GPath
        $previous = $G.CompositingMode
        $G.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceCopy
        try {
            Add-Capsule $path (0.235 * $S) (0.330 * $S) (0.530 * $S) (0.115 * $S)
            Add-Capsule $path (0.235 * $S) (0.545 * $S) (0.360 * $S) (0.115 * $S)
            $G.FillPath($clear, $path)
        }
        finally {
            $G.CompositingMode = $previous
            $path.Dispose()
        }
    }
    finally {
        $white.Dispose()
        $clear.Dispose()
    }
}

# ---------------------------------------------------------------------------------------------

<#
.SYNOPSIS
Renders one concept into a transparent square bitmap of the given pixel size.
#>
function Render-Icon {
    param(
        [ValidateSet('A', 'B', 'C')][string]$Concept,
        [int]$Size,
        [int]$Supersample = 4,
        [switch]$Badge
    )

    $canvas = $Size * $Supersample
    $big = New-Object System.Drawing.Bitmap($canvas, $canvas, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($big)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic

    if ($Badge) {
        Draw-Badge $g ([single]$canvas)
    }
    else {
        # 32 px is the last size where the capture brackets still resolve into corners — it is also
        # the taskbar's size on a 100-150% display, so it is worth keeping the full mark there. At
        # 24 and below the brackets smear into the tile and only the two text bars survive.
        $simple = $Size -lt 32
        & "Draw-Concept$Concept" $g ([single]$canvas) $simple
    }
    $g.Dispose()

    if ($Supersample -eq 1) { return $big }

    $small = New-Object System.Drawing.Bitmap($Size, $Size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g2 = [System.Drawing.Graphics]::FromImage($small)
    $g2.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g2.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g2.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    # Without TileFlipXY the bicubic kernel samples past the edge and leaves a pale border.
    $attr = New-Object System.Drawing.Imaging.ImageAttributes
    $attr.SetWrapMode([System.Drawing.Drawing2D.WrapMode]::TileFlipXY)
    $dest = New-Object System.Drawing.Rectangle(0, 0, $Size, $Size)
    $g2.DrawImage($big, $dest, 0, 0, $canvas, $canvas, [System.Drawing.GraphicsUnit]::Pixel, $attr)
    $g2.Dispose()
    $attr.Dispose()
    $big.Dispose()
    $small
}
