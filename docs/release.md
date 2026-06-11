# EdgeBrowsingPower Release

Current release version: `2.0.11`, aligned with `resources/EdgeBrowsingPowerV2.0.11.zip`.

## Build

```powershell
dotnet build .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release
```

## Publish

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\artifacts\EdgeBrowsingPower\win-x64
Compress-Archive -Path .\artifacts\EdgeBrowsingPower\win-x64\* -DestinationPath .\artifacts\EdgeBrowsingPower\EdgeBrowsingPower-win-x64.zip -Force
```

Expected release artifact: `artifacts/EdgeBrowsingPower/EdgeBrowsingPower-win-x64.zip`.

Publish self-contained so target machines do not need a separate .NET Desktop Runtime installation.

The published artifact must include the embedded `EdgeBrowsingPowerV2.0.11.zip` resource in the application assembly. Validate by running the published app and completing a create flow into a temporary folder.

## MSI Installer

Build the self-contained app before building the installer:

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\artifacts\EdgeBrowsingPower\win-x64
dotnet build .\installer\EdgeBrowsingPower.Installer\EdgeBrowsingPower.Installer.wixproj -c Release -p:SuppressValidation=true
```

Expected installer artifact: `artifacts/EdgeBrowsingPower/installer/EdgeBrowsingPower-2.0.11-win-x64.msi`.

Validate release version alignment before publishing:

```powershell
.\scripts\Test-ReleaseVersion.ps1
```

## Winget

Winget installer manifests for EdgeBrowsingPower must point installer URLs to GitHub Release assets. For version `2.0.11`, use:

```yaml
InstallerUrl: https://github.com/microsoft/WebBrowsingPowerRelease/releases/download/v2.0.11/EdgeBrowsingPower-2.0.11-win-x64.msi
```

After publishing the GitHub Release asset, update the winget manifest checksum from the final artifact:

```powershell
Get-FileHash .\artifacts\EdgeBrowsingPower\installer\EdgeBrowsingPower-2.0.11-win-x64.msi -Algorithm SHA256
```

Current local MSI SHA-256: `EFAE11DE488BAED9C77FA6F277AA348926DE972E9CB13128B2B8ED56C7C948BE`.

Validate the winget manifests locally:

```powershell
winget validate .\manifests\m\Microsoft\EdgeBrowsingPower\2.0.11
```