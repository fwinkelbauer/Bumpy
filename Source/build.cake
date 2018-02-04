#load "common.cake"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanArtifacts();
    CleanDirectories($"Bumpy*/bin/{configuration}");
    CleanDirectory("TestResults");
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
    StoreBuildArtifacts("Bumpy", $"Bumpy/bin/{configuration}/*");
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
    StoreChocolateyArtifacts("NuSpec/Chocolatey/Bumpy.Portable.nuspec");
    StoreNuGetArtifacts("NuSpec/NuGet/Bumpy.nuspec");
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
{
    PublishChocolateyArtifact("Bumpy.Portable", "https://push.chocolatey.org/");
    PublishNuGetArtifact("Bumpy", "https://www.nuget.org/api/v2/package");
});

Task("Default").IsDependentOn("Pack").Does(() => { });

RunTarget(target);
