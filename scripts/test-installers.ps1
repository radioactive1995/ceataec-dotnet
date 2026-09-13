# Disposable installation checks; never changes the real user's harness directories.
param([ValidateSet('bash', 'powershell')][string] $Installer = 'powershell')
$ErrorActionPreference = 'Stop'
$root = Join-Path ([IO.Path]::GetTempPath()) ('ceataec-install-test-' + [Guid]::NewGuid())
[IO.Directory]::CreateDirectory($root) | Out-Null

function Install([string] $Harness, [string] $UserDirectory, [bool] $Preview = $false) {
    if ($Installer -eq 'bash') {
        $arguments = @((Join-Path $PSScriptRoot 'install-skills.sh'), $Harness, '--user-home', $UserDirectory)
        if ($Preview) { $arguments += '--dry-run' }
        & bash @arguments
        if ($LASTEXITCODE -ne 0) { throw 'Bash installation failed.' }
    } else {
        & (Join-Path $PSScriptRoot 'install-skills.ps1') -Harness $Harness -UserHome $UserDirectory -DryRun:$Preview
    }
}
function Assert([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}
function Expect-Conflict([scriptblock] $Action) {
    $failed = $false
    try { & $Action } catch { $failed = $true }
    Assert $failed 'Expected a conflict to stop installation.'
}
try {
    foreach ($harness in @('cursor', 'claude', 'both')) {
        $userDirectory = Join-Path $root "$harness user with spaces"
        [IO.Directory]::CreateDirectory($userDirectory) | Out-Null
        Install $harness $userDirectory $true
        Assert (@(Get-ChildItem -LiteralPath $userDirectory -Force).Count -eq 0) 'Dry run wrote installation files.'
        Install $harness $userDirectory
        $directory = if ($harness -eq 'cursor') { '.cursor' } else { '.claude' }
        $destination = Join-Path (Join-Path $userDirectory $directory) 'skills'
        Assert (@(Get-ChildItem -LiteralPath $destination -Directory).Count -eq 3) 'Expected three skills.'
        foreach ($skill in @('review-score', 'refactor', 'scaffold')) {
            $folder = Join-Path $destination "ceataec-$skill"
            $text = Get-Content -LiteralPath (Join-Path $folder 'SKILL.md') -Raw
            Assert ($text.Contains("name: ceataec-$skill")) 'Skill name does not match its folder.'
            Assert ($text.Contains('(references/standard.md)')) 'Principles link was not adapted.'
            Assert (Test-Path -LiteralPath (Join-Path $folder 'references/standard.md')) 'Principles missing.'
        }
        $before = @(Get-ChildItem -LiteralPath $destination -File -Recurse | Get-FileHash | Select-Object -ExpandProperty Hash)
        Install $harness $userDirectory
        $after = @(Get-ChildItem -LiteralPath $destination -File -Recurse | Get-FileHash | Select-Object -ExpandProperty Hash)
        Assert (($before -join ',') -eq ($after -join ',')) 'Repeated install changed content.'
        if ($harness -eq 'both') {
            Assert (-not (Test-Path -LiteralPath (Join-Path $userDirectory '.cursor'))) 'Both created duplicate copies.'
            Expect-Conflict { Install 'cursor' $userDirectory }
        }
    }
    $conflictHome = Join-Path $root 'conflict'
    $edited = Join-Path $conflictHome '.claude/skills/ceataec-scaffold'
    [IO.Directory]::CreateDirectory($edited) | Out-Null
    Set-Content -LiteralPath (Join-Path $edited 'SKILL.md') -Value 'Local changes'
    $before = (Get-FileHash -LiteralPath (Join-Path $edited 'SKILL.md')).Hash
    Expect-Conflict { Install 'claude' $conflictHome }
    Assert ((Get-FileHash -LiteralPath (Join-Path $edited 'SKILL.md')).Hash -eq $before) 'Local changes were overwritten.'
    Assert (-not (Test-Path -LiteralPath (Join-Path $conflictHome '.claude/skills/ceataec-review-score'))) 'Conflict caused a partial install.'
    Write-Output "$Installer installer checks passed."
}
finally { Remove-Item -LiteralPath $root -Recurse -Force }

# Expected failing native commands must not determine the successful test process exit.
exit 0
