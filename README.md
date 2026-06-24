# EdgeBrowsingPower

## Introduction

EdgeBrowsingPower is a .NET 10 Windows desktop setup utility for the Edge Browsing Power deployment package. The app ships split `resources/EdgeBrowsingPowerV2.0.15.zip.*` archive parts and `resources/Browsing_BatteryLife_v2.0.15_and_YouTubeToS_v1.0.2.pdf` beside the executable, asks the user for an MSA account ID and password, lets the user choose an output folder, extracts the bundled package, copies the PDF into that folder, and updates `parameters.abl.credentials.json` in the extracted files.

The current project version is `2.0.15`, aligned with the bundled resource filename and the MSI/winget release metadata.

## Quick Start

For users installing a released build:

- Install `EdgeBrowsingPower` from the published MSI or winget package for version `2.0.15`.
- Launch `EdgeBrowsingPower` from the Start menu or Installed apps entry.
- Enter the MSA account ID and password required by the deployment package.
- The MSA account ID and password fields are for testing purposes. They are primarily used to test the Outlook Website. Please create a test account to run the test.
- Select `Create`, choose the destination folder, and wait for the completion message.
- Once unzip completed Execute "RunJob_Training.cmd" to launch Assessment in training mode. 
- Once "RunJob_Training.cmd" execute completed, Execute "RunJob_Trained.cmd" to launch Assessment in trained mode to get final result. 
- Use Windows Installed apps or winget to uninstall `EdgeBrowsingPower` when it is no longer needed.

For release validation before the GitHub Release asset is published:

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\EdgeBrowsingPower\win-x64
dotnet build .\installer\EdgeBrowsingPower.Installer\EdgeBrowsingPower.Installer.wixproj -c Release -p:SuppressValidation=true
.\scripts\Test-ReleaseVersion.ps1
winget validate .\manifests\m\Microsoft\EdgeBrowsingPower\2.0.15
```

## Repository Layout

- `src/EdgeBrowsingPower`: WPF application source.
- `resources`: Bundled deployment zip resource included in the application assembly.
- `installer/EdgeBrowsingPower.Installer`: WiX MSI installer project.
- `manifests/m/Microsoft/EdgeBrowsingPower/2.0.15`: winget manifests for the MSI release.
- `docs/release.md`: Release, MSI, checksum, and winget publishing commands.
- `openspec`: Planning artifacts and implementation task tracking.

## Build

Build the app in Release mode:

```powershell
dotnet build .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release
```

Publish a self-contained win-x64 build so target machines do not need a separate .NET Desktop Runtime installation. The project publishes an expanded ReadyToRun layout for fast startup and copies split `EdgeBrowsingPowerV2.0.15.zip.*` archive parts and `Browsing_BatteryLife_v2.0.15_and_YouTubeToS_v1.0.2.pdf` beside the executable:

```powershell
dotnet publish .\src\EdgeBrowsingPower\EdgeBrowsingPower.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\EdgeBrowsingPower\win-x64
```

Build the MSI installer:

```powershell
dotnet build .\installer\EdgeBrowsingPower.Installer\EdgeBrowsingPower.Installer.wixproj -c Release -p:SuppressValidation=true
```

Expected MSI artifact:

```text
artifacts/EdgeBrowsingPower/installer/EdgeBrowsingPower-2.0.15.msi
```

## Winget

The winget manifests are authored for package identifier `Microsoft.EdgeBrowsingPower`, version `2.0.15`, installer type `msi`, and architecture `x64`.

The installer manifest expects the MSI to be published as a GitHub Release asset at:

```text
https://github.com/microsoft/WebBrowsingPowerRelease/releases/download/v2.0.15/EdgeBrowsingPower-2.0.15.msi
```

After uploading the final MSI asset, recompute the checksum and update `InstallerSha256` if the bytes changed:

```powershell
Get-FileHash .\artifacts\EdgeBrowsingPower\installer\EdgeBrowsingPower-2.0.15.msi -Algorithm SHA256
winget validate .\manifests\m\Microsoft\EdgeBrowsingPower\2.0.15
```

## Security

EdgeBrowsingPower handles a password entered by the user. The password field is masked in the UI, the app does not display the password in error messages, and the app writes the password only to the extracted `parameters.abl.credentials.json` file required by the deployment package.

Choose the extraction destination carefully. The app extracts the bundled zip to the selected folder with overwrite enabled, then searches that folder for `parameters.abl.credentials.json`.

The release workflow signs the published application binaries and MSI with Azure Artifact Signing before uploading GitHub Release assets. See `docs/release.md` for the required CI/CD secrets and variables.

## Further Reading

- [Release documentation](docs/release.md)
- [Installer project](installer/EdgeBrowsingPower.Installer/EdgeBrowsingPower.Installer.wixproj)
- [Winget installer manifest](manifests/m/Microsoft/EdgeBrowsingPower/2.0.15/Microsoft.EdgeBrowsingPower.installer.yaml)
- [OpenSpec change: installer and winget packaging](openspec/changes/add-edgebrowsingpower-winget-installer/proposal.md)