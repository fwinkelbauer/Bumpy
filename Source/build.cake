#load "artifact.cake"
#load "changelog.cake"
#load "mstest2.cake"
#load "octokit.cake"

#addin Cake.Bumpy

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = "0.11.0";

RepositoryHome = "..";

var octokitSettings = new OctokitSettings
{
    Owner = "fwinkelbauer",
    Repository = "Bumpy",
    Token = EnvironmentVariable("GITHUB_RELEASE_TOKEN"),
    GitTag = version,
    TextBody = "More information about this release can be found in the [changelog](https://github.com/fwinkelbauer/Bumpy/blob/master/CHANGELOG.md)",
    IsDraft = true,
    IsPrerelease = false
};

Task("Clean").Does(() =>
{
    CleanArtifacts();
    CleanDirectories($"Bumpy*/bin/{configuration}");
    CleanDirectory("TestResults");
});

Task("Restore").Does(() =>
{
    NuGetRestore("Bumpy.sln");
});

Task("Build").Does(() =>
{
    MSBuild("Bumpy.sln", new MSBuildSettings { Configuration = configuration, WarningsAsError = true });
    StoreBuildArtifacts("Bumpy", $"Bumpy/bin/{configuration}/**/*");
});

Task("Test").Does(() =>
{
    MSTest2_VS2017($"*.Tests/bin/{configuration}/*.Tests.dll");
});

Task("CreatePackages").Does(() =>
{
    PackChocolateyArtifacts("NuSpec/Chocolatey/**/*.nuspec");
    PackNuGetArtifacts("NuSpec/NuGet/**/*.nuspec");
});

Task("PushPackages").Does(() =>
{
    BumpyEnsure();
    EnsureChangelog("../CHANGELOG.md", version);

    PublishChocolateyArtifact("Bumpy.Portable", "https://push.chocolatey.org/");
    PublishNuGetArtifact("Bumpy", "https://www.nuget.org/api/v2/package");

    var mime = "application/zip";
    PublishGitHubReleaseWithArtifacts(
        octokitSettings,
        new OctokitAsset(GetChocolateyArtifact("Bumpy.Portable"), mime),
        new OctokitAsset(GetNuGetArtifact("Bumpy"), mime));
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("CreatePackages");

Task("Publish")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("CreatePackages")
    .IsDependentOn("PushPackages");

RunTarget(target);
