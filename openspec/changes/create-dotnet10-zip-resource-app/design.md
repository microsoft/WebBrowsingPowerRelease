## Context

The change introduces `EdgeBrowsingPower`, a Windows desktop utility targeting .NET 10. The app ships with a bundled zip archive, collects an MSA account ID and password from the user, asks for an output folder, extracts the archive, and updates `parameters.abl.credentials.json` in the extracted content.

The repository currently has no existing specs or app structure for this capability, so the implementation can choose a simple .NET 10 desktop project while staying compatible with the repository's release and winget expectations. Winget manifests are expected to use GitHub release URLs for installable artifacts.

## Goals / Non-Goals

**Goals:**
- Provide `EdgeBrowsingPower`, a .NET 10 Windows desktop app that opens directly to the setup dialog.
- Bundle the deployment zip with the app so no separate archive download is required at run time.
- Validate the MSA account ID and password fields before extraction starts.
- Let the user choose the destination folder through a system folder picker.
- Extract the zip resource and update `parameters.abl.credentials.json` with the entered values.
- Handle cancellation and errors with user-visible messages that do not expose the password.
- Produce release artifacts suitable for GitHub Releases and winget manifests.

**Non-Goals:**
- Authenticating the MSA account or verifying the password with Microsoft services.
- Persisting credentials outside the extracted `parameters.abl.credentials.json` file.
- Encrypting or otherwise changing the schema of the credentials file unless that schema already requires it.
- Providing a custom installer wizard beyond packaging the app for installation.

## Decisions

### Use a small .NET 10 Windows desktop app named EdgeBrowsingPower

Target .NET 10 for the `EdgeBrowsingPower` application and select the simplest Windows desktop UI framework that fits the repository when implementation begins. A single-window workflow is enough: launch dialog, input validation, folder picker, extraction, credential update, and completion or error result.

Alternatives considered:
- Console app: simpler to build, but it does not satisfy the required dialog and folder picker experience.
- Web app: unnecessary hosting/runtime surface for a local file deployment tool.

### Bundle the zip as an app resource or content asset

Package the zip with the application so the `Create` flow can extract it without network access. The implementation should prefer an embedded resource when practical; content copied beside the executable is acceptable if it is simpler for publish and winget packaging, as long as the release artifact remains self-contained.

Alternatives considered:
- Download the zip at run time: rejected because the requested behavior is to contain the zip file as a resource.
- Require the user to browse for a zip: rejected because it adds manual setup and changes the requested workflow.

### Use structured JSON updates for credentials

After extraction, locate `parameters.abl.credentials.json` in the extracted files and update the username and password fields using a JSON parser. The implementation should preserve valid JSON output and avoid ad hoc text replacement. Field names must be confirmed from the bundled file during implementation; if the existing file uses `username` and `password`, those are the update targets.

Alternatives considered:
- Plain string replacement: brittle and can corrupt JSON if formatting or escaping changes.
- Regenerating the entire file from scratch: risky if the archive contains additional settings that must be preserved.

### Keep password handling local and quiet

The password input should be masked in the UI, kept in memory only as long as needed, never logged, and never shown in error messages. The app will write the password only to the requested credentials file because that is the explicit functional requirement.

Alternatives considered:
- Storing the password in app settings: rejected because persistent app-level credential storage is out of scope.
- Encrypting the written value: only appropriate if the target credentials file schema already expects encrypted values.

### Publish installable release artifacts for winget

The app should have a repeatable build/publish path that creates a GitHub Release asset suitable for winget. Winget manifests must reference the GitHub release URL for that asset, and packaging validation should confirm the artifact installs and launches on Windows with the required .NET 10 runtime assumptions.

Alternatives considered:
- Local-only publish output without release packaging: insufficient for winget installation.
- Winget manifests pointing to non-release URLs: rejected by project convention.

## Risks / Trade-offs

- Credentials file schema differs from expected `username` and `password` fields -> Inspect the bundled `parameters.abl.credentials.json` during implementation and update the correct fields with structured JSON APIs.
- Extraction target already contains files -> Define overwrite behavior in implementation and surface a clear error or confirmation if overwriting would be unsafe.
- Bundled zip path or build action is misconfigured -> Add build and publish validation that confirms the archive is present in the release output.
- Password is exposed through logs or UI errors -> Avoid logging entered credentials and use generic error messages for write failures.
- Winget install fails because the release asset shape changes -> Validate the generated installer/package URL from a GitHub Release and update manifest checksums as part of release tasks.