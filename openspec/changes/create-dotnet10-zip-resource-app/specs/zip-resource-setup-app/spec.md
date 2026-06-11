## ADDED Requirements

### Requirement: Launch setup dialog
The `EdgeBrowsingPower` app SHALL target .NET 10 and SHALL show a setup dialog when launched.

#### Scenario: App launch displays required controls
- **WHEN** the user starts the app
- **THEN** the app displays a dialog containing `Exit` and `Create` buttons
- **AND** the dialog displays input fields for MSA account ID and password

#### Scenario: Exit closes the app
- **WHEN** the user selects `Exit`
- **THEN** the app closes without extracting the bundled zip resource
- **AND** no credentials file is modified

### Requirement: Validate credential inputs
The app SHALL require non-empty MSA account ID and password values before starting folder selection or extraction.

#### Scenario: Missing MSA account ID
- **WHEN** the user selects `Create` with an empty MSA account ID field
- **THEN** the app keeps the setup dialog open
- **AND** the app shows a validation message for the MSA account ID
- **AND** the app does not open the folder selection dialog

#### Scenario: Missing password
- **WHEN** the user selects `Create` with an empty password field
- **THEN** the app keeps the setup dialog open
- **AND** the app shows a validation message for the password
- **AND** the app does not open the folder selection dialog

#### Scenario: Password is masked
- **WHEN** the user enters a password
- **THEN** the app masks the password value in the dialog

### Requirement: Select extraction folder
The app SHALL open a system folder selection dialog after valid credentials are provided and before extraction begins.

#### Scenario: User selects a destination folder
- **WHEN** the user selects `Create` after entering valid credentials
- **AND** the user chooses a destination folder in the folder selection dialog
- **THEN** the app uses the selected folder as the extraction destination

#### Scenario: User cancels folder selection
- **WHEN** the user selects `Create` after entering valid credentials
- **AND** the user cancels the folder selection dialog
- **THEN** the app returns to the setup dialog without extracting the bundled zip resource
- **AND** no credentials file is modified

### Requirement: Extract bundled zip resource
The app SHALL include the deployment zip as an application resource or packaged content asset and SHALL extract it into the user-selected destination folder.

#### Scenario: Successful extraction
- **WHEN** the user provides valid credentials and selects a destination folder
- **THEN** the app extracts the bundled zip resource into that folder
- **AND** the extracted content includes `parameters.abl.credentials.json`

#### Scenario: Bundled zip resource is unavailable
- **WHEN** the user starts creation but the bundled zip resource cannot be found or opened
- **THEN** the app shows an error message
- **AND** the app does not write credentials

#### Scenario: Extraction fails
- **WHEN** the app cannot extract the bundled zip resource to the selected folder
- **THEN** the app shows an error message
- **AND** the app does not report creation as complete

### Requirement: Update extracted credentials file
After successful extraction, the app SHALL update `parameters.abl.credentials.json` in the extracted content with the MSA account ID and password entered by the user.

#### Scenario: Credentials are written after extraction
- **WHEN** extraction completes successfully
- **THEN** the app replaces the username value in `parameters.abl.credentials.json` with the entered MSA account ID
- **AND** the app replaces the password value in `parameters.abl.credentials.json` with the entered password
- **AND** the file remains valid JSON

#### Scenario: Credentials file is missing
- **WHEN** extraction completes but `parameters.abl.credentials.json` is not present in the extracted content
- **THEN** the app shows an error message
- **AND** the app does not report creation as complete

#### Scenario: Credentials file cannot be updated
- **WHEN** the app cannot parse or write `parameters.abl.credentials.json`
- **THEN** the app shows an error message
- **AND** the app does not expose the entered password in the message

### Requirement: Report completion and errors
The app SHALL provide user-visible status for successful creation and recoverable errors.

#### Scenario: Creation completes
- **WHEN** extraction and credential update complete successfully
- **THEN** the app shows a completion message identifying that creation succeeded

#### Scenario: Error messages protect password values
- **WHEN** any validation, extraction, or credential update error is shown
- **THEN** the app does not display the entered password in plain text

### Requirement: Package for .NET 10 and winget release
The app SHALL provide release artifacts compatible with .NET 10 on Windows, and winget manifests SHALL reference GitHub release URLs for those artifacts.

#### Scenario: Release artifact is produced
- **WHEN** the release build runs
- **THEN** it produces an installable or runnable Windows artifact for the `EdgeBrowsingPower` .NET 10 app
- **AND** the artifact includes the bundled zip resource or packaged content asset

#### Scenario: Winget manifest uses GitHub release URL
- **WHEN** winget installer manifests are created or updated for this app
- **THEN** each installer URL references a GitHub Release asset URL
- **AND** the manifest metadata and installer checksum match the published release artifact

#### Scenario: Installed app launches setup dialog
- **WHEN** the app is installed through the release artifact used by winget
- **THEN** launching the app opens the setup dialog