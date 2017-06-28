#load "nuget:?package=cake.mug.tools"
#load "nuget:?package=cake.mug"

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

RunTarget(target);
