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
    StoreArtifacts("Bumpy", $"Bumpy/bin/{configuration}/*");
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
    StoreChocolatey("NuSpec/Chocolatey/Bumpy.Portable.nuspec");
    StoreNuGet("NuSpec/NuGet/Bumpy.nuspec");
});

Task("Default").IsDependentOn("Pack").Does(() => { });

RunTarget(target);
