#tool vswhere

private const string ArtifactsDirectory = "../Artifacts";

void CleanArtifacts()
{
    CleanDirectory(ArtifactsDirectory);
}

void StoreChocolatey(string nuspecPattern)
{
    StoreChocolatey(GetFiles(nuspecPattern));
}

void StoreChocolatey(IEnumerable<FilePath> nuspecPaths)
{
    var dir = $"{ArtifactsDirectory}/Chocolatey";
    EnsureDirectoryExists(dir);
    Information("Creating Chocolatey package(s) in directory {0}", dir);

    foreach (var nuspec in nuspecPaths)
    {
        ChocolateyPack(nuspec, new ChocolateyPackSettings { OutputDirectory = dir });
    }
}

void StoreNuGet(string nuspecPattern)
{
    StoreNuGet(GetFiles(nuspecPattern));
}

void StoreNuGet(IEnumerable<FilePath> nuspecPaths)
{
    var dir = $"{ArtifactsDirectory}/NuGet";
    EnsureDirectoryExists(dir);
    Information("Creating NuGet package(s) in directory {0}", dir);

    foreach (var nuspec in nuspecPaths)
    {
        NuGetPack(nuspec, new NuGetPackSettings { OutputDirectory = dir });
    }
}

void StoreArtifacts(string projectName, string filePattern)
{
    StoreArtifacts(projectName, GetFiles(filePattern));
}

void StoreArtifacts(string projectName, IEnumerable<FilePath> filePaths)
{
    var dir = $"{ArtifactsDirectory}/{projectName}";
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
    settings.ToolPath =
        VSWhereLatest(new VSWhereLatestSettings { Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" })
        .CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
    return settings;
}
