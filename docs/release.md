# EdgeBrowsingPower Release

Current release version: `2.0.15`, aligned with split `resources/EdgeBrowsingPowerV2.0.15.zip.*` archive parts.

## Build

```powershell
dotnet build .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release
```

## Publish

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\EdgeBrowsingPower\win-x64
Compress-Archive -Path .\artifacts\EdgeBrowsingPower\win-x64\* -DestinationPath .\artifacts\EdgeBrowsingPower\EdgeBrowsingPower-win-x64.zip -Force
```

Expected release artifact: `artifacts/EdgeBrowsingPower/EdgeBrowsingPower-win-x64.zip`.

Publish self-contained so target machines do not need a separate .NET Desktop Runtime installation. The project uses an expanded ReadyToRun publish layout to keep launch time under 3 seconds.

The published artifact must include split `EdgeBrowsingPowerV2.0.15.zip.*` archive parts and `Browsing_BatteryLife_v2.0.15_and_YouTubeToS_v1.0.2.pdf` beside `EdgeBrowsingPower.exe`. Each archive part must be smaller than 50 MB. Validate by running the published app and completing a create flow into a temporary folder.

## MSI Installer

Build the self-contained app before building the installer:

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\EdgeBrowsingPower\win-x64
dotnet build .\installer\EdgeBrowsingPower.Installer\EdgeBrowsingPower.Installer.wixproj -c Release -p:SuppressValidation=true
```

Expected installer artifact: `artifacts/EdgeBrowsingPower/installer/EdgeBrowsingPower-2.0.15-win-x64.msi`.

Validate release version alignment before publishing:

```powershell
.\scripts\Test-ReleaseVersion.ps1
```

## Winget

Winget installer manifests for EdgeBrowsingPower must point installer URLs to GitHub Release assets. For version `2.0.15`, use:

```yaml
InstallerUrl: https://github.com/microsoft/WebBrowsingPowerRelease/releases/download/v2.0.15/EdgeBrowsingPower-2.0.15-win-x64.msi
```

After publishing the GitHub Release asset, update the winget manifest checksum from the final artifact:

```powershell
Get-FileHash .\artifacts\EdgeBrowsingPower\installer\EdgeBrowsingPower-2.0.15-win-x64.msi -Algorithm SHA256
```

Current local MSI SHA-256: `D116C46BC08A3D536C9B930C55D228B87BCB9D0403435AAC84BA10CA5ADD6109`.

Validate the winget manifests locally:

```powershell
winget validate .\manifests\m\Microsoft\EdgeBrowsingPower\2.0.15
```