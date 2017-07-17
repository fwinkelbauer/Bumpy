#tool "nuget:?package=gitreleasemanager"

#load "nuget:?package=cake.mug.tools"
#load "nuget:?package=cake.mug"

var user = EnvironmentVariable("GITHUB_USERNAME");
var password = EnvironmentVariable("GITHUB_PASSWORD");
var owner = "fwinkelbauer";
var repository = "Bumpy";
var milestone = "0.3.0";

var target = Argument("target", "Default");
BuildParameters.Configuration = Argument("configuration", "Release");
BuildParameters.DupFinderExcludePattern = new string[] { "./**/CommandsTests.cs" };

PackageParameters.ChocolateySpecs.Add("../NuSpec/Chocolatey/Bumpy.Portable.nuspec");
PackageParameters.ChocolateyPushSource = "https://push.chocolatey.org/";

PackageParameters.NuGetSpecs.Add("../NuSpec/NuGet/Bumpy.nuspec");
PackageParameters.NuGetPushSource = "https://www.nuget.org/api/v2/package";

Task("Default")
    .IsDependentOn("Analyze")
    .IsDependentOn("CreatePackages")
    .Does(() =>
{
});

Task("CreateGitHubReleaseDraft")
    .Does(() =>
{
    GitReleaseManagerCreate(user, password, owner, repository, new GitReleaseManagerCreateSettings() {
        Milestone = milestone,
        Name = milestone
    });
});

Task("PublishGitHubRelease")
    .IsDependentOn("CreatePackages")
    .Does(() =>
{
    if (DirectoryExists(BuildArtifactParameters.ChocolateyDir))
    {
        foreach (var package in GetFiles(BuildArtifactParameters.ChocolateyDir + "/**/*.nupkg"))
        {
            GitReleaseManagerAddAssets(user, password, owner, repository, milestone, package.ToString());
        }
    }

    if (DirectoryExists(BuildArtifactParameters.NuGetDir))
    {
        foreach (var package in GetFiles(BuildArtifactParameters.NuGetDir + "/**/*.nupkg"))
        {
            GitReleaseManagerAddAssets(user, password, owner, repository, milestone, package.ToString());
        }
    }

    GitReleaseManagerPublish(user, password, owner, repository, milestone);
    GitReleaseManagerClose(user, password, owner, repository, milestone);
});

Task("Publish")
    .IsDependentOn("Analyze")
    .IsDependentOn("PushPackages")
    .IsDependentOn("PublishGitHubRelease")
    .Does(() =>
{
});

RunTarget(target);
