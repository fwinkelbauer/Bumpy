# Development

## Tools

- Windows
- Visual Studio 2017
- .NET Framework 4.6.1
- Chocolatey
- NuGet

## Setup to Publish a Release

- Register Chocolatey/NuGet API keys using `choco setApikey` and `nuget setApikey`
- Register a [GitHub token](https://github.com/settings/tokens) using the environment variable `GITHUB_RELEASE_TOKEN`

## Run a Build

- **Build artifacts:** `.\build.ps1`
- **Publish artifacts:** `.\build.ps1 -target publish`
