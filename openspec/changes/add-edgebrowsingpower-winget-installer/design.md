## Context

`EdgeBrowsingPower` is a .NET 10 Windows desktop app that already publishes as a self-contained win-x64 single-file executable plus required WPF native support files. The current release zip is runnable, but it is not an installer: winget can download an archive, yet that does not automatically provide Windows Installed apps registration, Add/Remove Programs metadata, or a normal uninstall entry.

The bundled deployment resource is named `EdgeBrowsingPowerV2.0.11.zip`, so this change treats `2.0.11` as the product, installer, and winget manifest version. The installer artifact will be uploaded to a GitHub Release and referenced by winget through a release asset URL.

## Goals / Non-Goals

**Goals:**
- Produce a Windows installer for `EdgeBrowsingPower` version `2.0.11`.
- Install the self-contained .NET 10 app without requiring a separate .NET Desktop Runtime install on target machines.
- Register `EdgeBrowsingPower` in Windows Installed apps with publisher, display version, install location, and uninstall support.
- Add winget manifests that download the installer from a GitHub Release asset and install directly.
- Keep release documentation repeatable, including installer build commands and checksum update steps.

**Non-Goals:**
- Changing the setup dialog, credential update behavior, or bundled zip contents.
- Implementing application auto-update.
- Publishing GitHub Releases automatically.
- Supporting non-Windows platforms or architectures other than win-x64 in this change.

## Decisions

### Package as MSI

Create an MSI installer around the existing self-contained publish output. MSI is supported by winget, registers cleanly in Windows Installed apps, provides standard uninstall behavior, and does not require a custom bootstrapper for this app's current needs.

Alternatives considered:
- Zip installer: rejected because it does not naturally satisfy Installed apps registration or uninstall expectations.
- Custom EXE installer: possible, but adds more custom behavior and validation surface than needed.
- MSIX: provides modern packaging, but can introduce signing and package identity requirements that are heavier than this release needs.

### Use WiX for installer authoring

Use WiX Toolset to define the MSI with a small installer project or build script checked into the repository. The installer should harvest or explicitly include the self-contained publish output, install per-user under LocalAppData, create a Start Menu shortcut if supported by the final authoring, and configure MajorUpgrade behavior so future versions replace older installations cleanly. Per-user scope keeps winget installation direct without requiring elevation while still registering in Windows Installed apps for the current user.

Alternatives considered:
- Visual Studio setup project: less automation-friendly for command-line release builds.
- Inno Setup: good for EXE installers, but MSI better matches winget metadata and Windows Installer uninstall semantics.

### Centralize version `2.0.11`

Set the app assembly/package metadata and installer metadata to `2.0.11`. The value must match the version embedded in `resources/EdgeBrowsingPowerV2.0.11.zip`; release validation should fail or explicitly call out any mismatch between the resource filename, project version, MSI ProductVersion, and winget PackageVersion.

Alternatives considered:
- Leave app version at SDK defaults: rejected because users and winget need a clear installable version.
- Use a different installer version from the resource version: rejected because the user explicitly requires alignment.

### Publish self-contained before packaging

The installer should consume the self-contained win-x64 publish output, not a framework-dependent build. This avoids the missing .NET runtime dialog on target machines and keeps winget installation direct.

Alternatives considered:
- Framework-dependent MSI with runtime prerequisite: smaller installer, but reintroduces the runtime prompt and adds prerequisite management.

### Reference GitHub Release MSI assets from winget

Winget manifests should point `InstallerUrl` to a GitHub Release asset such as `https://github.com/<owner>/<repo>/releases/download/v2.0.11/EdgeBrowsingPower-2.0.11-win-x64.msi`. The manifest checksum must be computed from the final uploaded MSI bytes and updated after the release asset is finalized.

Alternatives considered:
- Referencing repository raw files or local paths: rejected by project convention and unsuitable for winget users.

## Risks / Trade-offs

- WiX toolchain is not installed on a build machine -> Document the required tool/package restore command and keep installer generation repeatable from a clean checkout.
- Installer omits a native WPF support file from publish output -> Package from the complete publish directory and validate installed launch on a clean Windows environment.
- Version metadata drifts from `EdgeBrowsingPowerV2.0.11.zip` -> Add validation tasks that compare resource filename, project version, installer version, and winget version before release.
- Winget checksum changes after release asset replacement -> Compute the checksum only from the final GitHub Release asset and update manifests after upload.
- MSI upgrade behavior is misconfigured -> Use stable UpgradeCode and MajorUpgrade rules, then validate install, upgrade, and uninstall before release.