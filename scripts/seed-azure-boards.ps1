<#
.SYNOPSIS
  Creates the demo stories in Azure Boards so the same backlog exists in Azure DevOps and GitHub.

.DESCRIPTION
  Reads scripts/stories.json and creates one work item per story with the story text as the
  description and the acceptance criteria in the Acceptance Criteria field. Prints the AB#<id>
  reference for each story; put it in the matching GitHub issue and in pull request descriptions
  to link the two.

  Requires the Azure CLI with the azure-devops extension, signed in to the organization
  (az login, or az devops login with a PAT).

.EXAMPLE
  ./scripts/seed-azure-boards.ps1 -Organization https://dev.azure.com/contoso -Project EquipmentRegister

.EXAMPLE
  ./scripts/seed-azure-boards.ps1 -Organization https://dev.azure.com/contoso -Project Demo -WorkItemType "Product Backlog Item" -WhatIf
#>
[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)] [string] $Organization,
    [Parameter(Mandatory)] [string] $Project,
    [string] $WorkItemType = 'User Story',
    [string] $Tag = 'equipment-register-demo'
)

$ErrorActionPreference = 'Stop'

function ConvertTo-Html([string] $text) {
    $encoded = [System.Net.WebUtility]::HtmlEncode($text)
    # Inline code in the source uses backticks; Azure Boards renders HTML.
    return [regex]::Replace($encoded, '`([^`]+)`', '<code>$1</code>')
}

$stories = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'stories.json') | ConvertFrom-Json
$created = @()

foreach ($story in $stories) {
    $description = '<p>' + ((ConvertTo-Html $story.story) -replace "`n", '<br>') + '</p>'
    if ($story.outOfScope) {
        $items = ($story.outOfScope | ForEach-Object { '<li>' + (ConvertTo-Html $_) + '</li>' }) -join ''
        $description += "<p><b>Out of scope</b></p><ul>$items</ul>"
    }
    $criteria = '<ol>' + (($story.acceptanceCriteria | ForEach-Object { '<li>' + (ConvertTo-Html $_) + '</li>' }) -join '') + '</ol>'

    if (-not $PSCmdlet.ShouldProcess("$Organization/$Project", "Create $WorkItemType '$($story.title)'")) {
        continue
    }

    $workItem = az boards work-item create `
        --organization $Organization `
        --project $Project `
        --type $WorkItemType `
        --title $story.title `
        --description $description `
        --fields "Microsoft.VSTS.Common.AcceptanceCriteria=$criteria" "System.Tags=$Tag" `
        --output json | ConvertFrom-Json

    if (-not $workItem.id) {
        throw "Azure Boards did not return a work item for '$($story.title)'."
    }

    $created += [pscustomobject]@{
        Reference = "AB#$($workItem.id)"
        Title     = $story.title
        Url       = "$Organization/$Project/_workitems/edit/$($workItem.id)"
    }
}

if ($created) {
    $created | Format-Table -AutoSize -Wrap
    Write-Host 'Add each reference to the matching GitHub issue and to pull request descriptions to link them.'
}
