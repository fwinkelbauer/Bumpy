# Development

## Tools

- Windows
- Visual Studio 2017
- .NET Framework
- Chocolatey
  - Bumpy `choco install bumpy.portable`

## Setup to Publish a Release

- Register Chocolatey/NuGet API keys using `choco setApikey` and `nuget setApikey`
- Create the environment variable `GITHUB_RELEASE_TOKEN`

## Run a Build

- **Build:** `.\build.ps1`
- **Publish:** `.\build.ps1 -target publish`
