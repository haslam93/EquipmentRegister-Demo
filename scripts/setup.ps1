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
    Write-Host 'Ready. Run the API with:  dotnet run --project src/EquipmentRegister.Api'
    Write-Host '       Run the web app:   npm start   (in src/web)  ->  http://localhost:4200'
}
finally {
    Pop-Location
}
