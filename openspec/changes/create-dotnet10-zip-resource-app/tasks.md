## 1. Project Setup

- [x] 1.1 Inspect the repository structure and choose the target location for the new `EdgeBrowsingPower` .NET 10 Windows desktop app project.
- [x] 1.2 Create the `EdgeBrowsingPower` .NET 10 app project and solution wiring using the selected Windows desktop UI framework.
- [x] 1.3 Add the deployment zip to the project as an embedded resource or packaged content asset included in publish output.
- [x] 1.4 Add a sample or real `parameters.abl.credentials.json` inside the bundled zip and confirm its username/password field names.

## 2. Dialog UI

- [x] 2.1 Build the launch dialog with `Exit` and `Create` buttons plus MSA account ID and password input fields.
- [x] 2.2 Mask the password input and avoid displaying the password anywhere else in the UI.
- [x] 2.3 Implement `Exit` so it closes the app without extracting files or modifying credentials.
- [x] 2.4 Validate that MSA account ID and password are non-empty before opening folder selection.

## 3. Creation Flow

- [x] 3.1 Open a system folder selection dialog after valid inputs are submitted with `Create`.
- [x] 3.2 Handle folder selection cancellation by returning to the setup dialog without extracting files.
- [x] 3.3 Extract the bundled zip resource into the selected destination folder.
- [x] 3.4 Detect and report missing or unreadable bundled zip resource errors without writing credentials.
- [x] 3.5 Detect and report extraction failures without reporting creation as complete.

## 4. Credentials Update

- [x] 4.1 Locate `parameters.abl.credentials.json` in the extracted content after successful extraction.
- [x] 4.2 Update the username/MSA account field and password field using structured JSON APIs while preserving valid JSON.
- [x] 4.3 Handle missing, invalid, or unwritable credentials file errors without exposing the entered password.
- [x] 4.4 Show a completion message only after extraction and credentials update both succeed.

## 5. Packaging and Release

- [x] 5.1 Add repeatable build/publish commands for the `EdgeBrowsingPower` .NET 10 app and document the expected output artifact.
- [ ] 5.2 Verify the published output includes the bundled zip resource or packaged content asset.
- [ ] 5.3 Create or update winget installer manifests so installer URLs point to GitHub Release assets.
- [ ] 5.4 Update winget manifest metadata and installer checksums for the published release artifact.

## 6. Validation

- [x] 6.1 Run .NET 10 build validation for the `EdgeBrowsingPower` app project.
- [ ] 6.2 Manually verify launch, `Exit`, missing-input validation, password masking, folder selection cancellation, successful extraction, and successful credential replacement.
- [ ] 6.3 Verify error paths for missing zip resource, extraction failure, missing credentials file, and invalid credentials JSON.
- [ ] 6.4 Validate the release artifact installs or runs on Windows and launches the setup dialog.
- [ ] 6.5 Validate winget manifests reference GitHub Release URLs and pass manifest validation.