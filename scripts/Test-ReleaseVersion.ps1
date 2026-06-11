param(
    [string]$ExpectedVersion = "2.0.11"
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..")

function Assert-Equal {
    param(
        [string]$Name,
        [string]$Actual,
        [string]$Expected
    )

    if ($Actual -ne $Expected) {
        throw "$Name expected '$Expected' but found '$Actual'."
    }
}

function Assert-Contains {
    param(
        [string]$Name,
        [string]$Content,
        [string]$Expected
    )

    if (-not $Content.Contains($Expected)) {
        throw "$Name does not contain '$Expected'."
    }
}

$resourcePath = Join-Path $root "resources\EdgeBrowsingPowerV$ExpectedVersion.zip"
if (-not (Test-Path $resourcePath)) {
    throw "Expected resource file is missing: $resourcePath"
}

[xml]$project = Get-Content (Join-Path $root "src\EdgeBrowsingPower\EdgeBrowsingPower.csproj")
$propertyGroup = $project.Project.PropertyGroup | Select-Object -First 1
Assert-Equal "Project Version" $propertyGroup.Version $ExpectedVersion
Assert-Equal "Project AssemblyVersion" $propertyGroup.AssemblyVersion "$ExpectedVersion.0"
Assert-Equal "Project FileVersion" $propertyGroup.FileVersion "$ExpectedVersion.0"
Assert-Equal "Project PackageVersion" $propertyGroup.PackageVersion $ExpectedVersion

[xml]$installerProject = Get-Content (Join-Path $root "installer\EdgeBrowsingPower.Installer\EdgeBrowsingPower.Installer.wixproj")
$installerPropertyGroup = $installerProject.Project.PropertyGroup | Select-Object -First 1
Assert-Equal "Installer ProductVersion" $installerPropertyGroup.ProductVersion $ExpectedVersion

$wxs = Get-Content (Join-Path $root "installer\EdgeBrowsingPower.Installer\Product.wxs") -Raw
Assert-Contains "Installer source" $wxs 'EdgeBrowsingPower.exe'
Assert-Contains "Installer version variable" $wxs 'Version="$(var.ProductVersion)"'

$manifestRoot = Join-Path $root "manifests\m\Microsoft\EdgeBrowsingPower\$ExpectedVersion"
$versionManifest = Get-Content (Join-Path $manifestRoot "Microsoft.EdgeBrowsingPower.yaml") -Raw
$installerManifest = Get-Content (Join-Path $manifestRoot "Microsoft.EdgeBrowsingPower.installer.yaml") -Raw
$localeManifest = Get-Content (Join-Path $manifestRoot "Microsoft.EdgeBrowsingPower.locale.en-US.yaml") -Raw

Assert-Contains "Winget version manifest" $versionManifest "PackageVersion: $ExpectedVersion"
Assert-Contains "Winget installer manifest" $installerManifest "PackageVersion: $ExpectedVersion"
Assert-Contains "Winget locale manifest" $localeManifest "PackageVersion: $ExpectedVersion"
Assert-Contains "Winget installer URL" $installerManifest "https://github.com/microsoft/WebBrowsingPowerRelease/releases/download/v$ExpectedVersion/EdgeBrowsingPower-$ExpectedVersion-win-x64.msi"

$shaMatch = [regex]::Match($installerManifest, "InstallerSha256:\s*([A-Fa-f0-9]{64})")
if (-not $shaMatch.Success) {
    throw "Winget InstallerSha256 must be a 64-character SHA-256 value."
}

$releaseDoc = Get-Content (Join-Path $root "docs\release.md") -Raw
Assert-Contains "Release documentation" $releaseDoc $ExpectedVersion
Assert-Contains "Release documentation" $releaseDoc "EdgeBrowsingPower-$ExpectedVersion-win-x64.msi"

Write-Host "Release version metadata is aligned to $ExpectedVersion."