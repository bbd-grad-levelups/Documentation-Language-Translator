# C# CLI Application README

**Platform Compatibility:** This application is compatible with Windows and Linux machines.

## Usage

### 1. Help Command
- **Command:** help
- **Description:** Displays a list of available commands.

### 2. Login Command
- **Command:** login
- **Description:** Allows the user to log in with Google.

### 3. Logout Command
- **Command:** logout
- **Description:** Logs out of the application.

### 4. Languages Command
- **Command:** languages
- **Description:** Lists all supported languages.
- **Note:** The user must be logged in to execute this command.

### 5. Translate Command
- **Command:** translate \<filepath>
- **Description:** Uploads a file to be translated.
- **Usage:** `translate <filepath>`
- **Note:** The user must be logged in to execute this command.

### 6. Documents Command
- **Command:** documents
- **Description:** Returns a list of previously translated documents.
- **Note:** The user must be logged in to execute this command.

### 7. Download Command
- **Command:** download \<document_id> \<directory_path>
- **Description:** Downloads a translated document to the specified directory.
- **Usage:** `download <document_id> <directory_path>`
- **Note:** The user must be logged in to execute this command.

### 8. Clear Command
- **Command:** clear
- **Description:** Clears the console.

### 9. Exit Command
- **Command:** exit
- **Description:** Closes the application.

## Notes
- Some commands require the user to be logged in (languages, translate, documents, download).
- Use the `help` command to view the list of available commands at any time.
