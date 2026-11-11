<#
.SYNOPSIS
  One-time setup after cloning: restores dependencies and turns on the pre-commit hook.
#>
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

Push-Location $root
try {
    git config core.hooksPath .githooks
    dotnet restore EquipmentRegister.sln

    Push-Location src/web
    try { npm ci } finally { Pop-Location }

    Write-Host ''
    Write-Host 'Ready. In VS Code run "Tasks: Run Build Task" to start the API and the web app, or:'
    Write-Host '  dotnet watch --no-hot-reload --project src/EquipmentRegister.Api run -c Release'
    Write-Host '  npm start --prefix src/web      ->  http://localhost:4200'
}
finally {
    Pop-Location
}
