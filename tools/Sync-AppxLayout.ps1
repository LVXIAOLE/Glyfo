# Rebuilds the loose AppX layout that "Add-AppxPackage -Register" installs from, out of the payload
# list the build itself wrote.
#
# Why this exists. A wapproj build refreshes bin\<plat>\<cfg>\<app>\ but does NOT refresh
# bin\<plat>\<cfg>\AppX\, which is the folder the registered package actually points at — that one is
# left over from a Visual Studio deploy and is only rewritten when the IDE deploys. Build from the
# command line and the app keeps running the code from whenever you last pressed F5, silently: the
# package registers, the app starts, the version number matches, and none of your changes are in it.
# It cost two debugging sessions before anyone thought to hash the two copies of Glyfo.dll.
#
# The loose bin\<plat>\<cfg>\ folder is not a substitute. Its AppxManifest.xml points the logo at
# Images\StoreLogo.png, and only the AppX layout has an Images\ — registering the loose folder fails
# with 0x80070003 (path not found).
#
# The recipe (.build.appxrecipe) is written by every build and lists all ~145 payload files as
# source path plus package-relative destination, so a layout rebuilt from it is by construction the
# one the build meant to produce. Copying is unconditional: a hash comparison would cost more than
# the copy, and this is the step whose whole purpose is not being clever about staleness.

[CmdletBinding()]
param(
    [string]$Recipe,
    [switch]$Register
)

$ErrorActionPreference = 'Stop'

# Resolved here rather than as a parameter default: $PSScriptRoot is not populated yet while the
# param block is being bound under Windows PowerShell 5.1, so the default silently came out relative.
if (-not $Recipe) {
    $Recipe = Join-Path $PSScriptRoot '..\Glyfo (Package)\bin\x64\Debug\Glyfo (Package).build.appxrecipe'
}

if (-not (Test-Path $Recipe)) {
    throw "No recipe at $Recipe - build the package project first."
}

$recipeXml = [xml](Get-Content $Recipe)
$layout = Join-Path (Split-Path $Recipe -Parent) 'AppX'

if (Test-Path $layout) {
    Remove-Item $layout -Recurse -Force
}
$null = New-Item $layout -ItemType Directory

# Selected by local-name so the MSBuild namespace does not have to be registered, and across all
# ItemGroups at once: the recipe splits the payload over several of them, and walking them through
# dotted property access yields a null for every group that happens not to contain this element.
$payload = $recipeXml.SelectNodes("//*[local-name()='AppxPackagedFile']")

# MSBuild percent-escapes the characters that are special to it, so every path below this project
# arrives as "Glyfo %28Package%29" - 65 of the 145 entries, all of them the icon assets. Unescaping
# is the exact inverse; a literal percent in a real path would itself have been escaped to %25.
function Expand-MsBuildPath([string]$value) { [uri]::UnescapeDataString($value) }

$copied = 0
foreach ($file in $payload) {
    $source = Expand-MsBuildPath $file.Include
    $target = Join-Path $layout (Expand-MsBuildPath $file.PackagePath)
    $parent = Split-Path $target -Parent
    if (-not (Test-Path $parent)) {
        $null = New-Item $parent -ItemType Directory -Force
    }
    Copy-Item -LiteralPath $source -Destination $target -Force
    $copied++
}

# The manifest is a separate item because it is generated rather than copied from a project folder.
$manifest = Expand-MsBuildPath $recipeXml.SelectSingleNode("//*[local-name()='AppXManifest']").Include
Copy-Item -LiteralPath $manifest -Destination (Join-Path $layout 'AppxManifest.xml') -Force

Write-Output "layout: $layout  ($copied payload files)"

if ($Register) {
    # -ForceUpdateFromAnyVersion because the version in the manifest does not move between debug
    # builds, so without it a same-version re-register is a no-op and you are back where you started.
    Add-AppxPackage -Register (Join-Path $layout 'AppxManifest.xml') -ForceUpdateFromAnyVersion
    Write-Output ("registered: " + (Get-AppxPackage LVLE.Glyfo).InstallLocation)
}
