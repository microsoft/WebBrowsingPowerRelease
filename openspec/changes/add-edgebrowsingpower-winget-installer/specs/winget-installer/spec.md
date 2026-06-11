## ADDED Requirements

### Requirement: Produce an installer artifact
The release process SHALL produce a Windows installer artifact for `EdgeBrowsingPower` version `2.0.11` that installs the published win-x64 application.

#### Scenario: Installer artifact is generated
- **WHEN** the release installer build runs for version `2.0.11`
- **THEN** it produces a Windows installer artifact for `EdgeBrowsingPower`
- **AND** the artifact name identifies `EdgeBrowsingPower`, version `2.0.11`, and win-x64 architecture

#### Scenario: Installer contains published app files
- **WHEN** the installer artifact is inspected or installed
- **THEN** it includes the self-contained `EdgeBrowsingPower` application output required to launch on Windows
- **AND** it includes the bundled `EdgeBrowsingPowerV2.0.11.zip` resource through the application binary or packaged files

#### Scenario: Installer build fails on missing publish output
- **WHEN** the installer build runs without the required publish output
- **THEN** the build fails or reports a clear packaging error
- **AND** no incomplete installer is treated as a release artifact

### Requirement: Register installed application
The installer SHALL register `EdgeBrowsingPower` in Windows Installed apps with uninstall metadata.

#### Scenario: Installed app appears in Windows settings
- **WHEN** the user installs `EdgeBrowsingPower` using the installer
- **THEN** Windows Installed apps lists `EdgeBrowsingPower`
- **AND** the listed version is `2.0.11`

#### Scenario: Installed app launches
- **WHEN** the user launches `EdgeBrowsingPower` after installation
- **THEN** the app opens the setup dialog
- **AND** it does not show a missing .NET runtime prompt

#### Scenario: Install location contains app files
- **WHEN** installation completes successfully
- **THEN** the installed location contains the executable and required runtime support files
- **AND** the executable can access the bundled zip resource used by the create flow

### Requirement: Support uninstall
The installer SHALL support uninstalling `EdgeBrowsingPower` through Windows Installed apps and winget.

#### Scenario: Uninstall from Windows Installed apps
- **WHEN** the user uninstalls `EdgeBrowsingPower` from Windows Installed apps
- **THEN** the installed application files are removed
- **AND** the Installed apps entry is removed

#### Scenario: Uninstall from winget
- **WHEN** the user runs winget uninstall for the `EdgeBrowsingPower` package
- **THEN** winget invokes the registered uninstall operation
- **AND** the application is removed from the installed app list

#### Scenario: User-created deployment folders are preserved
- **WHEN** `EdgeBrowsingPower` is uninstalled
- **THEN** folders created by the app's setup flow outside the install directory are not deleted by the installer

### Requirement: Publish winget manifests for direct installation
The repository SHALL provide winget manifests for `EdgeBrowsingPower` version `2.0.11` that install from a GitHub Release installer asset.

#### Scenario: Winget manifest uses GitHub Release URL
- **WHEN** the winget installer manifest is reviewed
- **THEN** `InstallerUrl` references a GitHub Release asset URL
- **AND** the URL points to the version `2.0.11` installer artifact

#### Scenario: Winget manifest metadata matches installer
- **WHEN** the winget manifests are generated or updated
- **THEN** `PackageVersion` is `2.0.11`
- **AND** installer architecture, installer type, package identifier, package name, publisher, and checksum match the final installer artifact

#### Scenario: Winget install downloads and installs directly
- **WHEN** a user runs winget install for `EdgeBrowsingPower`
- **THEN** winget downloads the installer from the configured GitHub Release URL
- **AND** winget installs the app without requiring the user to manually extract a zip or install .NET separately

#### Scenario: Winget validation fails on placeholder release data
- **WHEN** winget manifests still contain placeholder URLs or checksums
- **THEN** manifest validation fails or the release checklist marks the manifests incomplete
- **AND** the manifests are not treated as ready for release submission

### Requirement: Align version with bundled resource
The application, installer, winget manifests, and release documentation SHALL use version `2.0.11`, matching the bundled resource file `EdgeBrowsingPowerV2.0.11.zip`.

#### Scenario: Version metadata is aligned
- **WHEN** release metadata is reviewed
- **THEN** application version metadata is `2.0.11`
- **AND** installer product version is `2.0.11`
- **AND** winget `PackageVersion` is `2.0.11`
- **AND** release documentation references version `2.0.11`

#### Scenario: Resource version mismatch is detected
- **WHEN** the bundled resource filename version does not match the application, installer, or winget version
- **THEN** validation reports the mismatch before release
- **AND** the release is not considered ready until the versions are aligned