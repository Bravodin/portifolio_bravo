<#
.SYNOPSIS
    Builds TRACE like the pipeline will (ADR-0009) and serves dist/ the way Azure Static Web Apps does.

.EXAMPLE
    ./scripts/run-local.ps1              # build, test, publish, prerender, serve on http://localhost:4280
    ./scripts/run-local.ps1 -SkipTests   # faster loop while editing content
    ./scripts/run-local.ps1 -NoServe     # only produce dist/
#>
param(
    [switch] $SkipTests,
    [switch] $NoServe,
    [int] $Port = 4280
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Invoke-Step([string] $Name, [scriptblock] $Command) {
    Write-Host "`n▶ $Name" -ForegroundColor Cyan
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "$Name failed (exit code $LASTEXITCODE)." }
}

Invoke-Step 'Build (warnings as errors)' { dotnet build Trace.slnx -c Release -warnaserror --nologo -v quiet }

if (-not $SkipTests) {
    Invoke-Step 'Format check' { dotnet format Trace.slnx --verify-no-changes }
    Invoke-Step 'Tests' { dotnet test --solution Trace.slnx -c Release --no-build }
}

Invoke-Step 'Publish Trace.Web' { dotnet publish src/Trace.Web -c Release -o artifacts/publish --nologo -v quiet }
Invoke-Step 'Prerender to dist/' { dotnet run --project tools/Trace.Prerender -c Release --no-build -- --publish-dir artifacts/publish --out dist }

if ($NoServe) { return }

if (-not (Get-Command npx -ErrorAction SilentlyContinue)) {
    throw 'Node.js is needed to serve dist/ like SWA does: winget install OpenJS.NodeJS.LTS'
}

Write-Host "`n▶ Serving dist/ on http://localhost:$Port (Ctrl+C to stop)" -ForegroundColor Green
npx -y @azure/static-web-apps-cli start dist --port $Port --swa-config-location dist
