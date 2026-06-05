## Why

Users need `EdgeBrowsingPower`, a small .NET 10 desktop utility that can deploy a bundled zip resource into a chosen folder and personalize the extracted credentials file from values they provide at run time. This change defines that user-facing packaging and setup flow so it can be implemented consistently and validated before release.

## What Changes

- Add `EdgeBrowsingPower`, a .NET 10 app that embeds or packages a zip file as an application resource.
- On launch, show a dialog with `Exit` and `Create` actions plus input fields for MSA account ID and password.
- When `Create` is selected, prompt the user to choose an extraction folder, unzip the bundled resource into that folder, and update `parameters.abl.credentials.json` with the supplied MSA account ID and password.
- Show clear completion and error feedback for validation failures, canceled folder selection, missing archive content, extraction failures, and credential replacement failures.
- Package the app for release in a way that remains compatible with .NET 10 and winget installation.
- Non-goals: account authentication against MSA services, password storage beyond writing the requested credentials file, changing the contents of the bundled zip except for the target credentials file, and building an installer UI separate from the app.

## Capabilities

### New Capabilities
- `zip-resource-setup-app`: Covers launching the .NET 10 setup dialog, collecting MSA credentials, selecting an extraction folder, unzipping the bundled resource, replacing credentials in `parameters.abl.credentials.json`, and packaging the app for release.

### Modified Capabilities

None.

## Impact

- Affected code: new `EdgeBrowsingPower` .NET 10 desktop app project, embedded resource configuration, extraction logic, dialog UI, JSON credential update logic, and release packaging scripts or project settings.
- Dependencies: .NET 10 SDK/runtime, desktop UI framework chosen during implementation, JSON processing APIs, and zip extraction APIs.
- User-visible behavior: running the app opens a credential-and-action dialog; choosing `Create` deploys the resource zip to a selected folder and writes the entered credentials into the extracted JSON file.
- .NET 10 compatibility: `EdgeBrowsingPower` implementation and packaging must target .NET 10 and validate build/publish output for the selected Windows desktop app type.
- Winget impact: winget manifests should reference GitHub release URLs for the app installer or packaged release artifacts; this change may require a new GitHub release asset URL if packaging output changes.