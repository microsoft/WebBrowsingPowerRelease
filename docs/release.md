# EdgeBrowsingPower Release

## Build

```powershell
dotnet build .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release
```

## Publish

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o .\artifacts\EdgeBrowsingPower\win-x64
Compress-Archive -Path .\artifacts\EdgeBrowsingPower\win-x64\* -DestinationPath .\artifacts\EdgeBrowsingPower\EdgeBrowsingPower-win-x64.zip -Force
```

Expected release artifact: `artifacts/EdgeBrowsingPower/EdgeBrowsingPower-win-x64.zip`.

The published artifact must include the embedded `EdgeBrowsingPowerV2.0.11.zip` resource in the application assembly. Validate by running the published app and completing a create flow into a temporary folder.

## Winget

Winget installer manifests for EdgeBrowsingPower must point installer URLs to GitHub Release assets, for example:

```yaml
InstallerUrl: https://github.com/<owner>/<repo>/releases/download/<tag>/EdgeBrowsingPower-win-x64.zip
```

After publishing the GitHub Release asset, update the winget manifest checksum from the final artifact:

```powershell
Get-FileHash .\artifacts\EdgeBrowsingPower\EdgeBrowsingPower-win-x64.zip -Algorithm SHA256
```