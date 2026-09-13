# Install personal skills only. Works with Windows PowerShell 5.1 and PowerShell 7.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('cursor', 'claude', 'both')]
    [string] $Harness,
    [switch] $DryRun,
    [string] $UserHome = [Environment]::GetFolderPath('UserProfile')
)
$ErrorActionPreference = 'Stop'

function Assert-PlainDirectory([string] $Path) {
    $item = Get-Item -LiteralPath $Path -Force -ErrorAction SilentlyContinue
    if ($null -ne $item -and (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -or -not $item.PSIsContainer)) {
        throw "Not a plain directory: $Path"
    }
}
Assert-PlainDirectory $UserHome
if (-not (Test-Path -LiteralPath $UserHome -PathType Container)) { throw 'User directory must exist.' }
$UserHome = (Resolve-Path -LiteralPath $UserHome).Path
$directory = if ($Harness -eq 'cursor') { '.cursor' } else { '.claude' }
$other = if ($Harness -eq 'cursor') { '.claude' } else { '.cursor' }
$destination = Join-Path (Join-Path $UserHome $directory) 'skills'
$plugin = Join-Path $PSScriptRoot '../plugins/ceataec-dotnet'
$skills = @('review-score', 'refactor', 'scaffold')
$utf8 = New-Object System.Text.UTF8Encoding($false)
$stage = Join-Path ([IO.Path]::GetTempPath()) ([Guid]::NewGuid().ToString())
[IO.Directory]::CreateDirectory($stage) | Out-Null
try {
    foreach ($skill in $skills) {
        $folder = Join-Path $stage "ceataec-$skill"
        [IO.Directory]::CreateDirectory((Join-Path $folder 'references')) | Out-Null
        $text = [IO.File]::ReadAllText((Join-Path $plugin "skills/$skill/SKILL.md")).Replace("`r`n", "`n")
        $text = $text.Replace("name: $skill`n", "name: ceataec-$skill`n").Replace('../../spec/standard.md', 'references/standard.md')
        [IO.File]::WriteAllText((Join-Path $folder 'SKILL.md'), $text, $utf8)
        $principles = [IO.File]::ReadAllText((Join-Path $plugin 'spec/standard.md')).Replace("`r`n", "`n")
        [IO.File]::WriteAllText((Join-Path $folder 'references/standard.md'), $principles, $utf8)
    }
    Assert-PlainDirectory (Join-Path $UserHome $directory)
    Assert-PlainDirectory $destination
    foreach ($skill in $skills) {
        $target = Join-Path $destination "ceataec-$skill"
        $duplicate = Join-Path (Join-Path (Join-Path $UserHome $other) 'skills') "ceataec-$skill"
        if ($null -ne (Get-Item -LiteralPath $duplicate -Force -ErrorAction SilentlyContinue)) {
            throw "Duplicate installation at $duplicate. Inspect/remove it first; 'both' uses .claude for both clients."
        }
        Assert-PlainDirectory $target
        if (Test-Path -LiteralPath $target) {
            $items = @(Get-ChildItem -LiteralPath $target -Recurse -Force)
            if ($items | Where-Object { $_.Attributes -band [IO.FileAttributes]::ReparsePoint }) {
                throw "Refusing links inside $target"
            }
            $files = @($items | Where-Object { -not $_.PSIsContainer })
            if ($files.Count -ne 2) { throw "Existing content differs: $target. Preserve local edits and remove this skill folder before reinstalling." }
            foreach ($relative in @('SKILL.md', 'references/standard.md')) {
                $existing = Join-Path $target $relative
                $expected = Join-Path (Join-Path $stage "ceataec-$skill") $relative
                if (-not (Test-Path -LiteralPath $existing -PathType Leaf) -or
                    (Get-FileHash -LiteralPath $existing).Hash -ne (Get-FileHash -LiteralPath $expected).Hash) {
                    throw "Existing content differs: $target. Preserve local edits and remove this skill folder before reinstalling."
                }
            }
        }
    }
    if ($DryRun) { Write-Output "Would install three personal skills in $destination (no installation files written)."; return }
    [IO.Directory]::CreateDirectory($destination) | Out-Null
    foreach ($skill in $skills) {
        $target = Join-Path $destination "ceataec-$skill"
        if (-not (Test-Path -LiteralPath $target)) {
            New-Item -ItemType Directory -Path $target | Out-Null
            Copy-Item -Path (Join-Path (Join-Path $stage "ceataec-$skill") '*') -Destination $target -Recurse
        }
    }
    Write-Output "Installed three personal skills in $destination. Restart your client and select /ceataec-review-score, /ceataec-refactor or /ceataec-scaffold."
    if ($Harness -eq 'both') { Write-Output 'Cursor also discovers these Claude personal skills; no second copy is needed.' }
}
finally { Remove-Item -LiteralPath $stage -Recurse -Force }
