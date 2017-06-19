#load "nuget:?package=cake.mug.tools"
#load "nuget:?package=cake.mug"

var target = Argument("target", "Default");
BuildParameters.Configuration = Argument("configuration", "Release");
BuildParameters.DupFinderExcludePattern = new string[] { "./**/CommandsTests.cs" };

PackageParameters.ChocolateySpecs.Add("../NuSpec/Chocolatey/Bumpy.Portable.nuspec");
PackageParameters.NuGetSpecs.Add("../NuSpec/NuGet/Bumpy.nuspec");

Task("Default")
    .IsDependentOn("Analyze")
    .IsDependentOn("CreatePackages")
    .Does(() =>
{
});

RunTarget(target);
