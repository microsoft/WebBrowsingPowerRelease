## 1. Version and Release Metadata

- [x] 1.1 Set `EdgeBrowsingPower` project version metadata to `2.0.11`, including assembly/file/package version fields that affect installed app display metadata.
- [x] 1.2 Add a repeatable validation step that confirms the version in `resources/EdgeBrowsingPowerV2.0.11.zip` matches project, installer, winget, and release documentation versions.
- [x] 1.3 Update release documentation to name version `2.0.11` and the final installer artifact naming convention.

## 2. Installer Packaging

- [x] 2.1 Add installer authoring files for an MSI package that installs the self-contained win-x64 publish output for `EdgeBrowsingPower`.
- [x] 2.2 Configure MSI product metadata for `EdgeBrowsingPower`, version `2.0.11`, publisher, install directory, architecture, and stable upgrade identity.
- [x] 2.3 Include the executable and all required publish output files in the MSI so the installed app launches without a separate .NET Desktop Runtime install.
- [x] 2.4 Configure uninstall behavior so Windows Installed apps and winget can remove the installed app files and registration entry.
- [x] 2.5 Preserve user-created deployment folders outside the installer-owned install directory during uninstall.

## 3. Build and Artifact Workflow

- [x] 3.1 Add or document commands to publish the self-contained win-x64 app before building the MSI.
- [x] 3.2 Add or document commands to build `EdgeBrowsingPower-2.0.11-win-x64.msi` into the release artifacts folder.
- [x] 3.3 Ensure the installer build fails clearly if required publish output is missing or incomplete.
- [x] 3.4 Compute and document the SHA-256 checksum workflow for the final MSI artifact after the GitHub Release asset is finalized.

## 4. Winget Manifests

- [x] 4.1 Create or update winget manifests for package version `2.0.11` using the chosen package identifier, package name `EdgeBrowsingPower`, publisher metadata, installer type `msi`, and architecture `x64`.
- [x] 4.2 Set winget `InstallerUrl` to a GitHub Release asset URL for `EdgeBrowsingPower-2.0.11-win-x64.msi`.
- [x] 4.3 Set winget `InstallerSha256` from the final MSI bytes and keep placeholder values out of release-ready manifests.
- [ ] 4.4 Validate winget manifests locally and confirm they install directly from the GitHub Release URL.

## 5. Validation

- [x] 5.1 Run the .NET 10 Release build and self-contained win-x64 publish for `EdgeBrowsingPower`.
- [x] 5.2 Build the MSI and verify the artifact name, version, architecture, and checksum.
- [x] 5.3 Install the MSI on Windows and confirm `EdgeBrowsingPower` appears in Installed apps with version `2.0.11`.
- [x] 5.4 Launch the installed app and confirm the setup dialog opens without a missing .NET runtime prompt.
- [x] 5.5 Uninstall from Windows Installed apps and confirm installed app files and registration are removed.
- [ ] 5.6 Install and uninstall through winget using the manifests and confirm both operations complete successfully.