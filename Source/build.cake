#load "artifact.cake"
#load "changelog.cake"
#load "mstest2.cake"
#load "octokit.cake"

#addin Cake.Bumpy

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var githubReleaseToken = EnvironmentVariable("GITHUB_RELEASE_TOKEN");
var githubReleaseVersion = "0.10.0";

ArtifactsDirectory = "../Artifacts";

Task("Clean")
    .Does(() =>
{
    CleanArtifacts();
    CleanDirectories($"Bumpy*/bin/{configuration}");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Bumpy.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild("Bumpy.sln", new MSBuildSettings { Configuration = configuration, WarningsAsError = true });
    StoreBuildArtifacts("Bumpy", $"Bumpy/bin/{configuration}/**/*");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    MSTest2_VS2017($"*.Tests/bin/{configuration}/*.Tests.dll");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    StoreChocolateyArtifact("NuSpec/Chocolatey/Bumpy.Portable.nuspec");
    StoreNuGetArtifact("NuSpec/NuGet/Bumpy.nuspec");
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
{
    BumpyEnsure();
    EnsureChangelog("../CHANGELOG.md", githubReleaseVersion);
    PublishChocolateyArtifact("Bumpy.Portable", "https://push.chocolatey.org/");
    PublishNuGetArtifact("Bumpy", "https://www.nuget.org/api/v2/package");

    var settings = new OctokitSettings
    {
        Owner = "fwinkelbauer",
        Repository = "Bumpy",
        Token = githubReleaseToken,
        GitTag = githubReleaseVersion,
        TextBody = "More information about this release can be found in the [changelog](https://github.com/fwinkelbauer/Bumpy/blob/master/CHANGELOG.md)",
        IsDraft = true,
        IsPrerelease = false
    };

    var files = new[] { GetChocolateyArtifact("Bumpy.Portable"), GetNuGetArtifact("Bumpy") };
    var assets = files.Select(f => new OctokitAsset(f, "application/zip"));

    PublishGitHubReleaseWithArtifacts(settings, assets);
});

Task("Default").IsDependentOn("Pack").Does(() => { });

RunTarget(target);
