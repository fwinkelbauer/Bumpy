private const string ArtifactsDirectory = "../Artifacts";
private const string ChocolateyDirectory = ArtifactsDirectory + "/Chocolatey";
private const string NuGetDirectory = ArtifactsDirectory + "/NuGet";

void CleanArtifacts()
{
    CleanDirectory(ArtifactsDirectory);
}

void PublishChocolateyArtifact(string packageId, string pushSource)
{
    var files = GetFiles($"{ChocolateyDirectory}/{packageId}/*.nupkg");

    foreach (var file in files)
    {
        ChocolateyPush(file, new ChocolateyPushSettings { Source = pushSource });
    }
}

void StoreChocolateyArtifacts(string nuspecPattern)
{
    StoreChocolateyArtifacts(GetFiles(nuspecPattern));
}

void StoreChocolateyArtifacts(IEnumerable<FilePath> nuspecPaths)
{
    foreach (var nuspec in nuspecPaths)
    {
        var dir = $"{ChocolateyDirectory}/{nuspec.GetFilenameWithoutExtension()}";
        EnsureDirectoryExists(dir);
        ChocolateyPack(nuspec, new ChocolateyPackSettings { OutputDirectory = dir });
    }
}

void PublishNuGetArtifact(string packageId, string pushSource)
{
    var files = GetFiles($"{NuGetDirectory}/{packageId}/*.nupkg");

    foreach (var file in files)
    {
        NuGetPush(files, new NuGetPushSettings { Source = pushSource });
    }
}

void StoreNuGetArtifacts(string nuspecPattern)
{
    StoreNuGetArtifacts(GetFiles(nuspecPattern));
}

void StoreNuGetArtifacts(IEnumerable<FilePath> nuspecPaths)
{
    foreach (var nuspec in nuspecPaths)
    {
        var dir = $"{NuGetDirectory}/{nuspec.GetFilenameWithoutExtension()}";
        EnsureDirectoryExists(dir);
        NuGetPack(nuspec, new NuGetPackSettings { OutputDirectory = dir });
    }
}

void StoreBuildArtifacts(string projectName, string filePattern)
{
    StoreBuildArtifacts(projectName, GetFiles(filePattern));
}

void StoreBuildArtifacts(string projectName, IEnumerable<FilePath> filePaths)
{
    var dir = $"{ArtifactsDirectory}/Build/{projectName}";
    EnsureDirectoryExists(dir);

    Information("Copying artifacts to {0}", dir);
    CopyFiles(filePaths, dir);
    DeleteFiles($"{dir}/*.pdb");
    DeleteFiles($"{dir}/*.lastcodeanalysissucceeded");
    DeleteFiles($"{dir}/*.CodeAnalysisLog.xml");
}

void MSTest2_VS2017(string assemblyPattern)
{
    MSTest2_VS2017(GetFiles(assemblyPattern));
}

void MSTest2_VS2017(IEnumerable<FilePath> assemblyPaths)
{
    VSTest(assemblyPaths, FixToolPath(new VSTestSettings { Logger = "trx", TestAdapterPath = "." }));
}

// https://github.com/cake-build/cake/issues/1522
VSTestSettings FixToolPath(VSTestSettings settings)
{
    #tool vswhere
    settings.ToolPath =
        VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" })
        .CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
    return settings;
}
