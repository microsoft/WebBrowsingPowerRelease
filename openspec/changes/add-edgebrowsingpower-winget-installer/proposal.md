## Why

Users need to install `EdgeBrowsingPower` through winget without manually downloading .NET or unpacking a zip, and they need the app to appear in Windows Installed apps so it can be found and uninstalled normally. This change turns the existing release binary into a proper installer-driven distribution path and aligns the product version to the bundled resource version `2.0.11`.

## What Changes

- Add an installer artifact for `EdgeBrowsingPower` that installs the app directly on Windows.
- Update winget manifests so `winget install` downloads the installer from a GitHub Release asset and installs the app without a separate manual step.
- Register `EdgeBrowsingPower` in Windows Installed apps with product metadata and uninstall support.
- Align application, installer, winget manifest, and release metadata versions to `2.0.11`, matching `EdgeBrowsingPowerV2.0.11.zip`.
- Document the installer build, release asset naming, GitHub Release URL, and checksum update workflow.
- Non-goals: changing the setup dialog or zip extraction flow, changing the bundled zip contents, adding auto-update behavior, or publishing the GitHub Release from this change.

## Capabilities

### New Capabilities
- `winget-installer`: Covers installer packaging, Windows Installed apps registration, uninstall behavior, winget manifest metadata, GitHub Release installer URLs, checksums, and version alignment to the bundled resource version.

### Modified Capabilities

None.

## Impact

- Affected code and artifacts: project/package version metadata, installer definition files, winget manifests, release documentation, and generated release artifacts.
- User-visible behavior: users can run winget to download and install `EdgeBrowsingPower`, find it in Windows Installed apps, launch it after installation, and uninstall it through Windows or winget.
- .NET 10 compatibility: the installer must deploy a build that does not trigger a missing .NET runtime prompt on target machines; the current release direction is a self-contained .NET 10 Windows publish.
- Winget impact: installer manifests must use GitHub Release asset URLs, version `2.0.11`, correct installer type and architecture metadata, and checksums computed from the final published installer asset.